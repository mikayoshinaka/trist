using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ManagementScript : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] int startPlayerHP = 1;
    public static int playerHP;
    private bool hitAttack;
    private bool result;
    private static bool stage1;
    private static bool stage2;
    private static bool stage3;
    private float impossibleAttackTime = 3.0f;
    private float timer = 0.0f;
    public static float gameTime;
    // Start is called before the first frame update
    void Start()
    {
        playerHP = startPlayerHP;
        gameTime = 0.0f;
        result = false;
        hitAttack = true;
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
        HPManagement();
        TimeManagement();
    }
    private void HPManagement()
    {
        text.text = "PlayerHP:" + playerHP;
        if (hitAttack == false && timer < impossibleAttackTime)
        {
            timer += Time.deltaTime;
            return;
        }
        hitAttack = true;
        timer = 0.0f;
    }
    private void TimeManagement()
    {
        if (result == false)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void PlayerMinusHP()
    {
        if (hitAttack && result == false)
        {
            playerHP -= 1;
            hitAttack = false;
        }
    }

    public static int GetPlayerHP()
    {
        return playerHP;
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
