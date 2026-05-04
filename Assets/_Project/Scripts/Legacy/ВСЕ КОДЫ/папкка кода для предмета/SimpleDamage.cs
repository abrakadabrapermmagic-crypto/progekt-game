using UnityEngine;

public class SimpleDamage : MonoBehaviour
{
    [Header("Настройки")]
    public float damage = 20f;
    public bool isAttackActive = false; // Включай через анимацию или код
    public LayerMask hitLayers;        // Выбери в инспекторе, кого бить

    private void OnTriggerEnter(Collider other)
    {
        // Если атака не включена — выходим
        if (!isAttackActive) return;

        // Проверяем, входит ли объект в разрешенные слои
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            // Пытаемся найти здоровье (игрока или врага)
            var pHealth = other.GetComponentInParent<PlayerHealth>();
            var eHealth = other.GetComponentInParent<EnemyHealth>();

            if (pHealth != null) pHealth.TakeDamage(damage);
            if (eHealth != null) eHealth.TakeDamage(damage);

            Debug.Log("Урон нанесен: " + other.name);
        }
    }
}