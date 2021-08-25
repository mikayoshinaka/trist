using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInvisibleScript : MonoBehaviour
{
    public GameObject playerRaycast;
    List<string> names = new List<string>();
    int num, hitNum;
    float playerDist;
   
    public Material color1, color2;

    void Start()
    {
        num = 0;
    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << 6; // Stages Layer
        layerMask = ~layerMask;

        hitNum = num;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, (playerRaycast.transform.position - transform.position), Mathf.Infinity, layerMask);
        num = hits.Length;

        if (hitNum != num)
        {
            string[] check = names.ToArray();
            if (names.Count > 0)
            {
                for (int i = 0; i < names.Count; i++)
                {
                    GameObject.Find(check[i]).GetComponent<MeshRenderer>().material = color1;
                }
                names.Clear();
            }

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider.transform.parent.tag == "Player")
                {
                    playerDist = hit.distance;
                }
            }

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider.transform.parent.tag != "Player" && hit.distance < playerDist)
                {
                    if (!names.Contains(hit.collider.name))
                    {
                        names.Add(hit.collider.name);
                        hit.collider.gameObject.GetComponent<MeshRenderer>().material = color2;
                    }
                }
            }
        }

        Debug.DrawRay(transform.position, (playerRaycast.transform.position - transform.position), Color.yellow);
    }
}
