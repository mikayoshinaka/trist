using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMigrate : MonoBehaviour
{
    //�Q�[���I�[�o�[�p

    [SerializeField] GameObject GameOverScene;
    [SerializeField] GameObject gameOver;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver()
    {
        GameOverScene.SetActive(true);
        Time.timeScale = 0f;
        gameOver.SetActive(false);
    }
}
