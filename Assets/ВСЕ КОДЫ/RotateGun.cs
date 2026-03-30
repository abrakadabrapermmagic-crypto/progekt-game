using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    public float pickupRange = 3f;               // Дальность, на которой можно поднять предмет
    public Camera playerCamera;                   // Ссылка на камеру игрока
    public Button pickupButton;                   // Ссылка на кнопку Canvas для взятия

    private GameObject currentItem;               // Текущий доступный для взятия предмет

    void Start()
    {
        // Подписываем кнопку на метод PickupItem
        if (pickupButton != null)
            pickupButton.onClick.AddListener(PickupItem);
    }

    void Update()
    {
        // Проверяем, есть ли предмет в зоне действия (перед игроком)
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                currentItem = hit.collider.gameObject;

                // Если нажата клавиша E, то взять предмет
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickupItem();
                }
                return;
            }
        }
        currentItem = null;
    }

    public void PickupItem()
    {
        if (currentItem != null)
        {
            // Здесь логика для взятия предмета, например деактивация объекта
            Debug.Log("Предмет взят: " + currentItem.name);
            currentItem.SetActive(false);
            currentItem = null;
        }
    }
}
