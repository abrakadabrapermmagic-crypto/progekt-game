using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 20; // Урон от пули
    public float lifeTime = 3f; // Время жизни пули, чтобы не засорять сцену

    void Start()
    {
        // Удалить пулю через 3 секунды, если она ни во что не попала
        Destroy(gameObject, lifeTime);
    }

    // Этот метод вызывается автоматически при столкновении (если есть Collider и Rigidbody)
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Ищем компонент HealthSystem на объекте, в который попали
        HealthSystem targetHealth = collision.gameObject.GetComponent<HealthSystem>();

        // 2. Если компонент найден — наносим урон
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        // 3. Уничтожаем пулю после попадания
        Destroy(gameObject);
    }
}