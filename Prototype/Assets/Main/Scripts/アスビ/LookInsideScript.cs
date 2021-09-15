using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LookInsideScript : MonoBehaviour
{
    public GameObject playerTPCamera;
    float camXAxisValue;

    public GameObject stageWall;

    /// ステージの透明
    void Update()
    {
        camXAxisValue = playerTPCamera.GetComponent<CinemachineFreeLook>().m_XAxis.Value;

        if ((camXAxisValue <= 0 && camXAxisValue > -120) || (camXAxisValue >= 0 && camXAxisValue < 30))
        {
            stageWall.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            stageWall.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        if ((camXAxisValue <= -60 && camXAxisValue > -180) || (camXAxisValue <= 180 && camXAxisValue > 150))
        {
            stageWall.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            stageWall.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        if ((camXAxisValue < 180 && camXAxisValue > 60) || (camXAxisValue >= -180 && camXAxisValue < -150))
        {
            stageWall.transform.GetChild(2).GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            stageWall.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        if ((camXAxisValue <= 120 && camXAxisValue > 0) || (camXAxisValue <= 0 && camXAxisValue > -30))
        {
            stageWall.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            stageWall.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
