using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyWeaponTrigger : MonoBehaviour
{
    private Collider weaponCollider;
    private readonly HashSet<int> hitTargetIds = new HashSet<int>();

    private bool attackActive;
    private float damage;

    private void Awake()
    {
        weaponCollider = GetComponent<Collider>();

        if (weaponCollider == null)
            weaponCollider = GetComponentInChildren<Collider>(true);

        if (weaponCollider == null)
        {
            Debug.LogError($"[{name}] Collider не найден ни на объекте, ни в дочерних!");
            return;
        }

        weaponCollider.isTrigger = true;
        weaponCollider.enabled = false;

        Debug.Log($"[{name}] EnemyWeaponTrigger Awake. Collider найден: {weaponCollider.name}");
    }

    public void BeginAttack(float damageAmount)
    {
        damage = damageAmount;
        attackActive = true;
        hitTargetIds.Clear();

        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
            Debug.Log($"[{name}] BeginAttack -> Collider ENABLED, damage = {damage}");
        }
        else
        {
            Debug.LogError($"[{name}] BeginAttack -> weaponCollider == null");
        }
    }

    public void EndAttack()
    {
        attackActive = false;
        hitTargetIds.Clear();

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
            Debug.Log($"[{name}] EndAttack -> Collider DISABLED");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[{name}] OnTriggerEnter with: {other.name}");
        TryHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHit(other);
    }

    private void TryHit(Collider other)
    {
        if (!attackActive) return;

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player == null)
        {
            Debug.Log($"[{name}] Trigger есть, но PlayerHealth не найден у {other.name}");
            return;
        }

        if (player.IsDead) return;

        int targetId = player.transform.root.gameObject.GetInstanceID();

        if (!hitTargetIds.Add(targetId))
            return;

        bool damaged = player.TakeDamage(damage);
        Debug.Log($"[{name}] ”дар по {player.name}, урон={damage}, успешный={damaged}");
    }
}