using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyKey : MonoBehaviour
{
    public GameObject dummySpawned;
    public GhostChange ghostChange;
    private bool once;

    private void Start()
    {
        ghostChange = GameObject.Find("Ghost").GetComponent<GhostChange>();
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
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<SphereCollider>().enabled = true;
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
        if (other.tag == "Player")
        {
            dummySpawned.SetActive(true);
            StartCoroutine(DestroyCountdown());
        }
    }

    IEnumerator DestroyCountdown()
    {
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(2f);
        Object.Destroy(this.gameObject);
    }
}
