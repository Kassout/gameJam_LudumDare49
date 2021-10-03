using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitBox : MonoBehaviour
{
    public enemyController origin;

    private void OnDestroy()
    {
        origin.attacking = false;
    }
}
