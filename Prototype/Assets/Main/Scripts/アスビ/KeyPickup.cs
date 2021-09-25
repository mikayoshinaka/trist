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

    public DoorView doorView;

    //ドアのアニメーション用
    public KeyDoorScript keyDoorScript;
    // ObjectVisibilityに合わせる
    public enum KeyType
    {
        type1,
        type2,
        type3
    };
    public KeyType keyType;

    private void Start()
    {
        keySpawner = transform.parent.GetComponent<KeySpawner>();
        keyUI = keySpawner.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        ghostChange = GameObject.Find("Ghost").GetComponent<GhostChange>();
        doorView = GameObject.Find("Door Gimmick").GetComponent<DoorView>();

        keyUI.text = "かぎ\n" + keySpawner.keyPicked + " / " + keySpawner.maxKey;
        once = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
    }

    private void Update()
    {
        //小野澤ゲームオーバー用
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        // 鍵が見える範囲
        if (!once && ghostChange.possess && ghostChange.possessObject.tag == "Monkey")
        {
            bool match = ObjectTypeCheck();
            if (match)
            {
                GetComponent<MeshRenderer>().enabled = true;
                GetComponent<SphereCollider>().enabled = true;
            }
            once = true;
        }
        else if (once && !ghostChange.possess)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            once = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 鍵の習得
        if (other.tag == "Player" && keySpawner.keyPicked < keySpawner.maxKey)
        {
            //ドアのアニメーション用
            keyDoorScript.KeyTouch();
            doorView.CloseUpDoorView(keySpawner.keyPicked);

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

    // ObjectVisibility と Keytype を確認する
    bool ObjectTypeCheck()
    {
        string objectType = ghostChange.possessObject.GetComponent<ObjectVisibility>().visibilityType.ToString();
        string keyTypename = keyType.ToString();

        if (objectType == keyTypename)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
