using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostChange : MonoBehaviour
{
    public GameObject[] enemy;
    public GameObject PlayerParent;
    public GameObject PlayerController;
    public GameObject[] PlayerBody;
    public GameObject possessObject;
    public bool possess = false;
    public List<GameObject> searchObject = new List<GameObject>();
    public List<GameObject> cooltimeObject = new List<GameObject>();
    public List<float> cool = new List<float>();
    private bool canPossess = false;
    private bool leave = false;
    private bool transparent = false;
    private bool normal = false;
    private bool enemyTransparent = false;
    private bool enemyNormal = false;
    private float setColor = 1.0f;
    private float setEnemyColor = 0.0f;
    private int enemyColor = 0;
    private int playerColor = 0;
    private float cooltime = 10.0f;
    private float speed = 2.0f;
    private float cameraFirstSpeed = 7.0f;
    private float cameraSpeed = 0.0f;
    private float cameraAccelerate = 15.0f;
    private float cameraSpeedMax = 50.0f;
    private float possessTimeMax = 7.0f;
    private float possessTime = 0.0f;
    public GameObject mainCamera;
    public GameObject relayCamera;
    private GameObject possessCamera;
    private List<Color> defColor = new List<Color>();
    private List<Color> enemyDefColor = new List<Color>();
    private Vector3 beforePossessPos;
    // private const float pi = 3.141592653589793238f;
    private int m = 100;
    //比例定数
    private float k;
    private float F;
    //点電荷
    private float Q = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        k = 6.33f * 10f * 10f * 10f * 10f;
        for (int i = 0; i < PlayerBody.Length; i++)
        {
            defColor.Add(PlayerBody[i].GetComponent<Renderer>().material.color);
        }
        for (int i = 0; i < enemy.Length; i++)
        {
            enemyDefColor.Add(enemy[i].GetComponent<Renderer>().material.color);
        }
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].GetComponent<Renderer>().material.SetOverrideTag("RenderType", "Transparent");
            enemy[i].GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            enemy[i].GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            enemy[i].GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
            enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            enemy[i].GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
            enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            enemy[i].GetComponent<Renderer>().material.renderQueue = 3000;
            enemy[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor[i].r, enemyDefColor[i].g, enemyDefColor[i].b, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1)) && possess == false && canPossess == false && searchObject.Count > 0 && (!cooltimeObject.Contains(searchObject[0])) && normal == false)
        {
            PlayerController.layer = LayerMask.NameToLayer("Disappear");

            PlayerController.GetComponent<CharacterMovementScript>().enabled = false;
            possessObject = searchObject[0];
            int objCount = possessObject.transform.childCount;
            for (int i = 0; i < objCount; i++)
            {
                if (possessObject.transform.GetChild(i).gameObject.GetComponent<Camera>())
                {
                    possessCamera = possessObject.transform.GetChild(i).gameObject;
                    break;
                }
            }
            relayCamera.SetActive(true);
            mainCamera.SetActive(false);
            beforePossessPos = PlayerController.transform.position;
            relayCamera.transform.position = mainCamera.transform.position;
            relayCamera.transform.rotation = mainCamera.transform.rotation;
            possess = true;
            canPossess = true;
            transparent = true;
            enemyNormal = true;
        }
        else if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1)) && possess == true && canPossess == false && transparent == false)
        {
            leave = true;
            normal = true;
            enemyTransparent = true;
        }
        else if (!((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1))) && possess == true && canPossess == false)
        {
            GhostChangeTime();
        }
        if (canPossess)
        {
            Vector3 possessPos = new Vector3(possessObject.transform.position.x, PlayerController.transform.position.y, possessObject.transform.position.z);
            ToPossess(possessPos);
        }
        if (leave)
        {
            FromPossess();
        }
        if (searchObject.Count > 1)
        {
            Sort();
        }
        if (cool.Count > 0)
        {
            TimeCount();
        }
        if (transparent == true)
        {
            ToTransparent();
        }
        if (normal == true)
        {
            FromTransparent();
        }
        if (enemyTransparent)
        {
            EnemyTransparent();
        }
        if (enemyNormal)
        {
            EnemycanLook();
        }
    }
    //とりつく動き
    private void ToPossess(Vector3 toPos)
    {
        float dis = Vector3.Distance(PlayerController.transform.position, toPos);
        float cameraDis = Vector3.Distance(relayCamera.transform.position, toPos);
        F = k * Q / (dis * dis);
        if (cameraSpeed < cameraSpeedMax)
        {
            cameraSpeed += Time.deltaTime * cameraAccelerate;
        }
        //加速度
        float a = F / m;
        relayCamera.transform.position = Vector3.MoveTowards(relayCamera.transform.position, toPos, Time.deltaTime * ((cameraSpeed * cameraSpeed) + cameraFirstSpeed));
        PlayerController.transform.position = Vector3.MoveTowards(PlayerController.transform.position, toPos, Time.deltaTime * speed * a);
        if (dis < 0.03f && cameraDis < 0.03f)
        {
            relayCamera.SetActive(false);
            possessCamera.SetActive(true);
            PlayerController.transform.parent = possessObject.gameObject.transform;
            if (possessObject.tag == "Monkey")
            {
                possessObject.GetComponent<MonkeyDoll>().enabled = true;
            }
            else if (possessObject.tag == "Box")
            {

            }
            cameraSpeed = 0.0f;
            cooltimeObject.Add(possessObject);
            cool.Add(cooltime);
            canPossess = false;
        }
    }
    //離れる動き
    private void FromPossess()
    {
        float dis;
        if (possessObject.tag == "Monkey")
        {
            dis = 0.0f;
        }
        else if (possessObject.tag == "Box")
        {
            dis = Vector3.Distance(PlayerController.transform.position, beforePossessPos);
            F = k * Q / (dis * dis);
            float a = F / m;
            PlayerController.transform.position = Vector3.MoveTowards(PlayerController.transform.position, beforePossessPos, Time.deltaTime * speed * a);
        }
        else
        {
            dis = 0.0f;
        }
        if (dis < 0.03f)
        {
            PlayerController.GetComponent<CharacterMovementScript>().enabled = true;
            PlayerController.layer = LayerMask.NameToLayer("Player");
            if (possessObject.tag == "Monkey")
            {
                possessObject.GetComponent<MonkeyDoll>().enabled = false;
            }
            else if (possessObject.tag == "Box")
            {

            }
            PlayerController.transform.parent = PlayerParent.transform;
            possess = false;
            leave = false;
            mainCamera.SetActive(true);
            possessCamera.SetActive(false);
            possessTime = 0.0f;
        }

    }
    //とりつく時間
    private void GhostChangeTime()
    {
        possessTime += Time.deltaTime;
        if (possessTime >= possessTimeMax)
        {
            PlayerController.GetComponent<CharacterMovementScript>().enabled = true;
            if (possessObject.tag == "Monkey")
            {
                possessObject.GetComponent<MonkeyDoll>().enabled = false;
            }
            else if (possessObject.tag == "Box")
            {

            }
            PlayerController.transform.parent = PlayerParent.transform;
            possess = false;
            normal = true;
            enemyTransparent = true;
            mainCamera.SetActive(true);
            possessCamera.SetActive(false);
            possessTime = 0.0f;
        }
    }
    //取り付ける者の範囲内距離ソート
    private void Sort()
    {
        GameObject temp;
        for (int i = 0; i < searchObject.Count - 1; i++)
        {
            for (int j = searchObject.Count - 1; j > i; j--)
            {
                if (Vector3.Distance(PlayerController.transform.position, searchObject[j - 1].transform.position)
                    > Vector3.Distance(PlayerController.transform.position, searchObject[j].transform.position))
                {
                    temp = searchObject[j - 1];
                    searchObject[j - 1] = searchObject[j];
                    searchObject[j] = temp;
                }
            }

        }
    }
    //とりついたものの次に取り付けるまでの時間計測
    private void TimeCount()
    {
        for (int i = 0; i < cool.Count; i++)
        {
            cool[i] -= Time.deltaTime;
            if (cool[i] <= 0)
            {
                cooltimeObject.Remove(cooltimeObject[i]);
                cool.Remove(cool[i]);
            }
        }
    }
    private void EnemyTransparent()
    {
        setEnemyColor -= Time.deltaTime;
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].GetComponent<Renderer>().material.SetOverrideTag("RenderType", "Transparent");
            enemy[i].GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            enemy[i].GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            enemy[i].GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
            enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            enemy[i].GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
            enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            enemy[i].GetComponent<Renderer>().material.renderQueue = 3000;
            if (setEnemyColor > 0.0f)
            {
                enemy[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor[i].r, enemyDefColor[i].g, enemyDefColor[i].b, setEnemyColor);
            }
            else
            {
                enemy[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor[i].r, enemyDefColor[i].g, enemyDefColor[i].b, 0.0f);
                enemyColor++;
            }
        }
        if (enemyColor >= enemy.Length)
        {
            enemyColor = 0;
            setEnemyColor = 0.0f;
            enemyTransparent = false;
        }
    }
    private void EnemycanLook()
    {
        setEnemyColor += Time.deltaTime;
        for (int i = 0; i < enemy.Length; i++)
        {
            if (setEnemyColor < 1.0f)
            {
                enemy[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor[i].r, enemyDefColor[i].g, enemyDefColor[i].b, setEnemyColor);
            }
            else
            {

                enemy[i].GetComponent<Renderer>().material.SetOverrideTag("RenderType", "");
                enemy[i].GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                enemy[i].GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                enemy[i].GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
                enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
                enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
                enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                enemy[i].GetComponent<Renderer>().material.renderQueue = -1;
                enemy[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor[i].r, enemyDefColor[i].g, enemyDefColor[i].b, 1.0f);
                enemyColor++;
            }
        }
        if (enemyColor >= enemy.Length)
        {
            enemyColor = 0;
            setEnemyColor = 1.0f;
            enemyNormal = false;
        }
    }
    //透明になる
    private void ToTransparent()
    {
        setColor -= Time.deltaTime;
        for (int i = 0; i < PlayerBody.Length; i++)
        {
            PlayerBody[i].GetComponent<Renderer>().material.SetOverrideTag("RenderType", "Transparent");
            PlayerBody[i].GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            PlayerBody[i].GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            PlayerBody[i].GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
            PlayerBody[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            PlayerBody[i].GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
            PlayerBody[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            PlayerBody[i].GetComponent<Renderer>().material.renderQueue = 3000;

            if (setColor > 0.0f)
            {
                PlayerBody[i].GetComponent<Renderer>().material.color = new Color(defColor[i].r, defColor[i].g, defColor[i].b, setColor);
            }
            else
            {
                PlayerBody[i].GetComponent<Renderer>().material.color = new Color(defColor[i].r, defColor[i].g, defColor[i].b, 0.0f);
                playerColor++;
            }
        }
        if (playerColor >= PlayerBody.Length)
        {
            playerColor = 0;
            setColor = 0.0f;
            transparent = false;
        }
    }
    //透明から元に戻る
    private void FromTransparent()
    {
        setColor += Time.deltaTime;
        for (int i = 0; i < PlayerBody.Length; i++)
        {
            if (setColor < 1.0f)
            {
                PlayerBody[i].GetComponent<Renderer>().material.color = new Color(defColor[i].r, defColor[i].g, defColor[i].b, setColor);
            }
            else
            {
                PlayerBody[i].GetComponent<Renderer>().material.SetOverrideTag("RenderType", "");
                PlayerBody[i].GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                PlayerBody[i].GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                PlayerBody[i].GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
                PlayerBody[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
                PlayerBody[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
                PlayerBody[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                PlayerBody[i].GetComponent<Renderer>().material.renderQueue = -1;
                PlayerBody[i].GetComponent<Renderer>().material.color = new Color(defColor[i].r, defColor[i].g, defColor[i].b, 1.0f);
                playerColor++;
            }
        }
        if (playerColor >= PlayerBody.Length)
        {
            playerColor = 0;
            setColor = 1.0f;
            normal = false;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monkey" || other.tag == "Box")
        {

            if (!searchObject.Contains(other.gameObject))
            {
                searchObject.Add(other.gameObject);
            }


        }
        else
        {
            if (searchObject.Contains(other.gameObject))
            {
                searchObject.Remove(other.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Monkey" || other.tag == "Box")
        {

            if (searchObject.Contains(other.gameObject))
            {
                searchObject.Remove(other.gameObject);
            }
        }
    }

}
