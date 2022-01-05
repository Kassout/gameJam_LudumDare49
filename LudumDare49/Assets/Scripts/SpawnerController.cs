using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// TODO: comments
/// </summary>
public class SpawnerController : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    public Transform player;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public GameObject[] toSpawnPrefab;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public SpawnDirection spawnDirection;

    /// <summary>
    /// TODO: comments
    /// </summary>
    public static bool stopSpawn = false;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float minSpawnForce = 0.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float maxSpawnForce = 200.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float spawnDelay = 2.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private int spawnQuantity = 1;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float spawnQuantityIncreaseChance = 20.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float timeUntilSpawnRateIncrease = 30.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float minSpawnDistance = 5.0f;

    /// <summary>
    /// TODO: comments
    /// </summary>
    [Serializable]
    public enum SpawnDirection
    {
        Left,
        Right
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    private Vector2 _direction;
    
    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        stopSpawn = false;
        
        _direction = spawnDirection == SpawnDirection.Left ? Vector2.left : Vector2.right;

        StartCoroutine(SpawnLoop(spawnDelay));
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <param name="firstDelay">TODO: comments</param>
    /// <returns>TODO: comments</returns>
    private IEnumerator SpawnLoop(float firstDelay)
    {
        float spawnRateCountdown = timeUntilSpawnRateIncrease;
        float spawnCountdown = firstDelay;

        while (true)
        {
            yield return true;
            
            if (!stopSpawn)
            {
                spawnRateCountdown -= Time.deltaTime;
                spawnCountdown -= Time.deltaTime;
            
                // Should a new object be spawned?
                if (spawnCountdown < 0 && Vector2.Distance(player.position, transform.position) > minSpawnDistance)
                {
                    spawnCountdown = spawnDelay;

                    for (int i = 0; i < spawnQuantity; i++)
                    {
                        yield return StartCoroutine(SpawnObject());
                    }

                }
            
                // Should the spawn rate increase?
                if (spawnRateCountdown < 0 && spawnDelay > 1.8f && spawnQuantity < 4)
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
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <returns>TODO: comments</returns>
    private IEnumerator SpawnObject()
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
