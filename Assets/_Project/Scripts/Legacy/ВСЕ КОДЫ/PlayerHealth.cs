using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float hitInvulnerabilityTime = 0.15f;
    [SerializeField] private Slider healthSlider;

    [Header("Respawn")]
    [SerializeField] private bool respawnOnDeath = false;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Optional")]
    [SerializeField] private Rigidbody rb;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float nextDamageTime;

    public float CurrentHealth;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        startPosition = transform.position;
        startRotation = transform.rotation;
        CurrentHealth = maxHealth;
        RefreshUI();
    }
    
    public bool TakeDamage(float amount)
    {
        if (IsDead)
            return false;

        if (Time.time < nextDamageTime)
            return false;

        nextDamageTime = Time.time + hitInvulnerabilityTime;
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0f, maxHealth);
        RefreshUI();

        if (CurrentHealth <= 0f)
            Die();

        return true;
    }

    public void Heal(float amount)
    {
        if (IsDead)
            return;

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, maxHealth);
        RefreshUI();
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (respawnOnDeath)
            Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        Vector3 newPosition = startPosition;
        Quaternion newRotation = startRotation;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (spawn != null)
            {
                newPosition = spawn.position;
                newRotation = spawn.rotation;
            }
        }

        if (rb != null)
        {
            rb.position = newPosition;
            rb.rotation = newRotation;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            transform.position = newPosition;
            transform.rotation = newRotation;
        }

        Physics.SyncTransforms();

        CurrentHealth = maxHealth;
        IsDead = false;
        nextDamageTime = 0f;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (healthSlider == null)
            return;

        healthSlider.minValue = 0f;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = CurrentHealth;
    }
}
