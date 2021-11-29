using UnityEngine;

/// <summary>
/// TODO: comments
/// </summary>
public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    public Transform target;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public Vector3 offset;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [Range(1, 10)] public float smoothFactor;

    /// <summary>
    /// TODO: comments
    /// </summary>
    private void FixedUpdate()
    {
        Follow();
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    private void Follow()
    {
        Vector3 targetPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.fixedDeltaTime);
        transform.position = targetPosition;
    }
}
