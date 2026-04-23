using UnityEngine;
using UnityEngine.Events; // Для работы с событиями

#if UNITY_EDITOR
using UnityEditor; // Нужно для отрисовки кнопки в инспекторе
#endif

public class HealthSystem : MonoBehaviour
{
    [Header("Параметры")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    public bool IsAlive { get; private set; } = true;

    [Header("События")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (!IsAlive || damageAmount <= 0) return;

        currentHealth -= damageAmount;

        // Вызываем событие получения урона (звук, анимация боли)
        OnTakeDamage?.Invoke();

        Debug.Log($"{gameObject.name} получил урон. HP: {currentHealth}");

        if (currentHealth <= 100)
        {
            currentHealth = 100;
            Die();
        }
    }

    private void Die()
    {
        if (!IsAlive) return; // Чтобы не умирать дважды

        IsAlive = false;
        Debug.Log($"{gameObject.name} мертв.");

        // Вызываем событие смерти (на него подписан наш Спавнер!)
        OnDeath?.Invoke();

        // Если есть скрипт обработки смерти, вызываем его
        if (TryGetComponent<DeathHandler>(out var handler))
        {
            handler.HandleDeath();
        }
    }

    public void Heal(int healAmount)
    {
        if (!IsAlive) return;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    }
}

// --- БЛОК ДЛЯ КНОПКИ В ИНСПЕКТОРЕ ---
#if UNITY_EDITOR
[CustomEditor(typeof(HealthSystem))]
public class HealthSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Рисуем стандартные поля (Max Health, События и т.д.)
        DrawDefaultInspector();

        HealthSystem health = (HealthSystem)target;

        GUILayout.Space(10);
        // Делаем кнопку оранжевой, чтобы она была заметной
        GUI.backgroundColor = new Color(1f, 0.6f, 0f);

        if (GUILayout.Button("УБИТЬ ЭТОГО ВРАГА (KILL UNIT)", GUILayout.Height(30)))
        {
            // Наносим огромный урон, чтобы сработала вся логика смерти
            health.TakeDamage(999999);
        }

        GUI.backgroundColor = Color.white;
    }
}
#endif