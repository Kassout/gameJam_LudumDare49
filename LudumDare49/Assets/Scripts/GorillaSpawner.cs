using System.Collections;
using UnityEngine;

/// <summary>
/// Class <c>GorillaSpawner</c> is a Unity component script used to manage the gorilla enemy spawner behavior.
/// </summary>
public class GorillaSpawner : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance field <c>gorillaPrefab</c> is a Unity <c>GameObject</c> representing the gorilla prefabricated object to spawn.
    /// </summary>
    [SerializeField] private GameObject gorillaPrefab;
    
    /// <summary>
    /// Instance field <c>spawnDelay</c> represents the duration value between two gorilla enemy spawns.
    /// </summary>
    [SerializeField] private float spawnDelay = 120.0f;

    /// <summary>
    /// Instance field <c>spawnCountDown</c> represents the time before the last gorilla enemy spawn.
    /// </summary>
    private float _spawnCountDown;
    
    /// <summary>
    /// Instance field <c>countDownTrigger</c> represents the starting trigger status of the gorilla enemy spawning count down.
    /// </summary>
    private bool _countDownTrigger = true;
    
    /// <summary>
    /// Instance field <c>currentGorilla</c> is a Unity <c>GameObject</c> representing the current gorilla enemy game object.
    /// </summary>
    private GameObject _currentGorilla;

    /// <summary>
    /// Instance field <c>firstTimeSpawn</c> represents the first time spawning gorilla enemy status of the game.
    /// </summary>
    private bool _firstTimeSpawn = true;
    
    /// <summary>
    /// Instance field <c>hasStartedSpawn</c> represents the starting spawn process status of the gorilla enemy.
    /// </summary>
    private bool _hasStartedSpawn;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _spawnCountDown = spawnDelay;
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
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

    #endregion

    #region Private

    /// <summary>
    /// This function is responsible for managing the spawning gorilla enemy.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator SpawnGorilla()
    {
        _hasStartedSpawn = true;
        MusicPlayer.Instance.Switch();

        yield return new WaitForSeconds(1.5f);
        
        _currentGorilla = Instantiate(gorillaPrefab, transform, true);
        
        _hasStartedSpawn = false;
    }

    #endregion
}
