using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 10;
    public float attackCooldown = 1.5f;
    public float attackRange = 2f;
    [SerializeField] private float hitRangeTolerance = 0.5f;
    [SerializeField] private bool applyDamageImmediately = true;

    private float lastAttackTime;
    private Transform playerTransform;
    private PlayerHealth playerHealth;

    // Поддерживаем и новый EnemyAAI, и старый EnemyAI.
    private EnemyAAI enemyAAI;
    private EnemyAI enemyAI;
    private EnemyAnimatorController animController;
    private Animator animator;

    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int attackHashWithSpace = Animator.StringToHash("Attack ");

    private void Awake()
    {
        enemyAAI = GetComponent<EnemyAAI>();
        enemyAI = GetComponent<EnemyAI>();
        animController = GetComponent<EnemyAnimatorController>();
        animator = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth = player.GetComponentInChildren<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        lastAttackTime = Time.time;

        if (enemyAAI != null) enemyAAI.StopForAttack();
        else if (enemyAI != null) enemyAI.StopForAttack();

        if (animController != null)
        {
            animController.PlayAttack();
        }
        else if (animator != null)
        {
            // Fallback на случай, если EnemyAnimatorController не добавлен на объект.
            animator.ResetTrigger(attackHash);
            animator.ResetTrigger(attackHashWithSpace);
            animator.SetTrigger(attackHash);
            animator.SetTrigger(attackHashWithSpace);
        }

        // Урон наносится сразу, даже если в атакующем клипе нет Animation Event.
        if (applyDamageImmediately)
            HitPlayer();
    }

    // Можно оставить привязку к Animation Event — метод остается совместимым.
    public void HitPlayer()
    {
        if (playerTransform == null) return;

        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange + hitRangeTolerance)
        {
            if (playerHealth == null)
                playerHealth = playerTransform.GetComponentInChildren<PlayerHealth>();

            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
    }

    public void ResumeMoveAfterAttack()
    {
        if (enemyAAI != null) enemyAAI.ResumeAfterAttack();
        else if (enemyAI != null) enemyAI.ResumeAfterAttack();
    }
}
