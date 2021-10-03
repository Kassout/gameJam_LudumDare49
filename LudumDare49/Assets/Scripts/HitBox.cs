using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public EnemyController origin;

    private void OnDestroy()
    {
        origin.attacking = false;
    }
}
