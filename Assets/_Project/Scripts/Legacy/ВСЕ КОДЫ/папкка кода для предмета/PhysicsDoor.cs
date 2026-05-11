using UnityEngine;

public class PhysicsDoor : MonoBehaviour
{
    [Header("Настройки силы")]
    public float openForce = 500f; // Сила толчка
    public float doorMass = 1f;    // Масса (синхронизируется с Rigidbody)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = doorMass;
        }
    }

    // Метод, который мы вызовем из скрипта игрока
    public void Interact(Vector3 pushDirection)
    {
        // Прикладываем силу в точку взаимодействия (центр двери)
        rb.AddForce(pushDirection * openForce);
    }
}