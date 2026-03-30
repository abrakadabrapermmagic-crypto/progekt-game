using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health System")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private AudioClip deathSound;

    [Header("Damage Settings")]
    [SerializeField] private float enemyDamage = 10f; // урон от врага
    [SerializeField] private float damageCooldown = 1f; // задержка между ударами

    [Header("UI")]
    [SerializeField] private Slider healthSlider;

    public float currentHealth;
    private bool isDead = false;

    private float lastDamageTime;

    // Движение
    private CharacterController m_CharacterController;
    private Vector3 m_MovementDirection;
    private Vector3 m_OriginalScale;
    private float m_CrouchTimeElapse;
    private Vector3 m_LandMomentum;
    private Vector3 m_OriginalLandMomentum;

    void Start()
    {
        currentHealth = maxHealth;

        m_CharacterController = GetComponent<CharacterController>();
        m_OriginalScale = transform.localScale;
        m_OriginalLandMomentum = Vector3.zero;
        m_CrouchTimeElapse = 0f;
        m_MovementDirection = Vector3.zero;

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " мертв.");

        // Анимация смерти
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
            enemyAI.OnDeath();
        else
        {
            EnemyAnimatorController animCtrl = GetComponent<EnemyAnimatorController>();
            if (animCtrl != null)
                animCtrl.PlayDeath();
        }

        // Обработчик смерти
        DeathHandler deathHandler = GetComponent<DeathHandler>();
        if (deathHandler != null)
        {
            deathHandler.HandleDeath();
        }
        else
        {
            Debug.LogError("На объекте " + gameObject.name + " отсутствует компонент DeathHandler!");
        }

        // Можно включить респавн через задержку
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        if (m_CharacterController == null)
            m_CharacterController = GetComponent<CharacterController>();

        if (m_CharacterController != null)
            m_CharacterController.enabled = false;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }

        Physics.SyncTransforms();

        currentHealth = maxHealth;
        isDead = false;
        transform.localScale = m_OriginalScale;
        m_CrouchTimeElapse = 0f;
        m_LandMomentum = m_OriginalLandMomentum;
        m_MovementDirection = Vector3.zero;

        if (m_CharacterController != null)
            m_CharacterController.enabled = true;

        UpdateHealthUI();
    }

    private void Update()
    {
        if (isDead) return;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    // 💥 Урон при столкновении с врагом
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                TakeDamage(enemyDamage);
                lastDamageTime = Time.time;
            }
        }
    }

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;
}