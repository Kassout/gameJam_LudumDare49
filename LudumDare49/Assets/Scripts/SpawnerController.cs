using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerController : MonoBehaviour
{
    public Transform player;
    public GameObject[] toSpawnPrefab;
    public SpawnDirection spawnDirection;

    public static bool stopSpawn = false;
    
    [SerializeField] private float minSpawnForce = 0.0f;
    [SerializeField] private float maxSpawnForce = 200.0f;
    [SerializeField] private float spawnDelay = 2.0f;
    [SerializeField] private int spawnQuantity = 1;
    [SerializeField] private float spawnQuantityIncreaseChance = 20.0f;
    [SerializeField] private float timeUntilSpawnRateIncrease = 30.0f;
    [SerializeField] private float minSpawnDistance = 5.0f;

    [Serializable]
    public enum SpawnDirection
    {
        Left,
        Right
    }

    private Vector2 _direction;
    
    // Start is called before the first frame update
    void Start()
    {
        stopSpawn = false;
        
        _direction = spawnDirection == SpawnDirection.Left ? Vector2.left : Vector2.right;

        StartCoroutine(SpawnLoop(spawnDelay));
    }

    IEnumerator SpawnLoop(float firstDelay)
    {
        float spawnRateCountdown = timeUntilSpawnRateIncrease;
        float spawnCountdown = firstDelay;

        while (true)
        {
            yield return true;

            spawnRateCountdown -= Time.deltaTime;
            spawnCountdown -= Time.deltaTime;
            
            // Should a new object be spawned?
            if (spawnCountdown < 0 && Vector2.Distance(player.position, transform.position) > minSpawnDistance && !stopSpawn)
            {
                spawnCountdown = spawnDelay;

                for (int i = 0; i < spawnQuantity; i++)
                {
                    yield return StartCoroutine(SpawnObject());
                }

            }
            
            // Should the spawn rate increase?
            if (spawnRateCountdown < 0 && spawnDelay > 1 && spawnQuantity < 4)
            {
                spawnRateCountdown += timeUntilSpawnRateIncrease;
                if (Random.Range(0, 100) >= spawnQuantityIncreaseChance && spawnDelay > 1)
                {
                    spawnDelay -= 0.1f;
                }
                else if (spawnQuantity < 4)
                {
                    spawnQuantity += 1;
                }
            }
        }
    }

    IEnumerator SpawnObject()
    {
        float spawnForce = Random.Range(minSpawnForce, maxSpawnForce);
        int toSpawnIndex = Random.Range(0, 100) > 10 ? 0 : 1;
        
        GameObject spawnedObject = Instantiate(toSpawnPrefab[toSpawnIndex], transform.position, Quaternion.identity);
        if (spawnedObject.GetComponent<Rigidbody2D>())
        {
            Rigidbody2D spawnedObjectRb = spawnedObject.GetComponent<Rigidbody2D>();
            spawnedObjectRb.AddForce(_direction * spawnForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.2f);
    }
}
