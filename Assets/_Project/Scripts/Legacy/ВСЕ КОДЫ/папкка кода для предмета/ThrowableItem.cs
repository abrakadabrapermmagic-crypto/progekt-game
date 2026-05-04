using UnityEngine;

public class ThrowableItem : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 10;              // урон при попадании
    [SerializeField] private float destroyDelay = 0.1f;    // маленькая задержка перед уничтожением

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb)
            Debug.LogError("На объекте " + name + " не найден Rigidbody!", this);
    }

    /// Сделать объект бросаемым (вызывать из.Character)
    public void Throw(Vector3 forceDirection, float throwForce)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(forceDirection * throwForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем тег "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Пытаемся найти HealthSystem, но бьём только если это敌
            if (collision.gameObject.TryGetComponent<HealthSystem>(out HealthSystem health))
            {
                health.TakeDamage(damage);
            }
        }

        Destroy(gameObject, destroyDelay);
    }
}