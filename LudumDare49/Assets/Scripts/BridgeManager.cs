using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeManager : MonoBehaviour
{
    public GameObject linkPrefab;
    public GameObject lastLinkInstantiated;

    private Transform playerTransform;

    private float spawnY = 0.0f;
    private float bridgeLength = 12.0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject lastLinkInstantiated = Instantiate(linkPrefab) as GameObject;
    }
}
