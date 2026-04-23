using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private HealthSystem healthSystem; // Скрипт здоровья врага
    [SerializeField] private Slider healthSlider;       // Ссылка на Slider над головой

    [Header("Настройки")]
    [SerializeField] private bool hideOnFullHealth = true; // Скрывать, если HP полное
    [SerializeField] private float smoothSpeed = 5f;       // Плавность полоски

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        // Автоматический поиск скрипта здоровья на враге
        if (healthSystem == null)
            healthSystem = GetComponentInParent<HealthSystem>();
    }

    private void Start()
    {
        if (healthSlider != null && healthSystem != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = 100; // Соответствует maxHealth в твоем HealthSystem
            healthSlider.value = healthSystem.currentHealth;
        }
    }

    private void Update()
    {
        if (healthSystem == null || healthSlider == null) return;

        // Плавное обновление значения Slider (как в твоем PlayerHealth)
        healthSlider.value = Mathf.Lerp(healthSlider.value, healthSystem.currentHealth, Time.deltaTime * smoothSpeed);

        // Поворот полоски к игроку (Billboard)
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }

        // Логика отображения
        if (canvas != null)
        {
            // Скрываем, если враг мертв или здоровье полное (если включено)
            bool shouldShow = healthSystem.IsAlive && (!hideOnFullHealth || healthSystem.currentHealth < 100);
            canvas.enabled = shouldShow;
        }
    }
}