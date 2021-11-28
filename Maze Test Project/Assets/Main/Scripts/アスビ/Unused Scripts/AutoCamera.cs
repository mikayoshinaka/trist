using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCamera : MonoBehaviour
{
    public GameObject mainCamera;
    public Transform player;
    public Vector3 cameraPosOffset;

    private void Start()
    {
        mainCamera = GameObject.Find("Cameras").transform.Find("Main Camera").gameObject;
        player = GameObject.Find("Player").transform.GetChild(0);
    }

    private void LateUpdate()
    {
        Vector3 cameraPos = mainCamera.transform.position;
        mainCamera.transform.position = new Vector3(player.position.x, cameraPos.y, player.position.z + cameraPosOffset.z);
    }
}
