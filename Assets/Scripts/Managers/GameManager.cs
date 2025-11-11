using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ItemSpawnManager spawnManager;
    [SerializeField] private CollectionManager collectionManager;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ResetGame();
        }
    }

    public void ResetGame()
    {
        GameEvents.TriggerGameReset();
    }

    private void OnDestroy()
    {
        GameEvents.ClearAllEvents();
    }
}
