using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 10;
    public float attackCooldown = 1.5f;
    public float attackRange = 2f; // Расстояние для удара

    private float lastAttackTime;
    private Transform playerTransform;
    private EnemyAI enemyAI;
    private EnemyAnimatorController animController;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        animController = GetComponent<EnemyAnimatorController>();
        // Ищем игрока по тегу один раз при старте
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Если игрок рядом и кулдаун прошел
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
    }

    // ВАЖНО: Этот метод привяжи к Animation Event в окне Animation
    // На том кадре анимации, где происходит сам удар
    public void HitPlayer()
    {
        if (playerTransform == null) return;

        // Проверяем расстояние еще раз в момент удара (вдруг игрок убежал)
        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange + 0.5f)
        {
            PlayerHealth health = playerTransform.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damage);
        }
    }

    // Этот метод вызывается в самом конце анимации атаки
    public void ResumeMoveAfterAttack()
    {
        if (enemyAI != null) enemyAI.ResumeAfterAttack();
    }
}