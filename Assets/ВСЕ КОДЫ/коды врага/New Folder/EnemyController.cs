using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    // Перечисление для отслеживания состояния бота
    public enum EnemyState { Idle, Chasing, Attacking, Dead }

    [Header("Current Status")]
    public EnemyState CurrentState = EnemyState.Idle;

    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [Header("Movement")]
    [SerializeField] private float chaseDistance = 20f;
    [SerializeField] private float attackDistance = 2.2f;
    [SerializeField] private float pathUpdateInterval = 0.2f;
    [SerializeField] private float faceTargetSpeed = 10f;

    [Header("Combat Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private EnemyWeaponTrigger weaponTrigger;

    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyDelay = 3f;

    private NavMeshAgent agent;
    private Animator anim;
    private PlayerHealth playerHealth;

    private bool isDead;
    private bool isAttacking;
    private float nextPathUpdateTime;
    private float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (weaponTrigger == null)
            weaponTrigger = GetComponentInChildren<EnemyWeaponTrigger>(true);
    }

    private void Start()
    {
        FindTarget();
    }

    private void Update()
    {
        if (isDead) return;

        if (target == null) FindTarget();

        // Если цели нет или она мертва — отдыхаем
        if (target == null || (playerHealth != null && playerHealth.IsDead))
        {
            StopMovement();
            CurrentState = EnemyState.Idle;
            UpdateAnimator();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        // Логика переключения состояний
        if (distance <= attackDistance)
        {
            StopMovement();
            FaceTarget();

            if (!isAttacking && Time.time >= nextAttackTime)
            {
                StartAttack();
            }
        }
        else if (!isAttacking && distance <= chaseDistance)
        {
            CurrentState = EnemyState.Chasing;
            ResumeMovement();

            if (Time.time >= nextPathUpdateTime && agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(target.position);
                nextPathUpdateTime = Time.time + pathUpdateInterval;
            }
        }
        else if (!isAttacking)
        {
            StopMovement();
            CurrentState = EnemyState.Idle;
        }

        UpdateAnimator();
    }

    private void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        target = player.transform;
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void StartAttack()
    {
        isAttacking = true;
        CurrentState = EnemyState.Attacking;
        nextAttackTime = Time.time + attackCooldown;

        StopMovement();

        // Запускаем анимацию. 
        // Логика нанесения урона теперь полностью внутри Animation Events.
        if (HasParameter("Attack", AnimatorControllerParameterType.Trigger))
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");
        }
    }

    // --- МЕТОДЫ ДЛЯ ANIMATION EVENTS ---

    // 1. Вызывается в момент начала активной фазы удара (замах завершен)
    public void Animation_BeginAttackWindow()
    {
        if (isDead || weaponTrigger == null) return;
        weaponTrigger.BeginAttack(damage);
    }

    // 2. Вызывается, когда удар прошел и меч нужно "выключить"
    public void Animation_EndAttackWindow()
    {
        if (weaponTrigger != null)
            weaponTrigger.EndAttack();
    }

    // 3. Вызывается в самом конце анимации, чтобы бот снова мог ходить
    public void Animation_AttackFinished()
    {
        isAttacking = false;
        if (weaponTrigger != null)
            weaponTrigger.EndAttack();
    }

    // -----------------------------------

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;
        CurrentState = EnemyState.Dead;
        weaponTrigger?.EndAttack();

        if (agent != null) agent.enabled = false;

        if (HasParameter("Death", AnimatorControllerParameterType.Trigger))
            anim.SetTrigger("Death");

        if (destroyOnDeath)
            Destroy(gameObject, destroyDelay);
    }

    private void StopMovement()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    private void ResumeMovement()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = false;
    }

    private void FaceTarget()
    {
        if (target == null) return;
        Vector3 lookDirection = (target.position - transform.position).normalized;
        lookDirection.y = 0;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, faceTargetSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimator()
    {
        if (anim == null) return;
        float speed = (agent != null && agent.enabled) ? agent.velocity.magnitude / agent.speed : 0;

        if (HasParameter("Speed", AnimatorControllerParameterType.Float))
            anim.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }

    private bool HasParameter(string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in anim.parameters)
        {
            if (param.name == paramName && param.type == type) return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}