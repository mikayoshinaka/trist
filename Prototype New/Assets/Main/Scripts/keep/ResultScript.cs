using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ResultScript : MonoBehaviour
{
    public Text playerHPText;
    public Text gameTimeText;
    public Text possessCountText;
    public GameObject possessAchieve;
    public GameObject playerHPAchieve;
    int gameTime;
    int playerHP;
    int possessCount;
    //bool stage1;
    bool stage2;
    //bool stage3;
    // Start is called before the first frame update
    void Start()
    {
        //gameTime = (int)ManagementScript.GetGameTime();
        possessCount = ManagementScript.GetPossessCount();
        playerHP = ManagementScript.GetPlayerHP();
        //stage1 = ManagementScript.GetStage1();
        stage2 = ManagementScript.GetStage2();
        //stage3 = ManagementScript.GetStage3();
        playerHPText.text = string.Format("残りHP:{0}", playerHP);
        //gameTimeText.text = string.Format("経過時間:{0}", gameTime);
        possessCountText.text = string.Format("とりついた回数:{0}", possessCount);
        Achieve();
        if (stage2)
        {
            PlayerPrefs.SetInt("version", 2);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            //if (stage1)
            //{
            //    SceneManager.LoadScene("Stage 2");
            //}
            //else if (stage2)
            //{
            //    SceneManager.LoadScene("Stage 3");
            //}
            //else if (stage3)
            //{
            //    SceneManager.LoadScene("Stage 1");
            //}
            //else
            //{
            //    SceneManager.LoadScene("Stage 1");
            //}
            SceneManager.LoadScene("Menu");
        }
    }
    //Ì†
    private void Achieve()
    {
        if (playerHP >= 3)
        {
            playerHPAchieve.SetActive(true);
        }
        if (possessCount >= 15)
        {
            possessAchieve.SetActive(true);
        }
    }
}
