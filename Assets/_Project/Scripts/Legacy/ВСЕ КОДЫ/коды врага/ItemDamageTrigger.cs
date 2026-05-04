using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDamageTrigger : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float damageDuration = 0.5f; // Сколько секунд длится фаза урона

    [Header("Ignore Settings")]
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private string ignoreTag = "Player"; // Например, игнорировать игрока, который держит предмет

    private bool attackActive = false;
    private HashSet<int> hitTargets = new HashSet<int>();

    /// <summary>
    /// Включает урон на время, указанное в инспекторе.
    /// Вызывай этот метод из скрипта игрока при атаке или броске.
    /// </summary>
    public void ActivateDamage()
    {
        StopAllCoroutines(); // Сброс, если активация уже была
        StartCoroutine(DamageRoutine(damageDuration));
    }

    /// <summary>
    /// Перегрузка: позволяет задать время активности вручную.
    /// </summary>
    public void ActivateDamageCustom(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DamageRoutine(duration));
    }

    private IEnumerator DamageRoutine(float duration)
    {
        attackActive = true;
        hitTargets.Clear(); // Очищаем список пораженных целей для новой атаки

        yield return new WaitForSeconds(duration);

        attackActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Если предмет просто лежит (атака не активна) — ничего не делаем
        if (!attackActive) return;

        TryHit(other);
    }

    private void TryHit(Collider other)
    {
        // 1. Проверка на игнорирование (Слой или Тег)
        if (ShouldIgnore(other.gameObject)) return;

        // 2. Ищем компоненты здоровья (используем твою систему)
        var pHealth = other.GetComponentInParent<PlayerHealth>();
        var eHealth = other.GetComponentInParent<EnemyHealth>();

        if (pHealth == null && eHealth == null) return;

        // 3. Проверка на "смерть" цели
        bool isDead = (pHealth != null && pHealth.IsDead) || (eHealth != null && eHealth.IsDead);
        if (isDead) return;

        // 4. Идентификация цели, чтобы не бить дважды за один замах
        int targetId = other.transform.root.gameObject.GetInstanceID();
        if (!hitTargets.Add(targetId)) return;

        // 5. Нанесение урона
        if (pHealth != null) pHealth.TakeDamage(damage);
        else if (eHealth != null) eHealth.TakeDamage(damage);
        
        Debug.Log($"Нанесен урон объекту: {other.transform.root.name}");
    }

    private bool ShouldIgnore(GameObject obj)
    {
        // Битовая проверка слоя
        if (((1 << obj.layer) & ignoreMask) != 0) return true;
        
        // Проверка тега
        if (!string.IsNullOrEmpty(ignoreTag) && obj.CompareTag(ignoreTag)) return true;
        
        return false;
    }
}
