using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostChange : MonoBehaviour
{
    [SerializeField] EnemyAppearColor enemyAppearColor;
    [SerializeField] ManagementScript managementScript;
    public GameObject[] enemy;
    public GameObject PlayerParent;
    public GameObject PlayerController;
    public GameObject EnemySearch;
    public GameObject[] PlayerBody;
    public GameObject possessObject;
    public bool possess;
    public List<GameObject> searchObject = new List<GameObject>();
    public List<GameObject> cooltimeObject = new List<GameObject>();
    public List<float> cool = new List<float>();
    public List<float> setEnemyColor = new List<float>();
    public List<float> setKeyColor = new List<float>();
    public Material silhouetteMaterial;
    private Color silhouetteColor;
    public bool canPossess;
    public bool leave;
    public bool canPossessText;
    private bool transparent;
    private bool normal;
    private bool enemyTransparent;
    private bool changeTime;
    private bool away;
    //private bool enemyNormal;
    private bool once;
    private float setColor = 1.0f;
    [SerializeField] private float cooltime = 5.0f;
    private float speed = 2.0f;
    private float cameraFirstSpeed = 7.0f;
    //private float cameraFirstBackSpeed = 20.0f;
    //private float cameraFinalBackSpeed = 7.0f;
    private float cameraSpeed = 0.0f;
    private float cameraAccelerate = 15.0f;
    private float cameraSpeedMax = 50.0f;
    private float possessTimeMax = 10.0f;
    public float possessTime = 0.0f;
    public GameObject mainCamera;
    public GameObject relayCamera;
    private GameObject possessCamera;
    private List<Color> defColor = new List<Color>();
    private List<Color> enemyDefColor = new List<Color>();
    private Vector3 beforePossessPos;

    // private const float pi = 3.141592653589793238f;
    //private int m = 100;
    ////比例定数
    //private float k;
    //private float F;
    ////点電荷
    //private float Q = 0.25f;

    //private float olor = 0.0f;
    //private int lor = 0;
    // Start is called before the first frame update
    void Start()
    {

        changeTime = true;
        possess = false;
        canPossess = false;
        transparent = false;
        normal = false;
        enemyTransparent = false;
        //enemyNormal = false;
        once = false;
        away = false;
        silhouetteColor = silhouetteMaterial.color;
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
            enemy[i].GetComponent<Renderer>().materials[1].color = new Color(silhouetteColor.r, silhouetteColor.g, silhouetteColor.b, 0.0f);
            setEnemyColor.Add(enemy[i].GetComponent<Renderer>().material.color.a);
        }
       
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

        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1)) && possess == false && canPossess == false && searchObject.Count > 0 && (!cooltimeObject.Contains(searchObject[0])) && normal == false)
        {
            InputAndCanPossess();
        }
        else if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1)) && possess == true && canPossess == false && transparent == false && away == false)
        {
            GhostLeaveFromPossessObject();
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
        if (transparent)
        {
            ToTransparent();
        }
        if (normal)
        {
            FromTransparent();
        }
        if (enemyTransparent)
        {
            EnemyTransparent();
        }
        //if (enemyNormal)
        //{
        //    EnemycanLook();
        //}
        CanPossessText();
    }
    //とりつく動き
    private void ToPossess(Vector3 toPos)
    {
        float dis = Vector3.Distance(PlayerController.transform.position, toPos);
        float cameraDis = Vector3.Distance(relayCamera.transform.position, toPos);
        if (cameraSpeed < cameraSpeedMax)
        {
            cameraSpeed += Time.deltaTime * cameraAccelerate;
        }
        //加速度
        float a = MagneticAccelerate(dis);
        relayCamera.transform.position = Vector3.MoveTowards(relayCamera.transform.position, toPos, Time.deltaTime * ((cameraSpeed * cameraSpeed) + cameraFirstSpeed));
        PlayerController.transform.position = Vector3.MoveTowards(PlayerController.transform.position, toPos, Time.deltaTime * speed * a);
        if (dis < 0.03f && cameraDis < 0.03f)
        {
            EnemySearch.SetActive(true);
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

    private float MagneticAccelerate(float dis)
    {
        //比例定数
        float k = 6.33f * 10f * 10f * 10f * 10f;
        float F;
        int m = 100;
        //点電荷
        float Q = 0.25f;
        F = k * Q / (dis * dis);
        //加速度
        float a = F / m;
        return a;
    }
    //離れる動き
    private void FromPossess()
    {
        float dis;
        float cameraDis = Vector3.Distance(mainCamera.transform.position, relayCamera.transform.position);
        if (cameraSpeed < cameraSpeedMax)
        {
            cameraSpeed += Time.deltaTime * cameraAccelerate;
        }
        relayCamera.transform.position = Vector3.MoveTowards(relayCamera.transform.position, mainCamera.transform.position, Time.deltaTime * ((cameraSpeed * cameraSpeed) + cameraFirstSpeed));
        relayCamera.transform.rotation = mainCamera.transform.rotation;
        //if ( (cameraFirstBackSpeed - (cameraSpeed * cameraSpeed))*Time.deltaTime> cameraFinalBackSpeed)
        //{
        //    cameraSpeed += Time.deltaTime;
        //    relayCamera.transform.position = Vector3.MoveTowards(relayCamera.transform.position, mainCamera.transform.position, (cameraFirstBackSpeed-(cameraSpeed * cameraSpeed))*Time.deltaTime);
        //}
        //else
        //{
        //    relayCamera.transform.position = Vector3.MoveTowards(relayCamera.transform.position, mainCamera.transform.position,cameraFinalBackSpeed*Time.deltaTime);
        //}
        if (possessObject.tag == "Monkey")
        {
            dis = 0.0f;
        }
        else if (possessObject.tag == "Box")
        {
            dis = Vector3.Distance(PlayerController.transform.position, beforePossessPos);
            float a = MagneticAccelerate(dis);
            PlayerController.transform.position = Vector3.MoveTowards(PlayerController.transform.position, beforePossessPos, Time.deltaTime * speed * a);
        }
        else
        {
            dis = 0.0f;
        }

        if (dis < 0.03f && cameraDis < 0.03f)
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
            away = false;
            mainCamera.SetActive(true);
            relayCamera.SetActive(false);
            changeTime = true;
            possessTime = 0.0f;
            cameraSpeed = 0.0f;
        }

    }
    //とりつく時間
    private void GhostChangeTime()
    {
        possessTime += Time.deltaTime;
        if (possessTime >= possessTimeMax && changeTime == true)
        {
            GhostLeaveFromPossessObject();
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
    //敵が透明になる
    private void EnemyTransparent()
    {
        bool enemyBecameTransparent = false;
        if (once == false)
        {
            NowEnemyTransparentColor();
            once = true;
        }
        for (int i = 0; i < enemy.Length; i++)
        {
            setEnemyColor[i] -= Time.deltaTime;
            enemy[i].GetComponent<Renderer>().material.SetOverrideTag("RenderType", "Transparent");
            enemy[i].GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            enemy[i].GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            enemy[i].GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
            enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            enemy[i].GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
            enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            enemy[i].GetComponent<Renderer>().material.renderQueue = 3000;
            enemy[i].GetComponent<Renderer>().materials[1].color = new Color(silhouetteColor.r, silhouetteColor.g, silhouetteColor.b, 0.0f);
            if (setEnemyColor[i] > 0.0f)
            {
                enemy[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor[i].r, enemyDefColor[i].g, enemyDefColor[i].b, setEnemyColor[i]);
            }
            else
            {
                enemy[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor[i].r, enemyDefColor[i].g, enemyDefColor[i].b, 0.0f);
            }
        }

        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i].GetComponent<Renderer>().material.color.a == 0.0f)
            {
                enemyBecameTransparent = true;
            }
            else
            {
                enemyBecameTransparent = false;
                break;
            }
        }
        if (enemyBecameTransparent)
        {
            for (int i = 0; i < enemy.Length; i++)
            {
                setEnemyColor[i] = 0.0f;
            }
            enemyTransparent = false;
            once = false;
        }
    }

    //private void EnemycanLook()
    //{
    //    olor += Time.deltaTime;
    //    for (int i = 0; i < enemy.Length; i++)
    //    {
    //        if (olor < 1.0f)
    //        {
    //            enemy[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor[i].r, enemyDefColor[i].g, enemyDefColor[i].b, olor);
    //        }
    //        else
    //        {

    //            enemy[i].GetComponent<Renderer>().material.SetOverrideTag("RenderType", "");
    //            enemy[i].GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
    //            enemy[i].GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
    //            enemy[i].GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
    //            enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
    //            enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
    //            enemy[i].GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    //            enemy[i].GetComponent<Renderer>().material.renderQueue = -1;
    //            enemy[i].GetComponent<Renderer>().material.color = new Color(enemyDefColor[i].r, enemyDefColor[i].g, enemyDefColor[i].b, 1.0f);
    //            lor++;
    //        }
    //    }
    //    if (lor >= enemy.Length)
    //    {
    //        lor = 0;
    //        olor = 0.0f;
    //        enemyNormal = false;
    //    }
    //}
    //透明になる前の敵の透明度
    private void NowEnemyTransparentColor()
    {
        for (int i = 0; i < setEnemyColor.Count; i++)
        {
            setEnemyColor[i] = enemy[i].GetComponent<Renderer>().material.color.a;
        }
    }
    //透明になる
    private void ToTransparent()
    {
        bool becameTransparent = false;
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

            }
        }
        for (int i = 0; i < PlayerBody.Length; i++)
        {
            if (PlayerBody[i].GetComponent<Renderer>().material.color.a == 0.0f)
            {
                becameTransparent = true;
            }
            else
            {
                becameTransparent = false;
                break;
            }
        }
        if (becameTransparent)
        {
            setColor = 0.0f;
            transparent = false;
        }
    }
    //透明から元に戻る
    private void FromTransparent()
    {
        bool def = false;
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
            }
        }
        for (int i = 0; i < PlayerBody.Length; i++)
        {
            if (PlayerBody[i].GetComponent<Renderer>().material.color.a == 1.0f)
            {
                def = true;
            }
            else
            {
                def = false;
                break;
            }
        }
        if (def)
        {
            setColor = 1.0f;
            normal = false;
        }
    }
    //とりつくとき
    private void InputAndCanPossess()
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
        managementScript.PlusPossessCount();
        //enemyNormal = true;
    }
    //とりついたものから離れるとき
    private void GhostLeaveFromPossessObject()
    {
        leave = true;
        normal = true;
        enemyTransparent = true;
        away = true;
        EnemySearch.GetComponent<EnemySearch>().OtherObjectClear();
        relayCamera.SetActive(true);
        possessCamera.SetActive(false);
        if (possessObject.tag == "Monkey")
        {
            relayCamera.transform.position = possessObject.transform.position;
        }
        changeTime = false;
    }

    private void CanPossessText()
    {
        if (possess == false && canPossess == false && searchObject.Count > 0 && (!cooltimeObject.Contains(searchObject[0])) && normal == false)
        {
            canPossessText = true;
        }
        else
        {
            canPossessText = false;
        }
    }
    //即時切替
    public void AttackTransparent(GameObject attackBody)
    {
        Color defColor = attackBody.GetComponent<Renderer>().material.color;
        attackBody.GetComponent<Renderer>().material.SetOverrideTag("RenderType", "");
        attackBody.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        attackBody.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        attackBody.GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
        attackBody.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
        attackBody.GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
        attackBody.GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        attackBody.GetComponent<Renderer>().material.renderQueue = -1;
        attackBody.GetComponent<Renderer>().material.color = new Color(defColor.r, defColor.g, defColor.b, 1.0f);
    }
    //即時切替
    public void AttackedTransparent(GameObject attackedBody)
    {
        if (EnemySearch.activeSelf==false) {
            Color defColor = attackedBody.GetComponent<Renderer>().material.color;
            attackedBody.GetComponent<Renderer>().material.SetOverrideTag("RenderType", "Transparent");
            attackedBody.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            attackedBody.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            attackedBody.GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
            attackedBody.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            attackedBody.GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
            attackedBody.GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            attackedBody.GetComponent<Renderer>().material.renderQueue = 3000;
            attackedBody.GetComponent<Renderer>().material.color = new Color(defColor.r, defColor.g, defColor.b, 0.0f);
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
