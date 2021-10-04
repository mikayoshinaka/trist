using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    Scene currentScene;
    public static bool gameOver;
    public GameObject gameOverUI;
    GameObject canvas;
    //小野澤　ゲームオーバー用
    [SerializeField] GameOverMigrate gameOverMigrate;
    //小野澤ゲームクリア判定用
    [SerializeField] GameClearOverJugge gameClearOverJugge;

    void Start()
    {
        canvas = transform.GetChild(0).gameObject;
        canvas.SetActive(true);

        currentScene = SceneManager.GetActiveScene();
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            gameOver = false;
            //小野澤ゲームクリア判定用
            if (gameClearOverJugge.gameClear==false) {
                StartCoroutine(GameOver());

                gameOverUI.SetActive(true);
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

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        //小野澤　ゲームオーバー用
        gameOverMigrate.GameOver();
        //SceneManager.LoadScene(currentScene.name);

    }
}
