using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ResultScript : MonoBehaviour
{
    public Text playerHPText;
    public Text gameTimeText;
    int gameTime;
    int playerHP;
    bool stage1;
    bool stage2;
    // Start is called before the first frame update
    void Start()
    {
        gameTime = (int)ManagementScript.GetGameTime();
        playerHP = ManagementScript.GetPlayerHP();
        stage1 = ManagementScript.GetStage1();
        stage2 = ManagementScript.GetStage2();
        playerHPText.text = string.Format("Žc‚èHP:{0}", playerHP);
        gameTimeText.text = string.Format("Œo‰ßŽžŠÔ:{0}", gameTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.B)|| Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if(stage1)
            {
                SceneManager.LoadScene("Stage 2");
            }
            else if(stage2)
            {
                SceneManager.LoadScene("Stage 1");
            }
            else
            {
                SceneManager.LoadScene("Stage 1");
            }
        }
    }
}
