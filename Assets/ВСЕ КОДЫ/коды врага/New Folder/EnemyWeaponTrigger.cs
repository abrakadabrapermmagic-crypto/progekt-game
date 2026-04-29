using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponTrigger : MonoBehaviour
{
    [Header("Hit Detection")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private LayerMask hitMask = ~0;
    [SerializeField] private bool useTriggerColliderToo = true;

    [Header("Ignore Settings")]
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private string ignoreTag = "";

    private readonly HashSet<int> hitTargets = new HashSet<int>();
    private bool attackActive;
    private float damage;

    private void Awake()
    {
        if (attackPoint == null)
            attackPoint = transform;
    }

    private void Update()
    {
        if (!attackActive) return;
        CheckHits();
    }

    public void BeginAttack(float damageAmount)
    {
        damage = damageAmount;
        attackActive = true;
        hitTargets.Clear();
        CheckHits();
    }

    public void EndAttack()
    {
        attackActive = false;
        hitTargets.Clear();
    }

    private void CheckHits()
    {
        Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
        Collider[] hits = Physics.OverlapSphere(center, attackRadius, hitMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < hits.Length; i++)
            TryHit(hits[i]);
    }

    private void OnTriggerEnter(Collider other) => HandleTrigger(other);
    private void OnTriggerStay(Collider other) => HandleTrigger(other);

    private void HandleTrigger(Collider other)
    {
        if (useTriggerColliderToo) TryHit(other);
    }

    private void TryHit(Collider other)
    {
        if (!attackActive || other == null) return;

        // 1. Проверка на игнорирование
        if (ShouldIgnore(other.gameObject)) return;

        // 2. Поиск здоровья (Игрок или Враг)
        PlayerHealth pHealth = other.GetComponentInParent<PlayerHealth>();
        EnemyHealth eHealth = other.GetComponentInParent<EnemyHealth>();

        // Проверяем, нашли ли мы хоть какой-то компонент здоровья
        if (pHealth == null && eHealth == null) return;

        // 3. Проверка на "смерть" цели
        bool isDead = (pHealth != null && pHealth.IsDead) || (eHealth != null && eHealth.IsDead);
        if (isDead) return;

        // 4. Проверка на дубликаты попаданий (по корневому объекту)
        int targetId = other.transform.root.gameObject.GetInstanceID();
        if (!hitTargets.Add(targetId)) return;

        // 5. Нанесение урона конкретному типу
        if (pHealth != null)
        {
            pHealth.TakeDamage(damage);
        }
        else if (eHealth != null && other.CompareTag("Enemy"))
        {
            eHealth.TakeDamage(damage);
        }
    }

    private bool ShouldIgnore(GameObject obj)
    {
        if (((1 << obj.layer) & ignoreMask) != 0) return true;
        if (!string.IsNullOrEmpty(ignoreTag) && obj.CompareTag(ignoreTag)) return true;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = attackPoint != null ? attackPoint : transform;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(point.position, attackRadius);
    }
}