using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GorillaSpawner : MonoBehaviour
{
    public GameObject gorilla;
    [SerializeField] private float spawnDelay = 120.0f;

    private float _spawnCountDown;
    private bool _countDownTrigger = true;
    private GameObject currentGorilla;

    private bool _firstTimeSpawn = true;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnCountDown = spawnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (_countDownTrigger)
        {
            if (_spawnCountDown <= 0)
            {
                if (_firstTimeSpawn)
                {
                    SpawnerController.stopSpawn = true;
                    _firstTimeSpawn = false;
                }
                
                currentGorilla = Instantiate(gorilla);
                currentGorilla.transform.SetParent(transform);

                _countDownTrigger = false;
                _spawnCountDown = spawnDelay;
            }
            
            _spawnCountDown -= Time.unscaledDeltaTime;
        }

        if (!currentGorilla)
        {
            _countDownTrigger = true;
        }
    }
}
