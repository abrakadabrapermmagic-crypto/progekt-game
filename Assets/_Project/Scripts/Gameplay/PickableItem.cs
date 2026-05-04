using UnityEngine;

/// <summary>
/// Настройки предмета, который можно поднимать.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PickableItem : MonoBehaviour
{
    [Header("Точка захвата (опционально)")]
    public Transform grabPoint;

    [Header("Поведение при удержании")]
    public Vector3 holdPositionOffset = Vector3.zero;
    public Vector3 holdRotationOffsetEuler = Vector3.zero;
    public Vector3 holdScale = Vector3.zero;

    // Внутренние резервные значения
    Vector3 originalLocalScale;
    Quaternion originalWorldRotation;
    Collider[] allColliders; // Массив для хранения всех коллайдеров

    public bool UsesGrabPoint => grabPoint != null;

    void Awake()
    {
        originalLocalScale = transform.localScale;
        originalWorldRotation = transform.rotation;

        // Кэшируем все коллайдеры объекта и его "детей" один раз при старте
        allColliders = GetComponentsInChildren<Collider>();
    }

    public void OnPickedUp()
    {
        originalLocalScale = transform.localScale;
        originalWorldRotation = transform.rotation;

        if (holdScale != Vector3.zero)
            transform.localScale = holdScale;

        // ВЫКЛЮЧАЕМ все коллайдеры
        ToggleColliders(false);
    }

    public void OnDropped()
    {
        transform.localScale = originalLocalScale;
        transform.rotation = originalWorldRotation;

        // ВКЛЮЧАЕМ все коллайдеры обратно
        ToggleColliders(true);
    }

    /// <summary>
    /// Вспомогательный метод для переключения состояния коллайдеров
    /// </summary>
    private void ToggleColliders(bool state)
    {
        if (allColliders == null) return;

        foreach (var col in allColliders)
        {
            if (col != null)
                col.enabled = state;
        }
    }
}