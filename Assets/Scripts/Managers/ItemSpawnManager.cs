using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameConfig config;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Light[] sceneLights;

    private readonly List<GameObject> spawnedItems = new List<GameObject>();
    private Coroutine additionalSpawnCoroutine;

    public List<GameObject> SpawnedItems => spawnedItems;

    private void Start()
    {
        StartCoroutine(InitializeGame());
    }

    private void OnEnable()
    {
        GameEvents.OnGameReset += HandleGameReset;
    }

    private void OnDisable()
    {
        GameEvents.OnGameReset -= HandleGameReset;
        
        if (additionalSpawnCoroutine != null)
        {
            StopCoroutine(additionalSpawnCoroutine);
        }
    }

    private IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(config.gameStartDelay);
        SpawnInitialItems();
        StartAdditionalSpawning();
    }

    private void SpawnInitialItems()
    {
        for (int i = 0; i < config.maxInitialItems; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-config.spawnRange, config.spawnRange),
                Random.Range(config.spawnHeightRange.x, config.spawnHeightRange.y),
                Random.Range(-config.spawnRange, config.spawnRange)
            );

            float scale = Random.Range(config.itemScaleRange.x, config.itemScaleRange.y);
            SpawnItem(position, scale);
        }
    }

    private void StartAdditionalSpawning()
    {
        additionalSpawnCoroutine = StartCoroutine(SpawnAdditionalItems());
    }

    private IEnumerator SpawnAdditionalItems()
    {
        while (true)
        {
            yield return new WaitForSeconds(config.additionalSpawnInterval);

            if (spawnedItems.Count < config.maxTotalItems)
            {
                Vector3 position = new Vector3(
                    Random.Range(config.additionalSpawnRange.x, config.additionalSpawnRange.y),
                    Random.Range(config.additionalSpawnHeightRange.x, config.additionalSpawnHeightRange.y),
                    Random.Range(config.additionalSpawnRange.x, config.additionalSpawnRange.y)
                );

                SpawnItem(position, 1f);
            }
        }
    }

    private void SpawnItem(Vector3 position, float scale)
    {
        GameObject item = Instantiate(itemPrefab, position, Quaternion.identity);
        item.transform.localScale = Vector3.one * scale;

        CollectibleItem collectible = item.GetComponent<CollectibleItem>();
        if (collectible != null)
        {
            collectible.Initialize();
            collectible.SetSceneLights(sceneLights);
        }

        spawnedItems.Add(item);
        GameEvents.TriggerItemSpawned(item);
    }

    private void HandleGameReset()
    {
        DestroyAllItems();
        StartCoroutine(InitializeGame());
    }

    private void DestroyAllItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        
        spawnedItems.Clear();
    }
}
