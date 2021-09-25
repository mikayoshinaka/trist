using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAppearColor : MonoBehaviour
{
    private GameObject enemy;
    [SerializeField] private GameObject[] doorAppearEnemy;
    [SerializeField] private GameObject closeupCamera;
    private Color enemyDefColor;
    public bool doorScene = false;
    bool closeup = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }
        EnemyAppear();
    }

    private void EnemyAppear()
    {
        if (closeupCamera.activeSelf == true && doorScene == true)
        {
            closeup = true;
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.SetOverrideTag("RenderType", "");
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.renderQueue = -1;
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(enemyDefColor.r, enemyDefColor.g, enemyDefColor.b, 1.0f);
        }
        else if (closeupCamera.activeSelf == false && doorScene == true && closeup == true)
        {
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.SetOverrideTag("RenderType", "Transparent");
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.renderQueue = 3000;
            enemy.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(enemyDefColor.r, enemyDefColor.g, enemyDefColor.b, 0.0f);
            doorScene = false;
            closeup = false;
        }
    }

    public void SetAppearEnemy(GameObject doorEnemy = null)
    {
        for (int i = 0; i < doorAppearEnemy.Length; i++)
        {
            if (doorAppearEnemy[i] == doorEnemy)
            {
                enemy = doorAppearEnemy[i];
                enemyDefColor = enemy.transform.GetChild(0).GetComponent<Renderer>().material.color;
                doorScene = true;
                return;
            }

        }
    }
}
