using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Class <c>SpawnerController</c> is a Unity component script used to manage the enemy spawn behavior.
/// </summary>
public class SpawnerController : MonoBehaviour
{
    #region Fields / Properties

        /// <summary>
    /// Instance field <c>player</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the player game object.
    /// </summary>
    [Header("Spawner setup parameters")]
    [SerializeField] private Transform player;
    
    /// <summary>
    /// Instance field <c>toSpawnPrefab</c> is a list of Unity <c>GameObject</c> representing the different spawnable game object.
    /// </summary>
    [SerializeField] private GameObject[] toSpawnPrefab;
    
    /// <summary>
    /// Instance field of the <c>SpawnDirection</c> enumeration, <c>spawnDirection</c> represents the spawning direction of the spawner game object.
    /// </summary>
    [SerializeField] private SpawnDirection spawnDirection;
    
    /// <summary>
    /// Enum field <c>SpawnDirection</c> representing the different spawn direction of the spawner.
    /// </summary>
    public enum SpawnDirection
    {
        Left,
        Right
    }

    /// <summary>
    /// Instance field <c>direction</c> represents the spawning vector direction.
    /// </summary>
    private Vector2 _direction;

    /// <summary>
    /// Instance field <c>stopSpawn</c> represents the stop spawn status of the spawner game object.
    /// </summary>
    public static bool stopSpawn;
    
    /// <summary>
    /// Instance field <c>minSpawnForce</c> represents the minimum ejection force value of the game object spawned by the spawner game object.
    /// </summary>
    [Header("Spawn parameters")]
    [SerializeField] private float minSpawnForce = 0.0f;
    
    /// <summary>
    /// Instance field <c>maxSpawnForce</c> represents the maximum ejection force value of the game object spawned by the spawner game object.
    /// </summary>
    [SerializeField] private float maxSpawnForce = 200.0f;
    
    /// <summary>
    /// Instance field <c>spawnDelay</c> represents the duration value between two spawns.
    /// </summary>
    [SerializeField] private float spawnDelay = 2.0f;
    
    /// <summary>
    /// Instance field <c>spawnQuantity</c> represents the number of game objects to spawn at each spawn tick.
    /// </summary>
    [SerializeField] private int spawnQuantity = 1;
    
    /// <summary>
    /// Instance field <c>spawnQuantityIncreaseChance</c> represents the probability value in percentage of spawning quantity value to increase at each spawn rate increase.
    /// </summary>
    [SerializeField] private float spawnQuantityIncreaseChance = 20.0f;
    
    /// <summary>
    /// Instance field <c>timeUntilSpawnRateIncrease</c> represents the duration value until the spawn rate increase.
    /// </summary>
    [SerializeField] private float timeUntilSpawnRateIncrease = 30.0f;
    
    /// <summary>
    /// Instance field <c>minSpawnDistance</c> represents the minimum distance value at which the spawned object are send.
    /// </summary>
    [SerializeField] private float minSpawnDistance = 5.0f;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        stopSpawn = false;
        
        _direction = spawnDirection == SpawnDirection.Left ? Vector2.left : Vector2.right;

        StartCoroutine(SpawnLoop(spawnDelay));
    }

    #endregion

    #region Private

        /// <summary>
    /// This function is responsible for managing the spawning loop behavior of the spawner game object.
    /// </summary>
    /// <param name="firstDelay">A float value representing the first time spawn delay before being evolved by the spawn controller.</param>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
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
    /// This function is responsible for managing the spawning behavior of the spawner.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
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

    #endregion
}
