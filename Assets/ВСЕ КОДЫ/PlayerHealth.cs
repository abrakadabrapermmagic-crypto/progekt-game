using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Damage Protection")]
    [SerializeField] private float hitInvulnerabilityTime = 0.15f;

    [Header("Optional Components")]
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private Rigidbody rb;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float nextDamageTime;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        if (capsuleCollider == null)
            capsuleCollider = GetComponent<CapsuleCollider>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        CurrentHealth = maxHealth;
        UpdateHealthUI();
    }

    public bool TakeDamage(float damage)
    {
        if (IsDead) return false;
        if (Time.time < nextDamageTime) return false;

        nextDamageTime = Time.time + hitInvulnerabilityTime;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);
        UpdateHealthUI();

        if (CurrentHealth <= 0f)
            Die();

        return true;
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);
        UpdateHealthUI();
    }

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;
        Debug.Log($"{gameObject.name} умер.");

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        Vector3 respawnPosition = startPosition;
        Quaternion respawnRotation = startRotation;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            respawnPosition = spawnPoint.position;
            respawnRotation = spawnPoint.rotation;
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = respawnPosition;
            rb.rotation = respawnRotation;
        }
        else
        {
            transform.position = respawnPosition;
            transform.rotation = respawnRotation;
        }

        Physics.SyncTransforms();

        CurrentHealth = maxHealth;
        IsDead = false;
        nextDamageTime = 0f;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthSlider == null) return;

        healthSlider.minValue = 0f;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = CurrentHealth;
    }
}