using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponTrigger : MonoBehaviour
{
    [Header("Hit Detection")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private LayerMask hitMask = ~0;
    [SerializeField] private bool useTriggerColliderToo = true;

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
        if (!attackActive)
            return;

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

    private void OnTriggerEnter(Collider other)
    {
        if (!useTriggerColliderToo)
            return;

        TryHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!useTriggerColliderToo)
            return;

        TryHit(other);
    }

    private void TryHit(Collider other)
    {
        if (!attackActive || other == null)
            return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null || playerHealth.IsDead)
            return;

        int targetId = playerHealth.transform.root.gameObject.GetInstanceID();
        if (!hitTargets.Add(targetId))
            return;

        playerHealth.TakeDamage(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = attackPoint != null ? attackPoint : transform;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(point.position, attackRadius);
    }
}
