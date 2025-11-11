using UnityEngine;

public class CollectibleItem : MonoBehaviour, ICollectible
{
    private const float POSITIVE_VALUE_THRESHOLD = 0.5f;
    private const int POSITIVE_COLLECT_VALUE = 1;
    private const int NEGATIVE_COLLECT_VALUE = -2;

    [SerializeField] private float rotationSpeed = 180f;
    
    private MeshRenderer meshRenderer;
    private Material instanceMaterial;
    private Color itemColor;
    private bool isCollected;
    private Light[] sceneLights;

    public int CollectValue { get; private set; }

    public void Initialize()
    {
        SetupRenderer();
        DetermineItemValue();
        isCollected = false;
    }

    public void SetSceneLights(Light[] lights)
    {
        sceneLights = lights;
    }

    public void Collect(GameObject collector)
    {
        if (isCollected)
            return;

        isCollected = true;
        GameEvents.TriggerItemCollected(gameObject);
        Destroy(gameObject);
    }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        RotateItem();
        UpdateColorBasedOnLighting();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Collect(collision.gameObject);
        }
    }

    private void SetupRenderer()
    {
        if (meshRenderer == null)
            return;

        instanceMaterial = new Material(meshRenderer.material);
        meshRenderer.material = instanceMaterial;
        meshRenderer.material.color = Color.white;
    }

    private void DetermineItemValue()
    {
        float randomValue = Random.Range(0f, 1f);
        
        if (randomValue >= POSITIVE_VALUE_THRESHOLD)
        {
            itemColor = Color.green;
            CollectValue = POSITIVE_COLLECT_VALUE;
        }
        else
        {
            itemColor = Color.red;
            CollectValue = NEGATIVE_COLLECT_VALUE;
        }
    }

    private void RotateItem()
    {
        float randomRotation = Random.Range(10f, 360f);
        transform.Rotate(Vector3.up * randomRotation * Time.deltaTime);
    }

    private void UpdateColorBasedOnLighting()
    {
        if (meshRenderer == null || sceneLights == null || sceneLights.Length < 2)
            return;

        bool isLit = IsIlluminatedByLight(sceneLights[0]) || IsIlluminatedByLight(sceneLights[1]);
        meshRenderer.material.color = isLit ? itemColor : Color.white;
    }

    private bool IsIlluminatedByLight(Light light)
    {
        if (light == null)
            return false;

        Ray ray = new Ray(light.transform.position, light.transform.forward);
        
        if (Physics.SphereCast(ray, 1f, out RaycastHit hit, 5f))
        {
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (sceneLights == null || sceneLights.Length < 2)
            return;

        Gizmos.color = itemColor;
        DrawLightRayGizmo(sceneLights[0]);
        DrawLightRayGizmo(sceneLights[1]);
    }

    private void DrawLightRayGizmo(Light light)
    {
        if (light == null)
            return;

        const float rayLength = 5f;
        const float sphereRadius = 0.2f;

        Vector3 startPosition = light.transform.position;
        Vector3 endPosition = startPosition + light.transform.forward * rayLength;
        
        Gizmos.DrawLine(startPosition, endPosition);
        Gizmos.DrawWireSphere(startPosition, sphereRadius);
    }
}
