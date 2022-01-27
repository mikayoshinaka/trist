using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    bool spawn;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject player;
    int halfTime;
    private float timer;
    [SerializeField] float spawnTimeMax=5.0f;
    [SerializeField] GameObject bossUp;
    [SerializeField] GameObject bossLeft;
    [SerializeField] GameObject bossRight;
    [SerializeField] GameObject bossDown;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        spawn = false;
        halfTime = (int)GameObject.Find("Timer").GetComponent<GameTimer>().countdown / 2;
    }

    // Update is called once per frame
    void Update()
    {
        //ボスの出現
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
        //ボスUIの表示
        if(spawn==true&&timer<spawnTimeMax)
        {
            float angle = GetAngle(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(boss.transform.position.x, boss.transform.position.z));
            Debug.Log(angle);
            timer += Time.deltaTime;
            if(angle>=45&& angle <= 135)
            {
                if (bossRight.activeSelf == false)
                {
                    bossRight.SetActive(true);
                }
                if (bossUp.activeSelf == true)
                {
                    bossUp.SetActive(false);
                }
                if (bossLeft.activeSelf == true)
                {
                    bossLeft.SetActive(false);
                }
                if (bossDown.activeSelf == true)
                {
                    bossDown.SetActive(false);
                }
            }
            else if((angle>135&&angle<=180)|| (angle >= -180 && angle <= -135))
            {
                if (bossUp.activeSelf == false)
                {
                    bossUp.SetActive(true);
                }
                if (bossLeft.activeSelf == true)
                {
                    bossLeft.SetActive(false);
                }
                if (bossDown.activeSelf == true)
                {
                    bossDown.SetActive(false);
                }
                if (bossRight.activeSelf == true)
                {
                    bossRight.SetActive(false);
                }
            }
            else if (angle > -135&&angle<=-45)
            {
                if (bossLeft.activeSelf == false)
                {
                    bossLeft.SetActive(true);
                }
                if (bossUp.activeSelf == true)
                {
                    bossUp.SetActive(false);
                }
                if (bossDown.activeSelf == true)
                {
                    bossDown.SetActive(false);
                }
                if (bossRight.activeSelf == true)
                {
                    bossRight.SetActive(false);
                }
               
            }
            else if (angle>-45&&angle<45)
            {
                if (bossDown.activeSelf == false)
                {
                    bossDown.SetActive(true);
                }
                if (bossUp.activeSelf == true)
                {
                    bossUp.SetActive(false);
                }
                if (bossLeft.activeSelf == true)
                {
                    bossLeft.SetActive(false);
                }
                if (bossRight.activeSelf == true)
                {
                    bossRight.SetActive(false);
                }
            }
        }

        if(timer >= spawnTimeMax)
        {

            if (bossUp.activeSelf == true)
            {
                bossUp.SetActive(false);
            }
            if (bossLeft.activeSelf == true)
            {
                bossLeft.SetActive(false);
            }
            if (bossDown.activeSelf == true)
            {
                bossDown.SetActive(false);
            }
            if (bossRight.activeSelf == true)
            {
                bossRight.SetActive(false);
            }
        }
    }
    //角度取得
    float GetAngle(Vector2 start,Vector2 target)
    {
        Vector2 dt = target - start;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }
}
