using UnityEngine;
using UnityEngine;
using System.Collections;

public class GravityControl : MonoBehaviour
{
    [Header("Настройки Цели и Длительности")]
    [Tooltip("Объект, чьей гравитацией нужно управлять.")]
    public GameObject targetObject;

    [Tooltip("Время в секундах, на которое гравитация будет отключена по умолчанию.")]
    public float DefaultDisableDuration = 3.0f; // <--- Эта переменная настраивается в Инспекторе

    // Приватные поля
    private Rigidbody targetRigidbody;
    private Coroutine gravityControlCoroutine;

    void Start()
    {
        // 1. Проверки и инициализация
        if (targetObject == null)
        {
            Debug.LogError("Целевой объект не назначен!", this);
            enabled = false;
            return;
        }

        targetRigidbody = targetObject.GetComponent<Rigidbody>();

        if (targetRigidbody == null)
        {
            Debug.LogError("Целевой объект не имеет компонента Rigidbody!", targetObject);
            enabled = false;
            return;
        }

        // Базовое состояние: Гравитация ВКЛЮЧЕНА
        targetRigidbody.useGravity = true;
    }

    // --- Методы Активации для других скриптов ---

    /// <summary>
    /// Активирует временное отключение гравитации, используя DefaultDisableDuration.
    /// Этот метод вызывается внешними скриптами.
    /// </summary>
    public void ActivateTemporaryDisable()
    {
        DisableGravityForTime(DefaultDisableDuration);
    }

    /// <summary>
    /// Публичный метод, который можно вызвать для запуска с заданным временем.
    /// (Оставлен для гибкости, если другой скрипт захочет передать свое время)
    /// </summary>
    /// <param name="duration">Время в секундах, на которое отключается гравитация.</param>
    public void DisableGravityForTime(float duration)
    {
        if (targetRigidbody == null) return;

        // Останавливаем предыдущий таймер, если он работал
        if (gravityControlCoroutine != null)
        {
            StopCoroutine(gravityControlCoroutine);
        }

        // Запускаем корутину
        gravityControlCoroutine = StartCoroutine(GravityTimerRoutine(duration));
    }

    // --- Логика Таймера (Корутина) ---

    private IEnumerator GravityTimerRoutine(float time)
    {
        // 1. Активация временного эффекта: Гравитация ВЫКЛЮЧЕНА
        targetRigidbody.useGravity = false;

        // 2. Ждем заданное время
        yield return new WaitForSeconds(time);

        // 3. Возврат к базовому состоянию: Гравитация ВКЛЮЧЕНА
        targetRigidbody.useGravity = true;

        // Сбрасываем ссылку на корутину
        gravityControlCoroutine = null;
    }
}