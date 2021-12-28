using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThrough : MonoBehaviour
{
    Transform player;
    Transform mainCameraObj;
    Camera mainCamera;
    LayerMask seeThroughMask;
    [Header("See Through")]
    [SerializeField] private float cutoutSize = 0.15f;
    [SerializeField] private float falloffSize = 0.01f;
    [SerializeField] float radius = 7.5f;
    [Header("Editor View")]
    [SerializeField] bool enableGizmos;

    private void Start()
    {
        player = GameObject.Find("PlayerController").transform;
        mainCameraObj = GameObject.Find("Cameras").transform.Find("Main Camera").transform;
        mainCamera = mainCameraObj.GetComponent<Camera>();
        seeThroughMask = LayerMask.GetMask("SeeThrough");
    }

    private void Update()
    {
        //小野澤　開始用
        if (GameObject.Find("BeforeBegin").GetComponent<BeforeBegin>().begin == true)
        {
            return;
        }
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(player.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        int maxColliders = 20;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(player.position, radius, hitColliders, seeThroughMask);

        if (numColliders != 0)
        {
            float playerDistance = Vector3.Distance(mainCameraObj.position, player.position);
            for (int i = 0; i < numColliders; i++)
            {
                Transform obj = hitColliders[i].transform;
                float objDistance = Vector3.Distance(mainCameraObj.position, obj.position);
                if (objDistance < playerDistance)
                {
                    if (!obj.GetComponent<SeenThroughObject>().seenThrough)
                    {
                        obj.GetComponent<SeenThroughObject>().seenThrough = true;

                        // 透明化
                        SeeThroughActivation(true, cutoutPos, hitColliders, numColliders);
                    }
                }
                else if (obj.GetComponent<SeenThroughObject>().seenThrough)
                {
                    obj.GetComponent<SeenThroughObject>().seenThrough = false;

                    // 元のマテリアルに戻す
                    SeeThroughActivation(false, cutoutPos, hitColliders, numColliders);
                }
            }
        }
    }

    //public void SeeThroughReset()
    //{
    //    Vector2 cutoutPos = GameObject.Find("Main Camera").GetComponent<Camera>().WorldToViewportPoint(this.transform.position);
    //    cutoutPos.y /= (Screen.width / Screen.height);

    //    int maxColliders = 20;
    //    Collider[] hitColliders = new Collider[maxColliders];
    //    int numColliders = Physics.OverlapSphereNonAlloc(this.transform.position, radius, hitColliders, LayerMask.GetMask("SeeThrough"));

    //    for (int i = 0; i < numColliders; i++)
    //    {
    //        Transform obj = hitColliders[i].transform;
    //        if (obj.GetComponent<SeenThroughObject>().seenThrough)
    //        {
    //            obj.GetComponent<SeenThroughObject>().seenThrough = false;
    //            Material[] materials = obj.GetComponent<Renderer>().materials;

    //            for (int j = 0; j < materials.Length; j++)
    //            {
    //                materials[j].SetVector("_CutoutPos", cutoutPos);
    //                materials[j].SetFloat("_CutoutSize", 0);
    //                materials[j].SetFloat("_FalloffSize", 0);
    //            }
    //        }
    //    }
    //}

    // 透明化
    void SeeThroughActivation(bool flag, Vector2 cutoutPos, Collider[] hits, int index)
    {
        Vector2 size;
        if (flag)   // 透明
        {
            size = new Vector2(cutoutSize, falloffSize);
        }
        else    // 元
        {
            size = new Vector2(0, 0);
        }

        for (int i = 0; i < index; i++)
        {
            Material[] materials = hits[i].transform.GetComponent<Renderer>().materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].SetVector("_CutoutPos", cutoutPos);
                materials[j].SetFloat("_CutoutSize", size.x);
                materials[j].SetFloat("_FalloffSize", size.y);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (enableGizmos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(player.position, radius);
        }        
    }
}
