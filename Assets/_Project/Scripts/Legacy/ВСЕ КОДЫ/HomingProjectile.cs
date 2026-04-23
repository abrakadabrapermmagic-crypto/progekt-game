using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    // --- Настройки Самонаведения ---
    [Tooltip("Сила притяжения к цели.")]
    [SerializeField] private float homingForce = 15f;
    [Tooltip("Радиус поиска цели.")]
    [SerializeField] private float searchRadius = 10f;
    [Tooltip("Частота поиска новой цели (в секундах).")]
    [SerializeField] private float searchInterval = 0.5f;

    // --- Приватные Переменные ---
    private Rigidbody rb;
    private Transform target;
    private bool isThrown = false;
    private float nextSearchTime;

    private const string ENEMY_TAG = "Enemy";

    void Awake()
    {
        // Получаем Rigidbody один раз
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("HomingProjectile требует Rigidbody.");
            enabled = false;
        }
    }

    /// <summary>
    /// Активирует самонаведение. Вызывается из PlayerInteraction.
    /// </summary>
    public void ActivateHoming()
    {
        isThrown = true;
        nextSearchTime = Time.time; // Начинаем поиск сразу
    }

    void FixedUpdate()
    {
        // Физические расчеты должны быть в FixedUpdate
        if (isThrown)
        {
            // Поиск цели по таймеру
            if (Time.time >= nextSearchTime)
            {
                FindClosestTarget();
                nextSearchTime = Time.time + searchInterval;
            }

            // Применение силы притяжения, если цель найдена
            if (target != null)
            {
                ApplyHomingForce();
            }
        }
    }

    void FindClosestTarget()
    {
        // Поиск ближайшего противника в радиусе
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);
        Transform closestTarget = null;
        float closestDistanceSq = Mathf.Infinity;

        foreach (var hit in hitColliders)
        {
            // Проверяем тег и не является ли это сам предмет
            if (hit.CompareTag(ENEMY_TAG) && hit.transform != transform)
            {
                float distanceSq = (hit.transform.position - transform.position).sqrMagnitude;

                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closestTarget = hit.transform;
                }
            }
        }

        target = closestTarget;
    }

    void ApplyHomingForce()
    {
        // Вектор направления от текущей позиции к цели
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // Применяем силу, чтобы "тянуть" снаряд к цели (режим Acceleration игнорирует массу)
        rb.AddForce(directionToTarget * homingForce, ForceMode.Acceleration);
    }
}
