using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSesameScript : MonoBehaviour
{
    public GameObject clearUI;
    public bool stageClear;
    public static bool restart;

    float timer, timeLimit;



    // 仮のスクリプト

    //小野澤ゲームクリア判定用
    [SerializeField] GameClearOverJugge gameClearOverJugge;

    void Start()
    {
        stageClear = false;
        restart = false;
        timer = 0f;
        timeLimit = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        //小野澤ゲームオーバー用
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }
        if (stageClear)
        {
            timer += Time.deltaTime;
            if (timer >= timeLimit)
            {
                restart = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player"
            //小野澤ゲームクリア判定用
            && gameClearOverJugge.gameOver==false)
        {
            clearUI.SetActive(true);
            stageClear = true;
            Debug.Log("STAGE CLEAR");
        }
    }
}
