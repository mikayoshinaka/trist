using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LookInsideScript : MonoBehaviour
{
    public GameObject playerTPCamera;
    float camXAxisValue;

    public GameObject stageWall;

    public GhostChange ghostChange;
    private bool resetTransparency;
    private void Start()
    {
        ghostChange = GameObject.Find("Ghost").GetComponent<GhostChange>();
        resetTransparency = false;
    }

    /// ステージの透明
    void Update()
    {
        //小野澤ゲームオーバー用
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        camXAxisValue = playerTPCamera.GetComponent<CinemachineFreeLook>().m_XAxis.Value;

        if (!ghostChange.possess)
        {
            if (resetTransparency)
            {
                resetTransparency = false;
            }

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
        else if (ghostChange.possess && !resetTransparency)
        {
            resetTransparency = true;
            for (int i = 0; i < 4; i++) 
            {
                stageWall.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}
