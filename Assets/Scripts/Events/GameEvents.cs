using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int> OnCollectedCountChanged;
    public static event Action OnGameReset;
    public static event Action<GameObject> OnItemCollected;
    public static event Action<GameObject> OnItemSpawned;

    public static void TriggerCollectedCountChanged(int count)
    {
        OnCollectedCountChanged?.Invoke(count);
    }

    public static void TriggerGameReset()
    {
        OnGameReset?.Invoke();
    }

    public static void TriggerItemCollected(GameObject item)
    {
        OnItemCollected?.Invoke(item);
    }

    public static void TriggerItemSpawned(GameObject item)
    {
        OnItemSpawned?.Invoke(item);
    }

    public static void ClearAllEvents()
    {
        OnCollectedCountChanged = null;
        OnGameReset = null;
        OnItemCollected = null;
        OnItemSpawned = null;
    }
}
