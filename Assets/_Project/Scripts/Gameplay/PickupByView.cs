using UnityEngine;

public class PickupByView : MonoBehaviour
{
    [Header("Настройки камеры")]
    public Camera targetCamera;           // Камера, из которой пускаем луч

    [Header("Настройки подбора")]
    public Transform pickupSocket;        // Точка, куда будет помещаться предмет (например, рука)
    public float rayDistance = 5f;        // Дальность луча
    public LayerMask hitLayers = ~0;      // Слои, на которые реагирует луч
    public string pickupTag = "Pickable"; // Тег для подбираемых объектов

    private GameObject pickedObject;      // Текущий подобранный объект
    private Rigidbody pickedRigidbody;    // Rigidbody подобранного объекта

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (targetCamera == null || pickupSocket == null) return;

        // Луч из центра экрана
        Ray ray = targetCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, hitLayers))
        {
            // Проверяем, что объект подбираемый
            if (hit.collider.CompareTag(pickupTag))
            {
                // Если нажата клавиша подбора (например, E)
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickupObject(hit.collider.gameObject);
                }
            }
        }

        // Если предмет уже взят — перемещаем его к руке
        if (pickedObject != null)
        {
            pickedObject.transform.position = pickupSocket.position;
            pickedObject.transform.rotation = pickupSocket.rotation;
        }

        // Если нажата клавиша отпускания (например, Q)
        if (Input.GetKeyDown(KeyCode.Q) && pickedObject != null)
        {
            DropObject();
        }
    }

    void PickupObject(GameObject obj)
    {
        // Если уже что-то подобрано — отпускаем
        if (pickedObject != null)
        {
            DropObject();
        }

        pickedObject = obj;
        pickedRigidbody = obj.GetComponent<Rigidbody>();

        // Отключаем физику, чтобы объект не падал
        if (pickedRigidbody != null)
        {
            pickedRigidbody.isKinematic = true;
        }

        // Отключаем Outline, если нужно
        var outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    void DropObject()
    {
        if (pickedObject == null) return;

        // Включаем физику обратно
        if (pickedRigidbody != null)
        {
            pickedRigidbody.isKinematic = false;
        }

        pickedObject = null;
        pickedRigidbody = null;
    }
}
