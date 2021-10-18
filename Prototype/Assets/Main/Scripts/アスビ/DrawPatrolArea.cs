using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPatrolArea : MonoBehaviour
{
    public Transform[] spots;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(spots[0].position, spots[1].position);
    }
}
