﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class MinimapScript : MonoBehaviour
{

    // 仮スクリプト

    public Transform player;
    public GameObject playerTPCamera;
    float camXAxisValue;

    private void Start()
    {
        player = GameObject.Find("Player").transform.GetChild(0);   
    }

    private void LateUpdate()
    {
        camXAxisValue = playerTPCamera.GetComponent<CinemachineFreeLook>().m_XAxis.Value;
        transform.rotation = Quaternion.Euler(90f, camXAxisValue, 0f);
    }
}
