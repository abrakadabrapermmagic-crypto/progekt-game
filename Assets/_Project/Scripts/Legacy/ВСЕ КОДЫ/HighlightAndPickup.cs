using UnityEngine;

public class HighlightAndPickup : MonoBehaviour
{
    [Header("Камера")]
    public Camera targetCamera;

    [Header("Луч")]
    public float rayDistance = 5f;
    public LayerMask hitLayers = ~0;

    [Header("Руки / точка удержания")]
    public Transform handPoint;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;

    private Outline currentOutline;
    private Transform heldObject;
    private Rigidbody heldRb;

    public float ipulse;

    // Ссылка на компонент настроек текущего предмета
    private PickableItem currentPickableItem;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (targetCamera == null) return;

        HandleLookHighlight();
        HandlePickupDrop();

        // Обновляем позицию/поворот предмета каждый кадр, но только если у предмета нет grabPoint
        UpdateHeldItemPosition();
    }

    void UpdateHeldItemPosition()
    {
        if (heldObject == null || currentPickableItem == null) return;

        // Если предмет использует grabPoint — мы уже его выровняли при взятии и не перезаписываем каждый кадр.
        if (currentPickableItem.UsesGrabPoint) return;

        // Иначе применяем локальные оффсеты (удобно для мелких предметов)
        heldObject.localPosition = currentPickableItem.holdPositionOffset;
        heldObject.localRotation = Quaternion.Euler(currentPickableItem.holdRotationOffsetEuler);
    }

    void HandleLookHighlight()
    {
        if (heldObject != null)
        {
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
            return;
        }

        Ray ray = targetCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, hitLayers))
        {
            Outline outline = hit.collider.GetComponent<Outline>();

            if (outline == null)
            {
                outline = hit.collider.gameObject.AddComponent<Outline>();
            }

            if (outline != null)
            {
                if (currentOutline != null && currentOutline != outline)
                    currentOutline.enabled = false;

                currentOutline = outline;
                currentOutline.enabled = true;
            }
            else
            {
                if (currentOutline != null)
                {
                    currentOutline.enabled = false;
                    currentOutline = null;
                }
            }
        }
        else
        {
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
        }
    }

    void HandlePickupDrop()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
        }

        if (Input.GetKeyDown(dropKey))
        {
            if (heldObject != null)
            {
                DropObject();
            }
        }
    }

    void TryPickupObject()
    {
        if (currentOutline == null) return;

        Transform obj = currentOutline.transform;
        heldObject = obj;

        // 1. Физика
        heldRb = heldObject.GetComponent<Rigidbody>();
        if (heldRb == null)
        {
            heldRb = heldObject.gameObject.AddComponent<Rigidbody>();
        }

        heldRb.isKinematic = true;
        heldRb.useGravity = false;

        // 2. Иерархия (берем в руку) — используем SetParent( handPoint ) (worldPositionStays = true по умолчанию)
        heldObject.SetParent(handPoint);

        // 3. Проверяем, есть ли на предмете особые настройки (PickableItem)
        currentPickableItem = heldObject.GetComponent<PickableItem>();

        if (currentPickableItem != null)
        {
            // Если указан grabPoint — выравниваем точку захвата и ориентацию
            if (currentPickableItem.UsesGrabPoint)
            {
                // 1) Поворачиваем объект так, чтобы grabPoint.rotation совпал с handPoint.rotation
                Quaternion deltaRot = handPoint.rotation * Quaternion.Inverse(currentPickableItem.grabPoint.rotation);
                heldObject.rotation = deltaRot * heldObject.rotation;

                // 2) После поворота перемещаем объект так, чтобы grabPoint.position == handPoint.position
                Vector3 deltaPos = handPoint.position - currentPickableItem.grabPoint.position;
                heldObject.position += deltaPos;

                // 3) Можно применить дополнительный локальный оффсет и/или дополнительный локальный поворот
                if (currentPickableItem.holdPositionOffset != Vector3.zero)
                {
                    heldObject.localPosition += currentPickableItem.holdPositionOffset;
                }

                if (currentPickableItem.holdRotationOffsetEuler != Vector3.zero)
                {
                    // Применяем дополнительный локальный поворот
                    heldObject.localRotation *= Quaternion.Euler(currentPickableItem.holdRotationOffsetEuler);
                }
            }
            else
            {
                // fallback: используем заранее указанные локальные смещения
                heldObject.localPosition = currentPickableItem.holdPositionOffset;
                heldObject.localRotation = Quaternion.Euler(currentPickableItem.holdRotationOffsetEuler);
            }

            // Применяем остальные изменения (например масштаб) через сам PickableItem
            currentPickableItem.OnPickedUp();
        }
        else
        {
            // Если настроек нет, просто обнуляем (стандартное поведение)
            heldObject.localPosition = Vector3.zero;
            heldObject.localRotation = Quaternion.identity;
        }

        // Выключаем подсветку
        currentOutline.enabled = false;
        currentOutline = null;
    }

    void DropObject()
    {
        // 1. Если был компонент настроек, выключаем скрипты/восстанавливаем параметры
        if (currentPickableItem != null)
        {
            currentPickableItem.OnDropped();
            currentPickableItem = null;
        }

        // 2. Отцепляем от руки
        heldObject.SetParent(null);

        // 3. Возвращаем физику
        if (heldRb != null)
        {
            heldRb.isKinematic = false;
            heldRb.useGravity = true;

            // Можно добавить небольшой толчок при броске
            heldRb.AddForce(targetCamera.transform.forward * ipulse, ForceMode.Impulse);

            heldRb = null;
        }

        heldObject = null;
    }
}
