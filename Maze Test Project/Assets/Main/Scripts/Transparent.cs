using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent : MonoBehaviour
{
    public List<GameObject> enterObject = new List<GameObject>();
    [SerializeField] EnemySearchAreaTransparent enemySearchAreaTransparent;
    public bool catchEnemy;
    public bool bookShelfPossess;
    // Start is called before the first frame update
    void Start()
    {
        catchEnemy = false;
        bookShelfPossess = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (bookShelfPossess==true)
        {
            for (int i = 0; i < enterObject.Count; i++)
            {
                EnemySeem(enterObject[i].GetComponent<Transform>().transform.GetChild(0).gameObject);
            }
        }
        else {
            if (catchEnemy)
            {
                for (int i = 0; i < enterObject.Count; i++)
                {
                    if (enemySearchAreaTransparent.areaEnemy.Contains(enterObject[i])) {
                        EnemySeem(enterObject[i].GetComponent<Transform>().transform.GetChild(0).gameObject);
                    }
                    else
                    {
                        EnemyInvisible(enterObject[i].GetComponent<Transform>().transform.GetChild(0).gameObject);
                    }
                }
            }
            else if (catchEnemy == false)
            {
                for (int i = 0; i < enterObject.Count; i++)
                {
                    EnemySeem(enterObject[i].GetComponent<Transform>().transform.GetChild(0).gameObject);
                }
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "YellowEnemyBody"|| other.tag == "BlueEnemyBody" || other.tag == "RedEnemyBody")
        {
            Debug.Log("hit");
            if (!enterObject.Contains(other.gameObject))
            {
                enterObject.Add(other.gameObject);
            }
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "YellowEnemyBody" || other.tag == "BlueEnemyBody" || other.tag == "RedEnemyBody")
        {
            if (enterObject.Contains(other.gameObject))
            {
                enterObject.Remove(other.gameObject);
            }
        }
    }
}
