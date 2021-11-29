using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TODO: comments
/// </summary>
public class ApplyForce : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    public Transform leftAnchor;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public Transform rightAnchor;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private List<Rigidbody2D> jointRbs;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    void Start()
    {
        jointRbs = GetComponentsInChildren<Rigidbody2D>().ToList();
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
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
