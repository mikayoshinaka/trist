using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectBoxPost : MonoBehaviour
{
    [Header("Assign SaveBox Manually")]
    public GameObject SaveBox;
    public Transform[] BoxSpots;
    public int saveBoxAnchor;
    public int playerAnchor;
    private Transform player;

    private void Start()
    {
        BoxSpots = new Transform[transform.childCount];
        for (int i = 0; i < BoxSpots.Length; i++)
        {
            BoxSpots[i] = transform.GetChild(i);
        }

        SaveBox.transform.position = BoxSpots[0].transform.position;
        saveBoxAnchor = 0;

        player = GameObject.Find("PlayerController").transform;
    }

    // ハコの配置を決める処理
    public void SwitchBox()
    {
        int exclude = ExcludeNearest();
        int spots = Random.Range(exclude + 1, exclude + BoxSpots.Length) % BoxSpots.Length;
        SaveBox.transform.position = BoxSpots[spots].transform.position;
        saveBoxAnchor = spots;

        for (int i = 0; i < BoxSpots.Length; i++)
        {
            if (i == spots)
            {
                BoxSpots[i].gameObject.SetActive(false);
            }
            else
            {
                BoxSpots[i].gameObject.SetActive(true);
            }
        }
    }

    // プレイヤーから一番近いハコを除く
    int ExcludeNearest()
    {
        int nearestPos = 0;
        float tempValue = 1000f;
        for (int i = 0; i < BoxSpots.Length; i++)
        {
            // NavMeshによる距離計算
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(player.position, BoxSpots[i].transform.position, NavMesh.AllAreas, path);
            float distance = 0f;

            if ((path.status != NavMeshPathStatus.PathInvalid))
            {
                for (int j = 1; j < path.corners.Length; ++j)
                {
                    distance += Vector3.Distance(path.corners[j - 1], path.corners[j]);
                }
            }
            
            if (i == 0)
            {
                tempValue = distance;
                nearestPos = 0;
                continue;
            }
            else if (distance < tempValue)
            {
                tempValue = distance;
                nearestPos = i;
            }
        }

        playerAnchor = nearestPos;
        return nearestPos;
    }

    public void HideBox(bool flag)
    {
        if (!flag)
        {
            SaveBox.SetActive(true);
        }
        else
        {
            GameObject.Find("CameraArea").GetComponent<ShootCameraCanLookBox>().furniture.Clear();
            SaveBox.SetActive(false);
        }
    }
}
