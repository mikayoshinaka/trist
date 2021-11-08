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
            Color enemyDefColor;
            enemy.GetComponent<Renderer>().material.SetOverrideTag("RenderType", "Transparent");
            enemy.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            enemy.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            enemy.GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
            enemy.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            enemy.GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
            enemy.GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            enemy.GetComponent<Renderer>().material.renderQueue = 3000;
            enemyDefColor = enemy.GetComponent<Renderer>().material.color;
            enemyDefColor.a -= Time.deltaTime;
            if (enemyDefColor.a > 0.0f)
            {
                enemy.GetComponent<Renderer>().material.color = enemyDefColor;
            }
            else
            {
                enemy.GetComponent<Renderer>().material.color = new Color(enemyDefColor.r, enemyDefColor.g, enemyDefColor.b, 0.0f);
            }
    }
    private void EnemySeem(GameObject enemy)
    {
            Color enemyDefColor;
            enemyDefColor = enemy.GetComponent<Renderer>().material.color;
            enemyDefColor.a += Time.deltaTime;
            if (enemyDefColor.a < 1.0f)
            {
                enemy.GetComponent<Renderer>().material.color = enemyDefColor;
            }
            else
            {
                enemy.GetComponent<Renderer>().material.SetOverrideTag("RenderType", "");
                enemy.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                enemy.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                enemy.GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
                enemy.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
                enemy.GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
                enemy.GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                enemy.GetComponent<Renderer>().material.renderQueue = -1;
                enemy.GetComponent<Renderer>().material.color = new Color(enemyDefColor.r, enemyDefColor.g, enemyDefColor.b, 1.0f);
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
