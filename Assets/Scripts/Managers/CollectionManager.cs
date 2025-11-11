using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    private int collectedCount;

    public int CollectedCount => collectedCount;

    private void OnEnable()
    {
        GameEvents.OnItemCollected += HandleItemCollected;
        GameEvents.OnGameReset += HandleGameReset;
    }

    private void OnDisable()
    {
        GameEvents.OnItemCollected -= HandleItemCollected;
        GameEvents.OnGameReset -= HandleGameReset;
    }

    private void HandleItemCollected(GameObject item)
    {
        ICollectible collectible = item.GetComponent<ICollectible>();
        
        if (collectible != null)
        {
            collectedCount += collectible.CollectValue;
            GameEvents.TriggerCollectedCountChanged(collectedCount);
        }
    }

    private void HandleGameReset()
    {
        collectedCount = 0;
        GameEvents.TriggerCollectedCountChanged(collectedCount);
    }
}
