using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearOverJugge : MonoBehaviour
{
    public bool gameClear;
    public bool gameOver;
    [SerializeField] OpenSesameScript openSesameScript;
    // Start is called before the first frame update
    void Start()
    {
        gameClear = false;
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(openSesameScript.stageClear==true&&gameOver==false)
        {
            gameClear = true;
        }
        if (SceneManagerScript.gameOver == true&&gameClear==false)
        {
            gameOver = true;
        }
    }
}
