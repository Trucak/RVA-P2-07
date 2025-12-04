using UnityEngine;

public class SeedSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay = 2.0f;
    [SerializeField] private float spawnRadius = 0.2f; 

    private GameObject currentSeed;
    private float timer;
    private bool isWaitingToSpawn;

    private void Start()
    {
        if (spawnPoint == null) spawnPoint = transform;
        SpawnSeed();
    }

    private void Update()
    {
        if (currentSeed != null)
        {
            float distance = Vector3.Distance(currentSeed.transform.position, spawnPoint.position);
            if (distance > spawnRadius)
            {
                currentSeed = null;
                isWaitingToSpawn = true;
                timer = 0f;
            }
        }
        else 
        {
            timer += Time.deltaTime;
            if (timer >= spawnDelay)
            {
                SpawnSeed();
                isWaitingToSpawn = false;
                timer = 0f;
            }
        }
    }

    private void SpawnSeed()
    {
        if (seedPrefab != null)
        {
            currentSeed = Instantiate(seedPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
