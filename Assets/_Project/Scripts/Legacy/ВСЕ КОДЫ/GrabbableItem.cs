using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GrabbableItem : MonoBehaviour
{
    [Header("--- СКРИПТЫ ПРИ ПОДБОРЕ (GRAB) ---")]
    [Tooltip("Скрипты на этом объекте, которые нужно ВРЕМЕННО включить при подборе.")]
    [SerializeField] private MonoBehaviour[] scriptsToEnableOnGrab;

    // ⭐ НОВОЕ: Таймер для скриптов при поднятии
    [Tooltip("Как долго (в секундах) скрипты 'OnGrab' должны работать. 0 = работать 1 кадр.")]
    [SerializeField] private float grabScriptDuration = 1.0f;


    [Header("--- СКРИПТЫ ПРИ ВЫБРАСЫВАНИИ (DROP) ---")]
    [Tooltip("Скрипты на этом объекте, которые нужно ВРЕМЕННО включить при броске.")]
    [SerializeField] private MonoBehaviour[] scriptsToEnableOnDrop;

    [Tooltip("Как долго (в секундах) скрипты 'OnDrop' должны работать. 0 = работать 1 кадр.")]
    [SerializeField] private float dropScriptDuration = 1.0f;

    // Приватные ссылки
    private Rigidbody rb;
    private Collider col;
    private Coroutine grabCoroutine; // Хранит ссылку на запущенный таймер
    private Coroutine dropCoroutine; // Хранит ссылку на запущенный таймер

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Выключаем все целевые скрипты при старте
        SetScriptsEnabled(scriptsToEnableOnGrab, false);
        SetScriptsEnabled(scriptsToEnableOnDrop, false);
    }

    /// <summary>
    /// (Оптимизация) Безопасно включает или выключает список скриптов.
    /// </summary>
    private void SetScriptsEnabled(MonoBehaviour[] scripts, bool enabledState)
    {
        foreach (var script in scripts)
        {
            if (script != null)
                script.enabled = enabledState;
        }
    }

    /// <summary>
    /// Вызывается скриптом PlayerInteraction, когда игрок подбирает предмет
    /// </summary>
    public void OnGrab(Transform holdParent)
    {
        // Останавливаем любые предыдущие таймеры (например, таймер броска)
        if (dropCoroutine != null)
            StopCoroutine(dropCoroutine);

        // Физика и прикрепление
        rb.isKinematic = true;
        rb.useGravity = false;
        col.enabled = false;

        transform.SetParent(holdParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Запускаем таймер для скриптов "в руках"
        if (scriptsToEnableOnGrab.Length > 0)
        {
            grabCoroutine = StartCoroutine(RunScriptsWithTimer(scriptsToEnableOnGrab, grabScriptDuration));
        }

        // Убедимся, что скрипты броска выключены
        SetScriptsEnabled(scriptsToEnableOnDrop, false);
    }

    /// <summary>
    /// Вызывается скриптом PlayerInteraction, когда игрок бросает предмет
    /// </summary>
    public void OnDrop()
    {
        // Останавливаем таймер "в руках", если он еще идет
        if (grabCoroutine != null)
            StopCoroutine(grabCoroutine);

        // Физика и открепление
        transform.SetParent(null);
        col.enabled = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        // Убедимся, что скрипты "в руках" выключены
        SetScriptsEnabled(scriptsToEnableOnGrab, false);

        // Запускаем таймер для скриптов "броска"
        if (scriptsToEnableOnDrop.Length > 0)
        {
            dropCoroutine = StartCoroutine(RunScriptsWithTimer(scriptsToEnableOnDrop, dropScriptDuration));
        }
    }

    /// <summary>
    /// (Оптимизация) Общая корутина (таймер) для запуска скриптов.
    /// </summary>
    private IEnumerator RunScriptsWithTimer(MonoBehaviour[] scripts, float duration)
    {
        // 1. Включаем скрипты
        SetScriptsEnabled(scripts, true);

        // 2. Ждем
        if (duration <= 0)
        {
            yield return null; // Ждем 1 кадр
        }
        else
        {
            yield return new WaitForSeconds(duration); // Ждем N секунд
        }

        // 3. Выключаем скрипты
        SetScriptsEnabled(scripts, false);
    }
}