using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform mainCamera;

    private void Start()
    {
        mainCamera = GameObject.Find("Cameras").transform.Find("Main Camera");
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.forward);
    }
}
