using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyForce : MonoBehaviour
{
    public Transform leftAnchor;
    public Transform rightAnchor;

    private List<Rigidbody2D> jointRbs;
    
    // Start is called before the first frame update
    void Start()
    {
        jointRbs = GetComponentsInChildren<Rigidbody2D>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            foreach (var rigidbody in jointRbs)
            {
                if (Vector2.Distance(rigidbody.position, leftAnchor.position) <=
                    Vector2.Distance(rigidbody.position, rightAnchor.position))
                {
                    rigidbody.AddForce(Vector2.up * 400 / (Vector2.Distance(rigidbody.position, leftAnchor.position) / 2), ForceMode2D.Impulse);
                }
                else
                {
                    rigidbody.AddForce(Vector2.up * 400 / (Vector2.Distance(rigidbody.position, rightAnchor.position) / 2), ForceMode2D.Impulse);
                }
            }
        }
    }
}
