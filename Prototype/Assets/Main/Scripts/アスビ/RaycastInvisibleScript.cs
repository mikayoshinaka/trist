using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInvisibleScript : MonoBehaviour
{
    public GameObject playerRaycast;
    List<string> names = new List<string>();
    int num, hitNum;
    float playerDist;

    [Header("本棚")]
    public Material[] baseColor;
    public Material[] fadeColor;

    [Header("壁")]
    public Material wallColor;
    public Material wallFadeColor;

    void Start()
    {
        num = 0;
    }

    // 家具などの透明処理
    void Update()
    {
        //小野澤ゲームオーバー用
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }
        int layerMask = 1 << 6; // Stages Layer
        layerMask = ~layerMask;

        hitNum = num;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, (playerRaycast.transform.position - transform.position), Mathf.Infinity, layerMask);
        num = hits.Length;

        if (hitNum != num)
        {
            ResetTransparency();

            // プレイヤーの位置を習得
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider.transform.parent.tag == "Player")
                {
                    playerDist = hit.distance;
                }
            }

            // プレイヤーの位置より遠いものを透明化
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider.transform.parent.tag != "Player" && hit.distance < playerDist)
                {
                    if (!names.Contains(hit.collider.name) && hit.collider.tag == "Box")
                    {
                        names.Add(hit.collider.name);
                        hit.collider.gameObject.GetComponent<MeshRenderer>().materials = fadeColor;
                        //Debug.Log(hit.collider.name);
                    }

                    if (!names.Contains(hit.collider.name) && hit.collider.tag == "Wall")
                    {
                        names.Add(hit.collider.name);
                        hit.collider.gameObject.GetComponent<MeshRenderer>().material = wallFadeColor;
                        //Debug.Log(hit.collider.name);
                    }
                }
            }
        }

        Debug.DrawRay(transform.position, (playerRaycast.transform.position - transform.position), Color.yellow);
    }

    public void ResetTransparency()
    {
        string[] check = names.ToArray();
        if (names.Count > 0)
        {
            for (int i = 0; i < names.Count; i++)
            {
                //Debug.Log(check[i]);
                GameObject currentObject = GameObject.Find(check[i]);

                if (currentObject.tag == "Box")
                {
                    currentObject.GetComponent<MeshRenderer>().materials = baseColor;
                }

                if (currentObject.tag == "Wall")
                {
                    currentObject.GetComponent<MeshRenderer>().material = wallColor;
                }
            }
            names.Clear();
        }
    }
}
