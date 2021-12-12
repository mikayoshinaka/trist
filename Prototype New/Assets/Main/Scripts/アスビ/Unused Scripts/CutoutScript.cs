using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutScript : MonoBehaviour
{
    Transform player;
    LayerMask seeThroughMask;
    Camera mainCamera;

    [Header("See Through")]
    [SerializeField] private float cutoutSize = 0.15f;
    [SerializeField] private float falloffSize = 0.01f;
    int counter;

    List<GameObject> savedList = new List<GameObject>();

    private void Start()
    {
        player = GameObject.Find("Player").transform.Find("PlayerController");
        seeThroughMask = LayerMask.GetMask("SeeThrough");
        mainCamera = GetComponent<Camera>();
        counter = 0;
    }

    private void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(player.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        // Raycast
        Vector3 offset = player.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, offset, offset.magnitude, seeThroughMask);

        if (counter != hits.Length)
        {
            counter = hits.Length;
            CheckObject(hits);
            SeeThrough(cutoutPos, hits);
        }
    }

    void CheckObject(RaycastHit[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject obj = hits[i].collider.gameObject;
            if (!savedList.Contains(obj))
            {
                savedList.Add(obj);
            }
        }
    }

    // “§–¾‰»
    void SeeThrough(Vector2 cutoutPos, RaycastHit[] hits)
    {
        List<GameObject> entryList = new List<GameObject>();
        for (int i = 0; i < hits.Length; i++)
        {
            entryList.Add(hits[i].collider.gameObject);
            Material[] materials = hits[i].transform.GetComponent<Renderer>().materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].SetVector("_CutoutPos", cutoutPos);
                materials[j].SetFloat("_CutoutSize", cutoutSize);
                materials[j].SetFloat("_FalloffSize", falloffSize);
            }
        }

        GameObject[] saved = savedList.ToArray();
        for (int i = 0; i < saved.Length; i++)
        {
            if (!entryList.Contains(saved[i]))
            {
                savedList.Remove(saved[i]);
                Material[] materials = saved[i].GetComponent<Renderer>().materials;

                for (int j = 0; j < materials.Length; j++)
                {
                    materials[j].SetVector("_CutoutPos", cutoutPos);
                    materials[j].SetFloat("_CutoutSize", 0);
                    materials[j].SetFloat("_FalloffSize", 0);
                }
            }
        }
    }
}
