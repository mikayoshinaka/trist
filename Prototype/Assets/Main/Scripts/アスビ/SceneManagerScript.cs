using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public static bool gameOver;
    public GameObject gameOverUI;
    bool countdown;
    float timer, timeLimit;

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        countdown = false;
        timer = 0f;
        timeLimit = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            gameOverUI.SetActive(true);
            gameOver = false;
            countdown = true;
        }
        
        if (countdown)
        {
            timer += Time.deltaTime;
            if (timer >= timeLimit)
            {
                SceneManager.LoadScene("Stage 1");
            }
        }

        if (OpenSesameScript.restart)
        {
            SceneManager.LoadScene("Result");
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            SceneManager.LoadScene("Stage 1");
        }
    }
}
