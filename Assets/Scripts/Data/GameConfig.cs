using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Configuration")]
public class GameConfig : ScriptableObject
{
    [Header("Spawn Settings")]
    public int maxInitialItems = 50;
    public float spawnRange = 25f;
    public Vector2 spawnHeightRange = new Vector2(2f, 5f);
    public Vector2 itemScaleRange = new Vector2(0.2f, 1.7f);

    [Header("Runtime Spawn Settings")]
    public int maxTotalItems = 100;
    public float additionalSpawnInterval = 0.3f;
    public Vector2 additionalSpawnRange = new Vector2(-10f, 10f);
    public Vector2 additionalSpawnHeightRange = new Vector2(2f, 7f);

    [Header("Item Settings")]
    public float itemRotationSpeed = 180f;
    public float lightRaycastDistance = 5f;
    public float lightRaycastRadius = 1f;

    [Header("Timing")]
    public float gameStartDelay = 0.5f;
}
