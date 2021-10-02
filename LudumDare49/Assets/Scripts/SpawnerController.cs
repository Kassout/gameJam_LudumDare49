using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerController : MonoBehaviour
{
    public Transform player;
    public GameObject toSpawnPrefab;
    public SpawnDirection spawnDirection;
    
    [SerializeField] private float minSpawnForce = 0.0f;
    [SerializeField] private float maxSpawnForce = 200.0f;
    [SerializeField] private float spawnTimeWait = 1.0f;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(player.position, transform.position) > minSpawnDistance)
        {
            if (_lastSpawnTime > spawnTimeWait)
            {
                float spawnForce = Random.Range(minSpawnForce, maxSpawnForce);
                GameObject spawnedObject = Instantiate(toSpawnPrefab, transform.position, Quaternion.identity);
                if (spawnedObject.CompareTag("Enemy"))
                {
                    Rigidbody2D spawnedObjectRb = spawnedObject.GetComponent<Rigidbody2D>();
                    spawnedObjectRb.AddForce((Vector2.up + _direction) * spawnForce, ForceMode2D.Impulse);
                }

                _lastSpawnTime = 0.0f;
            }
        }

        _lastSpawnTime += Time.deltaTime;
    }
}
