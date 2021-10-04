using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorilla : MonoBehaviour
{
    public float maxLeft;
    public float maxRight;


    void Update()
    {
        float targetX = Random.Range(transform.position.x - 5, transform.position.x + 5);

        transform.Translate(new Vector3(targetX, 0, 0));

        if(transform.position.x > maxRight)
        {
            transform.position = new Vector3(maxRight, 0, 0);
        }
        if (transform.position.x < maxLeft)
        {
            transform.position = new Vector3(maxLeft, 0, 0);
        }
    }
}
