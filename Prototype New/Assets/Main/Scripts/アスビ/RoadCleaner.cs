using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCleaner : MonoBehaviour
{
    MazeAssignment mazeAssignment;
    public float radius = 5f;
    public bool enableGizmos;

    LayerMask bookstandMask;
    LayerMask deskMask;
    LayerMask chairMask;

    private void Awake()
    {
        mazeAssignment = GameObject.Find("Maze").GetComponent<MazeAssignment>();
        bookstandMask = LayerMask.GetMask("Bookstand");
        chairMask = LayerMask.GetMask("Chair");
        deskMask = LayerMask.GetMask("Desk");
        enableGizmos = false;     
    }

    private void OnEnable()
    {
        int maxColliders = 25;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, bookstandMask + deskMask + chairMask);
        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].transform.parent.tag == "Box")
            {
                if (!mazeAssignment.bookstandList.Contains(hitColliders[i].transform.parent.gameObject))
                {
                    hitColliders[i].transform.parent.gameObject.SetActive(false);
                }
            }
            if (hitColliders[i].transform.parent.tag == "Desk")
            {
                if (!mazeAssignment.deskList.Contains(hitColliders[i].transform.parent.gameObject))
                {
                    hitColliders[i].transform.parent.gameObject.SetActive(false);
                }
            }
            if (hitColliders[i].transform.parent.tag == "Chair")
            {
                if (!mazeAssignment.chairList.Contains(hitColliders[i].transform.parent.gameObject))
                {
                    hitColliders[i].transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (enableGizmos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
