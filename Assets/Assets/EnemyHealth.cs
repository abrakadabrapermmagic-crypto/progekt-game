using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Параметры")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Событие смерти")]
    [Tooltip("Перетащите сюда скрипт, который нужно ВКЛЮЧИТЬ после смерти (например, Ragdoll или спавнер лута).")]
    public MonoBehaviour scriptToActivateOnDeath;

    [Tooltip("Уничтожить ли объект врага полностью? Если включаете рэгдолл, ставьте false.")]
    public bool destroyOnDeath = true;
    public float delayBeforeDestroy = 0f;

    void Start()
    {
        currentHealth = maxHealth;

        // На старте выключаем "посмертный" скрипт, если он назначен
        if (scriptToActivateOnDeath != null)
        {
            scriptToActivateOnDeath.enabled = false;
        }
    }

    // Этот метод будет вызывать Пуля
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " получил урон. HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " умер!");

        // 1. Запускаем другой скрипт (если он назначен)
        if (scriptToActivateOnDeath != null)
        {
            scriptToActivateOnDeath.enabled = true;
        }

        // 2. Отключаем этот скрипт здоровья и коллайдер, чтобы в труп нельзя было стрелять
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        this.enabled = false;

        // 3. Уничтожение объекта (опционально)
        if (destroyOnDeath)
        {
            Destroy(gameObject, delayBeforeDestroy);
        }
    }
}