using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorView : MonoBehaviour
{
    public GhostChange ghostChange;
    GameObject doors;
    private GameObject cameras;
    public GameObject mainCamera;
    public GameObject relayCamera;
    public GameObject closeupCamera;

    public bool gimmickPlay;
    private void Start()
    {
        ghostChange = GameObject.Find("Ghost").GetComponent<GhostChange>();
        doors = transform.GetChild(1).gameObject;

        cameras = GameObject.Find("Cameras");
        mainCamera = cameras.transform.Find("Main Camera").gameObject;
        relayCamera = cameras.transform.Find("RelayCamera").gameObject;
        closeupCamera = cameras.transform.Find("CloseupCamera").gameObject;

        gimmickPlay = false;
    }


    // 調整中
    public void CloseUpDoorView(int doorNum)
    {
        //closeupCamera.transform.position = mainCamera.transform.position;
        //closeupCamera.transform.rotation = mainCamera.transform.rotation;

        // BUG ON RELAY CAMERA

        if (!ghostChange.possess)
        {
            mainCamera.SetActive(false);
        }        
        if (ghostChange.possessObject != null && ghostChange.possess)
        {
            ghostChange.possessObject.transform.Find("Camera").gameObject.SetActive(false);
        }        
        closeupCamera.SetActive(true);

        StartCoroutine(CloseupTimer());
        gimmickPlay = true;

        closeupCamera.transform.position = doors.transform.GetChild(doorNum).GetChild(1).transform.position;
        closeupCamera.transform.rotation = doors.transform.GetChild(doorNum).GetChild(1).transform.rotation;
        //Vector3 targetSpot = doors.transform.GetChild(doorNum).GetChild(1).transform.position;
        //Quaternion targetRotation = doors.transform.GetChild(doorNum).GetChild(1).transform.rotation;
        //closeupCamera.transform.position = Vector3.MoveTowards(closeupCamera.transform.position, targetSpot, 1f * Time.deltaTime);
        //closeupCamera.transform.rotation = Quaternion.Slerp(closeupCamera.transform.rotation, targetRotation, 1f * Time.deltaTime);



        //player.transform.position = Vector3.MoveTowards(player.transform.position, treeTopPosition.transform.position, speed * Time.deltaTime);
        //player.transform.rotation = Quaternion.Slerp(player.transform.rotation, treeTopPosition.transform.rotation, rotate * Time.deltaTime);

        //if (Vector3.Distance(player.transform.position, treeTopPosition.transform.position) < 0.001f)
        //{
        //    // 移動終了
        //    player.transform.position = treeTopPosition.transform.position;
        //    speed = 0;
        //}
        //if (Quaternion.Angle(player.transform.rotation, treeTopPosition.transform.rotation) < 0.001f)
        //{
        //    // 回転終了
        //    player.transform.rotation = treeTopPosition.transform.rotation;
        //    rotate = 0;
        //}
    }

    // BUG ON RELAY CAMERA

    IEnumerator CloseupTimer()
    {
        if (!ghostChange.possess && mainCamera.activeInHierarchy)
        {
            mainCamera.SetActive(false);
        }
        if (ghostChange.possessObject != null && ghostChange.possess && ghostChange.possessObject.transform.Find("Camera").gameObject.activeInHierarchy)
        {
            ghostChange.possessObject.transform.Find("Camera").gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(3f);

        closeupCamera.SetActive(false);
        if (!ghostChange.possess || ghostChange.leave)
        {
            mainCamera.SetActive(true);
        }
        if (ghostChange.possessObject != null && ghostChange.possess && !ghostChange.leave)
        {
            ghostChange.possessObject.transform.Find("Camera").gameObject.SetActive(true);
        }
        gimmickPlay = false;
    }
}
