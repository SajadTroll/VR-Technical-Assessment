using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    private const string UI_FORMAT = "Collected:{0}/{1} Avg:{2:F1}";

    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ItemSpawnManager spawnManager;
    [SerializeField] private CollectionManager collectionManager;
    [SerializeField] private int maxItems = 50;

    private void OnEnable()
    {
        GameEvents.OnCollectedCountChanged += HandleCollectedCountChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnCollectedCountChanged -= HandleCollectedCountChanged;
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        float averageDistance = CalculateAverageDistance();
        int collectedCount = collectionManager != null ? collectionManager.CollectedCount : 0;
        
        infoText.text = string.Format(UI_FORMAT, collectedCount, maxItems, averageDistance);
    }

    private float CalculateAverageDistance()
    {
        if (spawnManager == null || playerTransform == null)
            return 0f;

        List<GameObject> items = spawnManager.SpawnedItems;
        float totalDistance = 0f;
        int activeItemCount = 0;

        foreach (GameObject item in items)
        {
            if (item != null && item.activeSelf)
            {
                totalDistance += Vector3.Distance(playerTransform.position, item.transform.position);
                activeItemCount++;
            }
        }

        return activeItemCount > 0 ? totalDistance / activeItemCount : 0f;
    }

    private void HandleCollectedCountChanged(int newCount)
    {
        UpdateUI();
    }
}
