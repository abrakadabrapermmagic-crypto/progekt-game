using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform firePoint;      // Точка, откуда вылетает пуля
    public GameObject bulletPrefab;  // Префаб пули

    [Header("Параметры")]
    public float bulletForce = 20f;  // Сила выстрела

    void Update()
    {
        // Стрельба по нажатию ЛКМ (Fire1)
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // 1. Создаем пулю в точке firePoint с поворотом как у firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 2. Получаем Rigidbody пули для физического воздействия
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        // Внутри метода Shoot() в скрипте Weapon:
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = 50; // Устанавливаем урон для конкретного выстрела
        }

        // 3. Добавляем импульс вперед
        // ForceMode.Impulse идеально подходит для мгновенных толчков (выстрелов)
        rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);

    }
}