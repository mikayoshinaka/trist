using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearch : MonoBehaviour
{
    [SerializeField] EnemyAppearColor enemyAppearColor;
    [SerializeField] GhostChange ghostChange;
    public List<GameObject> enterObject = new List<GameObject>();
    public List<GameObject> exitObject = new List<GameObject>();
    public List<GameObject> enterKey = new List<GameObject>();
    public List<GameObject> exitKey = new List<GameObject>();
    public Vector3 magnificationSearch = new Vector3(2.5f, 2.5f, 2.5f);
    public Material silhouetteMaterial;
    private Color silhouetteColor;
    private float magnificationTime = 3.0f;
    private float timer = 0.0f;
    public bool silhouette = false;
    public bool monkey = false;
    // Start is called before the first frame update
    void Start()
    {
        silhouetteColor = silhouetteMaterial.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }
        if (enemyAppearColor.doorScene == true)
        {
            return;
        }

        ExitOtherObjectInvisible(exitObject);
        EnterOtherObjectSeem(enterObject);
        ExitOtherObjectInvisible(exitKey);
        EnterOtherObjectSeem(enterKey);
        if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.T) && ghostChange.possessObject.tag == "Monkey")
        {
            silhouette = true;

        }
        if (silhouette)
        {
            ExitOtherObjectInvisible(exitKey);
            EnterOtherObjectSeem(enterKey);
            this.transform.localScale = magnificationSearch;
            timer += Time.deltaTime;
            if (timer >= magnificationTime)
            {
                timer = 0.0f;
                SilhouetteClear();
                silhouette = false;
                this.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {

        }
    }
    private void ExitOtherObjectInvisible(List<GameObject> exitObject)
    {
        for (int i = 0; i < exitObject.Count; i++)
        {
            Color enemyDefColor;
            exitObject[i].GetComponent<Renderer>().material.SetOverrideTag("RenderType", "Transparent");
            exitObject[i].GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            exitObject[i].GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            exitObject[i].GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
            exitObject[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            exitObject[i].GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
            exitObject[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            exitObject[i].GetComponent<Renderer>().material.renderQueue = 3000;
            if (exitObject[i].tag == "Enemy")
            {
                exitObject[i].GetComponent<Renderer>().materials[1].color = new Color(silhouetteColor.r, silhouetteColor.g, silhouetteColor.b, 0.0f);
            }
            enemyDefColor = exitObject[i].GetComponent<Renderer>().material.color;
            enemyDefColor.a -= Time.deltaTime;
            if (enemyDefColor.a > 0.0f)
            {
                exitObject[i].GetComponent<Renderer>().material.color = enemyDefColor;
            }
            else
            {
                exitObject[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor.r, enemyDefColor.g, enemyDefColor.b, 0.0f);
                exitObject.Remove(exitObject[i]);
            }
        }

    }
    private void EnterOtherObjectSeem(List<GameObject> enterObject)
    {
        for (int i = 0; i < enterObject.Count; i++)
        {
            Color enemyDefColor;
            enemyDefColor = enterObject[i].GetComponent<Renderer>().material.color;
            enemyDefColor.a += Time.deltaTime;
            if (silhouette)
            {
                if (enterObject[i].tag == "Enemy")
                {
                    enterObject[i].GetComponent<Renderer>().materials[1].color = new Color(silhouetteColor.r, silhouetteColor.g, silhouetteColor.b, 1.0f);
                }
            }
            if (enemyDefColor.a < 1.0f)
            {
                enterObject[i].GetComponent<Renderer>().material.color = enemyDefColor;
            }
            else
            {
                enterObject[i].GetComponent<Renderer>().material.SetOverrideTag("RenderType", "");
                enterObject[i].GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                enterObject[i].GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                enterObject[i].GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
                enterObject[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
                enterObject[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
                enterObject[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                enterObject[i].GetComponent<Renderer>().material.renderQueue = -1;
                enterObject[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor.r, enemyDefColor.g, enemyDefColor.b, 1.0f);
            }

        }

    }
    private void SilhouetteClear()
    {
        for (int i = 0; i < enterObject.Count; i++)
        {
            enterObject[i].GetComponent<Renderer>().materials[1].color = new Color(silhouetteColor.r, silhouetteColor.g, silhouetteColor.b, 0.0f);
        }
        for (int i = 0; i < exitObject.Count; i++)
        {
            exitObject[i].GetComponent<Renderer>().materials[1].color = new Color(silhouetteColor.r, silhouetteColor.g, silhouetteColor.b, 0.0f);
        }
    }

    private void KeyColorClear(List<GameObject> key)
    {
        for (int i = 0; i < key.Count; i++)
        {

            Color keyDefColor;
            keyDefColor = key[i].GetComponent<Renderer>().material.color;
            key[i].GetComponent<Renderer>().material.SetOverrideTag("RenderType", "Transparent");
            key[i].GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            key[i].GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            key[i].GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
            key[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            key[i].GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
            key[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            key[i].GetComponent<Renderer>().material.renderQueue = 3000;
            key[i].GetComponent<Renderer>().material.color = new Color(keyDefColor.r, keyDefColor.g, keyDefColor.b, 0.0f);
        }
    }
    public void OtherObjectClear()
    {
        SilhouetteClear();
        KeyColorClear(enterKey);
        KeyColorClear(exitKey);
        enterObject.Clear();
        exitObject.Clear();
        enterObject.Clear();
        exitObject.Clear();
        this.gameObject.SetActive(false);
        timer = 0.0f;
        silhouette = false;
        this.transform.localScale = new Vector3(1, 1, 1);
    }

    public void DestroyKeyClear(GameObject key)
    {
        if (enterKey.Contains(key))
        {
            enterKey.Remove(key);
        }
        if (exitKey.Contains(key))
        {
            exitKey.Remove(key);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("hit");
            if (!enterObject.Contains(other.gameObject))
            {
                enterObject.Add(other.gameObject);
                if (exitObject.Contains(other.gameObject))
                {
                    exitObject.Remove(other.gameObject);
                }
            }
        }
        if (other.tag == "Key")
        {
            Debug.Log("hit");
            if (!enterKey.Contains(other.gameObject))
            {
                enterKey.Add(other.gameObject);
                if (exitKey.Contains(other.gameObject))
                {
                    exitKey.Remove(other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (enterObject.Contains(other.gameObject))
            {
                enterObject.Remove(other.gameObject);
                if (!exitObject.Contains(other.gameObject))
                {
                    exitObject.Add(other.gameObject);
                }
            }
        }

        if (other.tag == "Key")
        {
            if (enterKey.Contains(other.gameObject))
            {
                enterKey.Remove(other.gameObject);
                if (!exitKey.Contains(other.gameObject))
                {
                    exitKey.Add(other.gameObject);
                }
            }
        }
    }
}
