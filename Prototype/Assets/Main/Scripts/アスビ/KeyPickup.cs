using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyPickup : MonoBehaviour
{
    public KeySpawner keySpawner;
    public Text keyUI;
    public GhostChange ghostChange;
    private bool once;

    private void Start()
    {
        keySpawner = transform.parent.GetComponent<KeySpawner>();
        keyUI = keySpawner.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        ghostChange = GameObject.Find("Ghost").GetComponent<GhostChange>();

        keyUI.text = "かぎ\n" + keySpawner.keyPicked + " / " + keySpawner.maxKey;
        once = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        if (ghostChange.possess && !once)
        {
            GetComponent<MeshRenderer>().enabled = true;
            once = true;
        }
        else if (!ghostChange.possess && once)
        {
            GetComponent<MeshRenderer>().enabled = false;
            once = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && keySpawner.keyPicked < keySpawner.maxKey)
        {
            keySpawner.DoorSpawn();
            keySpawner.keyPicked++;
            keyUI.text = "かぎ\n" + keySpawner.keyPicked + " / " + keySpawner.maxKey;
            if(keySpawner.keyPicked != keySpawner.maxKey)
            {
                keySpawner.keys[keySpawner.keyPicked].SetActive(true);
                //keySpawner.SpawnKey();
            }
            Object.Destroy(this.gameObject);
        }
    }
}
