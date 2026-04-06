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
    private EnemyAI enemyAI;
    private EnemyAnimatorController animController;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        animController = GetComponent<EnemyAnimatorController>();

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

        if (enemyAI != null) enemyAI.StopForAttack();
        if (animController != null) animController.PlayAttack();

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
        if (enemyAI != null) enemyAI.ResumeAfterAttack();
    }
}
