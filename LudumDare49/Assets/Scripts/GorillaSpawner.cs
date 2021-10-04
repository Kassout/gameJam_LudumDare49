using System.Collections;
using UnityEngine;

public class GorillaSpawner : MonoBehaviour
{
    public GameObject gorilla;
    [SerializeField] private float spawnDelay = 120.0f;

    private float _spawnCountDown;
    private bool _countDownTrigger = true;
    private GameObject _currentGorilla;

    private bool _firstTimeSpawn = true;
    private bool _hasStartedSpawn = false;
    
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

                _countDownTrigger = false;
                
                StartCoroutine(SpawnGorilla());
                
                _spawnCountDown = spawnDelay;
            }
            
            _spawnCountDown -= Time.unscaledDeltaTime;
        }

        if (!_currentGorilla && !_hasStartedSpawn)
        {
            _countDownTrigger = true;
        }
    }

    IEnumerator SpawnGorilla()
    {
        _hasStartedSpawn = true;
        MusicPlayer.Instance.Switch();

        yield return new WaitForSeconds(1.5f);
        
        _currentGorilla = Instantiate(gorilla);
        
        _hasStartedSpawn = false;
        
        _currentGorilla.transform.SetParent(transform);
    }
}
