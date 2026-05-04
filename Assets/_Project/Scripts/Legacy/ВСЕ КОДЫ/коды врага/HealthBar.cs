using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Slider healthSlider;

    private void Start()
    {
        // Настраиваем слайдер под текущее HP
        if (healthSystem != null && healthSlider != null)
        {
            // Учитывая твой код HealthSystem, где maxHealth = 100
            healthSlider.maxValue = 100;
            healthSlider.value = healthSystem.currentHealth;

            // Подписываемся на событие урона
            healthSystem.OnTakeDamage.AddListener(UpdateHealthBar);
        }
    }

    // LateUpdate вызывается после всех перемещений, идеален для камер и UI
    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            // Магия: заставляем полоску всегда иметь тот же поворот, что и камера.
            // Теперь, как бы ни крутился враг, полоска будет стоять ровно лицом к игроку.
            transform.rotation = Camera.main.transform.rotation;
        }
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthSystem != null && healthSlider != null)
        {
            healthSlider.value = healthSystem.currentHealth;
        }
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnTakeDamage.RemoveListener(UpdateHealthBar);
        }
    }
}