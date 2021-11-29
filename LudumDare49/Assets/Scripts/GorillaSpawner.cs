using System.Collections;
using UnityEngine;

/// <summary>
/// TODO: comments
/// </summary>
public class GorillaSpawner : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    public GameObject gorilla;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float spawnDelay = 120.0f;

    /// <summary>
    /// TODO: comments
    /// </summary>
    private float _spawnCountDown;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private bool _countDownTrigger = true;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private GameObject _currentGorilla;

    /// <summary>
    /// TODO: comments
    /// </summary>
    private bool _firstTimeSpawn = true;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private bool _hasStartedSpawn = false;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    void Start()
    {
        _spawnCountDown = spawnDelay;
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
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
            
            _spawnCountDown -= Time.deltaTime;
        }

        if (!_currentGorilla && !_hasStartedSpawn)
        {
            _countDownTrigger = true;
        }
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <returns>TODO: comments</returns>
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
