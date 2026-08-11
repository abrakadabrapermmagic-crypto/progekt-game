using System;
using UnityEngine;

/// <summary>
/// Автоматически находит бытовые предметы в сцене и делает их подбираемыми/бросаемыми:
/// - снимает static-флаг,
/// - добавляет подходящий Collider,
/// - добавляет Rigidbody,
/// - добавляет PickableItem.
/// </summary>
public class AutoSetupThrowableItems : MonoBehaviour
{
    [Header("Ключевые слова для бытовых предметов")]
    [SerializeField] private string[] itemNameKeywords =
    {
        "book", "books", "chair", "sofa", "couch", "pillow", "pot", "plant", "vase", "cup", "bottle",
        "книга", "книги", "стул", "диван", "подушка", "горшок", "цветок", "ваза", "чашка", "бутылка"
    };

    [Header("Физика")]
    [SerializeField] private float defaultMass = 1.5f;
    [SerializeField] private bool setPickableTag = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        GameObject setupObject = new GameObject("AutoSetupThrowableItems");
        var setup = setupObject.AddComponent<AutoSetupThrowableItems>();
        setup.SetupItems();
        Destroy(setupObject);
    }

    [ContextMenu("Setup Throwable Items In Scene")]
    public void SetupItems()
    {
        var renderers = FindObjectsByType<MeshRenderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var meshRenderer in renderers)
        {
            if (meshRenderer == null) continue;

            GameObject go = meshRenderer.gameObject;
            string objectName = go.name.ToLowerInvariant();

            if (!MatchesKeyword(objectName))
                continue;

            if (go.GetComponentInParent<Camera>() != null)
                continue;

            PrepareObject(go);
        }
    }

    private bool MatchesKeyword(string objectName)
    {
        foreach (var keyword in itemNameKeywords)
        {
            if (!string.IsNullOrWhiteSpace(keyword) && objectName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private void PrepareObject(GameObject go)
    {
        go.isStatic = false;

        if (go.GetComponent<Collider>() == null)
            AddBestCollider(go);

        if (go.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = go.AddComponent<Rigidbody>();
            rb.mass = defaultMass;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        if (go.GetComponent<PickableItem>() == null)
            go.AddComponent<PickableItem>();

        if (setPickableTag)
        {
            try
            {
                go.tag = "Pickable";
            }
            catch (UnityException)
            {
                // Если тега нет в проекте - просто пропускаем.
            }
        }
    }

    private void AddBestCollider(GameObject go)
    {
        MeshFilter meshFilter = go.GetComponent<MeshFilter>();

        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            MeshCollider meshCollider = go.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            return;
        }

        go.AddComponent<BoxCollider>();
    }
}
