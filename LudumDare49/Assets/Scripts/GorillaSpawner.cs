using UnityEngine;

public class GorillaSpawner : MonoBehaviour
{
    public GameObject gorilla;
    [SerializeField] private float spawnDelay = 120.0f;

    private float _spawnCountDown;
    private bool _countDownTrigger = true;
    private GameObject currentGorilla;
    
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
                currentGorilla = Instantiate(gorilla);
                currentGorilla.transform.SetParent(transform);

                _countDownTrigger = false;
                _spawnCountDown = spawnDelay;
            }
            
            _spawnCountDown -= Time.deltaTime;
        }

        if (!currentGorilla)
        {
            _countDownTrigger = true;
        }
    }
}
