using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent : MonoBehaviour
{
    public GameObject enemySearchAreaTransparent;
    public GameObject possessScript;
    public GameObject ghostCatch;
    
    // Start is called before the first frame update
    void Start()
    {
        enemySearchAreaTransparent = GameObject.Find("SearchArea");
        possessScript= GameObject.Find("CatchArea");
        ghostCatch= GameObject.Find("CatchArea");
    }

    //敵の姿を消す　表示する
    // Update is called once per frame
    void Update()
    {
        //本にとりついている時
        if (possessScript.GetComponent<Possess>().possess ==true)
        {
            if (this.gameObject.tag=="BossEnemyBody")
            {
                EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
                EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(1).gameObject);
                EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(2).gameObject);
                EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(4).gameObject);
                EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(5).gameObject);
            }
            else
            {
                EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
            }        
        }
        else {
            //運んでいる時、人形を入れている時　ボスを入れた後
            if (ghostCatch.GetComponent<GhostCatch>().mode==GhostCatch.Mode.Carry|| ghostCatch.GetComponent<GhostCatch>().mode == GhostCatch.Mode.Shoot|| ghostCatch.GetComponent<GhostCatch>().mode == GhostCatch.Mode.End)
            {
                
                    if (enemySearchAreaTransparent.GetComponent<EnemySearchAreaTransparent>().areaEnemy.Contains(this.gameObject)) {
                    if (this.gameObject.tag == "BossEnemyBody")
                    {
                        EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
                        EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(1).gameObject);
                        EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(2).gameObject);
                        EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(4).gameObject);
                        EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(5).gameObject);
                    }
                    else
                    {
                        EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
                    }
                    }
                    else
                    {
                    if (this.gameObject.tag == "BossEnemyBody")
                    {
                        EnemyInvisible(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
                        EnemyInvisible(this.gameObject.GetComponent<Transform>().transform.GetChild(1).gameObject);
                        EnemyInvisible(this.gameObject.GetComponent<Transform>().transform.GetChild(2).gameObject);
                        EnemyInvisible(this.gameObject.GetComponent<Transform>().transform.GetChild(4).gameObject);
                        EnemyInvisible(this.gameObject.GetComponent<Transform>().transform.GetChild(5).gameObject);
                    }
                    else
                    {
                        EnemyInvisible(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
                    }
                    }
                
            }
            else
            {
                if (this.gameObject.tag == "BossEnemyBody")
                {
                    EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
                    EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(1).gameObject);
                    EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(2).gameObject);
                    EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(4).gameObject);
                    EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(5).gameObject);
                }
                else
                {
                    EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
                }
                
            }
        }
    }
    //見えない
    private void EnemyInvisible(GameObject enemy)
    {
        float a;
        a = enemy.GetComponent<Renderer>().material.GetFloat("_Transparent");
        a += Time.deltaTime;
        if (a < 1.0f)
        {
            enemy.GetComponent<Renderer>().material.SetFloat("_Transparent", a);
        }
        else
        {
            enemy.GetComponent<Renderer>().material.SetFloat("_Transparent", 1.0f);
        }
    }
    //見える
    private void EnemySeem(GameObject enemy)
    {
        float a;
        a = enemy.GetComponent<Renderer>().material.GetFloat("_Transparent");
        a -= Time.deltaTime;
        if (a > -1.0f)
        {
            enemy.GetComponent<Renderer>().material.SetFloat("_Transparent", a);
        }
        else
        {
            enemy.GetComponent<Renderer>().material.SetFloat("_Transparent", -1.0f);
        }

    }
   
}
