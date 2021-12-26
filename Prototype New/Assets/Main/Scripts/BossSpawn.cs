using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    bool spawn;
    [SerializeField] GameObject boss;
    int halfTime;
    // Start is called before the first frame update
    void Start()
    {
        spawn = false;
        halfTime =(int)GameObject.Find("Timer").GetComponent<GameTimer>().countdown / 2;
    }

    // Update is called once per frame
    void Update()
    {
        //É{ÉXÇÃèoåª
        if(spawn==false&&
          ((GameObject.Find("CatchArea").GetComponent<GhostCatch>().mode == GhostCatch.Mode.CanGrab)||
           (GameObject.Find("CatchArea").GetComponent<GhostCatch>().mode == GhostCatch.Mode.CannotGrab) ||
           (GameObject.Find("CatchArea").GetComponent<GhostCatch>().mode == GhostCatch.Mode.Carry))
            && GameObject.Find("Timer").GetComponent<GameTimer>().countdown <= halfTime
            &&!(GameObject.Find("GameState").GetComponent<GameStateManager>().gameState== GameStateManager.GameState.gameState_Maze))
        {
            spawn = true;
            boss.SetActive(true);
        }

    }
}
