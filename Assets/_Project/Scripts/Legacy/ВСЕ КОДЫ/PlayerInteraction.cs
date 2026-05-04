using UnityEngine;

// Поместите этот скрипт на ваш основной объект игрока
// (тот, на котором находится Character Controller или Rigidbody)
public class PlayerInteraction : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Камера игрока (обычно Main Camera внутри объекта игрока)")]
    [SerializeField] private Camera playerCamera;

    [Tooltip("Пустой объект, дочерний камере, в позиции 'рук'")]
    [SerializeField] private Transform holdPosition;

    [Header("Настройки")]
    [Tooltip("Как далеко игрок может 'дотянуться' до предмета")]
    [SerializeField] private float reachDistance = 10f;

    [Tooltip("Слой, на котором находятся все подбираемые предметы")]
    [SerializeField] private LayerMask grabbableLayer;

    // Приватная переменная для хранения предмета, который мы держим
    private GrabbableItem heldItem;

    void Update()
    {
        // Проверяем нажатие клавиши 'E'
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E");
            Debug.Log("E");
            // Если мы уже держим предмет - бросаем его
            if (heldItem != null)
            {
                Drop();
            }
            // Если мы не держим предмет - пытаемся подобрать
            else
            {
                TryGrab();
            }
        }
    }

    private void TryGrab()
    {
        Debug.Log("Луч запущен");
        // Пускаем луч из центра камеры вперед
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, grabbableLayer))
        {

            Debug.Log(hit.collider.gameObject.name);

            // Проверяем, есть ли на объекте, в который мы попали, скрипт GrabbableItem
            if (hit.collider.TryGetComponent<GrabbableItem>(out GrabbableItem item))
            {
                // Если есть - подбираем его
                Grab(item);
            }
        }
        else
        {
            Debug.Log(hit.collider.gameObject.name);
            Debug.Log("Непопал");

        }
    }

    private void Grab(GrabbableItem item)
    {
        heldItem = item;
        // Вызываем метод "OnGrab" на самом предмете, передавая ему точку "рук"
        item.OnGrab(holdPosition);
    }

    private void Drop()
    {
        // Вызываем метод "OnDrop" на предмете
        heldItem.OnDrop();
        // Забываем, что мы что-то держали
        heldItem = null;
    }


}
