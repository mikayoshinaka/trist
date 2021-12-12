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

    // Update is called once per frame
    void Update()
    {
        if (possessScript.GetComponent<Possess>().possess ==true)
        {
                EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);          
        }
        else {
            if (ghostCatch.GetComponent<GhostCatch>().mode==GhostCatch.Mode.Carry|| ghostCatch.GetComponent<GhostCatch>().mode == GhostCatch.Mode.Shoot)
            {
                
                    if (enemySearchAreaTransparent.GetComponent<EnemySearchAreaTransparent>().areaEnemy.Contains(this.gameObject)) {
                        EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
                    }
                    else
                    {
                        EnemyInvisible(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
                    }
                
            }
            else
            {
               
                    EnemySeem(this.gameObject.GetComponent<Transform>().transform.GetChild(0).gameObject);
                
            }
        }
    }
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
