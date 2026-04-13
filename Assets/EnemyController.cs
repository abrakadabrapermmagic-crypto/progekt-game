using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [Header("Movement")]
    [SerializeField] private float chaseDistance = 20f;
    [SerializeField] private float attackDistance = 2.2f;
    [SerializeField] private float pathUpdateInterval = 0.2f;
    [SerializeField] private float faceTargetSpeed = 10f;

    [Header("Attack")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float attackDuration = 0.9f;   // запасной сброс атаки
    [SerializeField] private EnemyWeaponTrigger weaponTrigger;

    [Header("Animator State Names")]
    [SerializeField] private string attackStateName = "Attack";
    [SerializeField] private string fallbackAttackStateName = "Attack ";

    [Header("Death")]
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyDelay = 3f;

    private NavMeshAgent agent;
    private Animator anim;
    private PlayerHealth playerHealth;

    private bool isDead;
    private bool isAttacking;
    private float nextPathUpdateTime;
    private float nextAttackTime;
    private float attackFinishTime;


    [SerializeField] private float attackWindup = 0.25f;   // через сколько после старта атаки включить меч
    [SerializeField] private float attackActiveTime = 0.25f; // сколько меч активен

    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private readonly int AttackHashWithSpace = Animator.StringToHash("Attack ");
    private readonly int DeathHash = Animator.StringToHash("Death");
    private bool HasParameter(string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in anim.parameters)
        {
            if (param.name == paramName && param.type == type)
                return true;
        }

        return false;
    }
    private System.Collections.IEnumerator AttackRoutine()
    {
        StopMovement();

        // Ждём момент удара
        yield return new WaitForSeconds(attackWindup);

        if (!isDead && weaponTrigger != null)
            weaponTrigger.BeginAttack(damage);

        // Окно нанесения урона
        yield return new WaitForSeconds(attackActiveTime);

        if (weaponTrigger != null)
            weaponTrigger.EndAttack();

        isAttacking = false;
    }
    private bool HasState(string stateName, int layer = 0)
    {
        if (anim == null || layer < 0) return false;
        return anim.HasState(layer, Animator.StringToHash(stateName));
    }
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
        if (isDead)
            return;

        if (target == null)
            FindTarget();

        if (target == null)
        {
            StopMovement();
            UpdateAnimator();
            return;
        }

        if (playerHealth != null && playerHealth.IsDead)
        {
            StopMovement();
            UpdateAnimator();
            return;
        }

        // Фолбэк: если event конца атаки не пришёл, сами сбрасываем атаку по таймеру
        if (isAttacking && Time.time >= attackFinishTime)
        {
            ForceFinishAttack();
        }

        float distance = Vector3.Distance(transform.position, target.position);

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
            ResumeMovement();

            if (Time.time >= nextPathUpdateTime && agent.enabled && agent.isOnNavMesh && !agent.isStopped)
            {
                agent.SetDestination(target.position);
                nextPathUpdateTime = Time.time + pathUpdateInterval;
            }
        }
        else if (!isAttacking)
        {
            StopMovement();
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
        nextAttackTime = Time.time + attackCooldown;
        attackFinishTime = Time.time + attackDuration;

        StopMovement();
        weaponTrigger?.EndAttack();

        bool startedAttack = false;

        if (HasParameter("Attack", AnimatorControllerParameterType.Trigger))
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");
            startedAttack = true;
        }
        else if (HasState("Attack"))
        {
            anim.CrossFadeInFixedTime("Attack", 0.05f, 0);
            startedAttack = true;
        }

        if (!startedAttack)
            Debug.LogWarning($"{name}: Атака логически запущена, но в Animator нет trigger/state Attack.");

        StartCoroutine(AttackRoutine());
    }

    private void ForceFinishAttack()
    {
        if (weaponTrigger != null)
            weaponTrigger.EndAttack();

        isAttacking = false;
    }

    // Animation Event
    public void Animation_BeginAttackWindow()
    {
        if (isDead || weaponTrigger == null) return;
        weaponTrigger.BeginAttack(damage);
    }

    // Animation Event
    public void Animation_EndAttackWindow()
    {
        if (weaponTrigger == null) return;
        weaponTrigger.EndAttack();
    }

    // Animation Event
    public void Animation_AttackFinished()
    {
        isAttacking = false;

        if (weaponTrigger != null)
            weaponTrigger.EndAttack();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;
        weaponTrigger?.EndAttack();

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;

            if (agent.isOnNavMesh)
                agent.ResetPath();

            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        if (HasParameter("Death", AnimatorControllerParameterType.Trigger))
            anim.SetTrigger("Death");
        else if (HasState("Death"))
            anim.CrossFadeInFixedTime("Death", 0.05f, 0);

        if (destroyOnDeath)
            Destroy(gameObject, destroyDelay);
    }

    private void StopMovement()
    {
        if (agent == null || !agent.enabled) return;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    private void ResumeMovement()
    {
        if (agent == null || !agent.enabled) return;

        agent.isStopped = false;
    }

    private void FaceTarget()
    {
        if (target == null) return;

        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            faceTargetSpeed * Time.deltaTime
        );
    }

    private void UpdateAnimator()
    {
        if (anim == null) return;

        float normalizedSpeed = 0f;

        if (agent != null && agent.enabled && agent.speed > 0f)
            normalizedSpeed = agent.velocity.magnitude / agent.speed;

        if (HasParameter("Speed", AnimatorControllerParameterType.Float))
            anim.SetFloat("Speed", normalizedSpeed, 0.1f, Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}