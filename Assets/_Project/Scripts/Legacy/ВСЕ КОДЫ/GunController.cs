using UnityEngine;

public class GunController : MonoBehaviour
{
    // 1. Переменная для префаба пули
    // Сюда мы перетащим наш префаб Bullet из окна Project
    public GameObject bulletPrefab;

    // 2. Точка, откуда будут вылетать пули
    // Сюда мы перетащим наш объект FirePoint
    public Transform firePoint;

    // 3. Сила (скорость) выстрела
    public float fireForce = 20f;

    // Update вызывается каждый кадр
    void Update()
    {
        // 4. Проверяем, нажата ли кнопка "Fire1" (по умолчанию - Левая кнопка мыши)
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    // Метод, отвечающий за выстрел
    void Shoot()
    {
        // 5. Создаем (Instantiate) копию префаба пули
        //    в позиции firePoint и с вращением firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 6. Получаем компонент Rigidbody у *только что созданной* пули
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // 7. Добавляем этой пуле силу (импульс)
        //    firePoint.forward - это направление "вперед" по синей оси (Z)
        //    ForceMode.Impulse - придает мгновенный импульс, как при выстреле
        rb.AddForce(firePoint.forward * fireForce, ForceMode.Impulse);

        // (Опционально) Уничтожаем пулю через 5 секунд,
        // чтобы она не летела вечно и не засоряла сцену
        Destroy(bullet, 5f);
    }
}
