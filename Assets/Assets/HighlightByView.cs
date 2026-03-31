using UnityEngine;

public class HighlightByView : MonoBehaviour
{
    [Header("Настройки камеры")]
    public Camera targetCamera;          // Камера, из которой пускаем луч

    [Header("Луч")]
    public float rayDistance = 5f;       // Дальность луча
    public LayerMask hitLayers = ~0;     // Какие слои учитываем

    // Текущий объект с Outline, на который смотрим
    private Outline currentOutline;

    void Start()
    {
        // Если камеру не задали в инспекторе — берём main
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (targetCamera == null) return;

        // Луч из центра экрана
        Ray ray = targetCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, hitLayers))
        {
            // Пытаемся получить Outline на объекте
            Outline outline = hit.collider.GetComponent<Outline>();

            if (outline != null)
            {
                // Если перешли на новый объект — отключаем прошлый
                if (currentOutline != null && currentOutline != outline)
                    currentOutline.enabled = false;

                currentOutline = outline;
                currentOutline.enabled = true;
            }
            else
            {
                // Попали в объект без Outline — выключаем прошлый
                if (currentOutline != null)
                {
                    currentOutline.enabled = false;
                    currentOutline = null;
                }
            }
        }
        else
        {
            // Луч ни во что не попал — выключаем прошлый
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
        }
    }
}
