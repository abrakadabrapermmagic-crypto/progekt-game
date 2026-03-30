using UnityEngine;

// Убеждаемся, что на объекте есть Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class GravityModifier : MonoBehaviour
{
    [Header("Настройки Гравитации")]
    [Tooltip("Множитель стандартной силы гравитации. 1.0 = нормальная гравитация сцены, 0.5 = половина гравитации.")]
    [SerializeField] private float gravityScale = 1.0f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // ⭐ ВАЖНО: Отключаем стандартную гравитацию Unity.
        // Теперь гравитация будет применяться только через наш скрипт.
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        // FixedUpdate лучше всего подходит для работы с физикой

        // Применяем гравитацию только тогда, когда предмет не находится в руках игрока.
        // Когда предмет подобран, GrabbableItem.cs устанавливает rb.isKinematic = true.
        if (!rb.isKinematic)
        {
            // Получаем стандартную вектор гравитации сцены (Physics.gravity)
            // и умножаем его на наш множитель (gravityScale)
            Vector3 customGravity = Physics.gravity * gravityScale;

            // Применяем силу к Rigidbody.
            // ForceMode.Acceleration игнорирует массу объекта.
            rb.AddForce(customGravity, ForceMode.Acceleration);
        }
    }

    // Публичный метод, который позволяет другим скриптам менять гравитацию на лету
    public void SetGravityScale(float newScale)
    {
        gravityScale = newScale;
    }
}