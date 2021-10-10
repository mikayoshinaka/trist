using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ManagementScript : MonoBehaviour
{  //リザルト用


    [SerializeField] Text text;
    [SerializeField] Text possessText;
    [SerializeField] Text searchText;
    [SerializeField] int startPlayerHP = 1;
    [SerializeField] GhostChange ghostChange;
    [SerializeField] EnemySearch enemySearch;
    [SerializeField] GameObject enemySearchObj;
    public static int playerHP;
    public static int possessCount;
    //private bool hitAttack;
    private bool result;
    private static bool stage1;
    private static bool stage2;
    private static bool stage3;
    //private float impossibleAttackTime = 3.0f;
    //private float timer = 0.0f;
    public static float gameTime;
    // Start is called before the first frame update
    void Start()
    {
        playerHP = startPlayerHP;
        possessCount = 0;
        gameTime = 0.0f;
        result = false;
        //hitAttack = true;
        if (SceneManager.GetActiveScene().name == "Stage 1")
        {
            stage1 = true;
            stage2 = false;
            stage3 = false;
        }
        else if (SceneManager.GetActiveScene().name == "Stage 2")
        {
            stage1 = false;
            stage2 = true;
            stage3 = false;
        }
        else if (SceneManager.GetActiveScene().name == "Stage 3")
        {
            stage1 = false;
            stage2 = false;
            stage3 = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }
        HPManagement();
        TimeManagement();
        PossessManagement();
        SearchManagement();
    }
    //現在のHP
    private void HPManagement()
    {
        text.text = "PlayerHP:" + playerHP;
        //if (hitAttack == false && timer < impossibleAttackTime)
        //{
        //    timer += Time.deltaTime;
        //    return;
        //}
        //hitAttack = true;
        //timer = 0.0f;
    }
    //時間経過
    private void TimeManagement()
    {
        if (result == false)
        {
            gameTime += Time.deltaTime;
        }
    }
    private void PossessManagement()
    {
        if (ghostChange.canPossessText == true)
        {
            possessText.text = "とりつき:可能";
        }
        else if (ghostChange.canPossessText == false&&ghostChange.possess==true) 
        {
            possessText.text = "とりついている";
        }
        else
        {
            possessText.text = "";
        }

    }
    private void SearchManagement()
    {
        if (enemySearch.silhouette == false && enemySearchObj.activeSelf&&ghostChange.possessObject.tag== "Monkey")
        {
            searchText.text = "サーチ:可能";
        }
        else if (enemySearch.silhouette == true && enemySearchObj.activeSelf)
        {
            searchText.text = "サーチしている";
        }
        else
        {
            searchText.text = "";
        }

    }
    //とりついた回数
    public void PlusPossessCount()
    {
        possessCount += 1;
    }
    //HP減少
    public void PlayerMinusHP()
    {
        //if (hitAttack && result == false)
        //{
        playerHP -= 1;
        //hitAttack = false;
        //}
    }

    public static int GetPlayerHP()
    {
        return playerHP;
    }
    public static int GetPossessCount()
    {
        return possessCount;
    }
    public static float GetGameTime()
    {
        return gameTime;
    }
    public static bool GetStage1()
    {
        return stage1;
    }
    public static bool GetStage2()
    {
        return stage2;
    }
    public static bool GetStage3()
    {
        return stage3;
    }
}
