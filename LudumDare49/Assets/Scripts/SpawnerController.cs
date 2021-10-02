using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerController : MonoBehaviour
{
    public Transform player;
    public GameObject toSpawnPrefab;
    public SpawnDirection spawnDirection;
    
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
    private float _lastSpawnTime = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
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
            if (spawnCountdown < 0 && Vector2.Distance(player.position, transform.position) > minSpawnDistance)
            {
                spawnCountdown += spawnDelay;

                for (int i = 0; i < spawnQuantity; i++)
                {
                    yield return StartCoroutine(SpawnObject());
                }

            }
            
            // Should the spawn rate increase?
            if (spawnRateCountdown < 0 && spawnDelay > 1 && spawnQuantity > 4)
            {
                spawnRateCountdown += timeUntilSpawnRateIncrease;
                if (Random.Range(0, 100) >= spawnQuantityIncreaseChance && spawnDelay > 1)
                {
                    spawnDelay -= 0.1f;
                }
                else if (spawnQuantity > 4)
                {
                    spawnQuantity += 1;
                }
            }
        }
    }

    IEnumerator SpawnObject()
    {
        float spawnForce = Random.Range(minSpawnForce, maxSpawnForce);
        GameObject spawnedObject = Instantiate(toSpawnPrefab, transform.position, Quaternion.identity);
        if (spawnedObject.CompareTag("Enemy"))
        {
            Rigidbody2D spawnedObjectRb = spawnedObject.GetComponent<Rigidbody2D>();
            spawnedObjectRb.AddForce((Vector2.up + _direction) * spawnForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.2f);
    }
}
