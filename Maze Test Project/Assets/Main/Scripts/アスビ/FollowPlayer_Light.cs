using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer_Light : MonoBehaviour
{
    GameObject playerController;
    public float height = 10f;

    private void Start()
    {
        playerController = GameObject.Find("Player").transform.Find("PlayerController").gameObject;
    }

    private void LateUpdate()
    {
        Vector3 playerPos = playerController.transform.position;
        transform.position = new Vector3(playerPos.x, playerPos.y + height, playerPos.z);
    }
}
