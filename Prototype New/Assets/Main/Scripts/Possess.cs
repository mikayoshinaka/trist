using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Possess : MonoBehaviour
{
    public GameObject PlayerParent;
    public GameObject PlayerController;
    public GameObject[] PlayerBody;
    public GameObject possessObject;
    public GhostCatch ghostCatch;
    public bool possess;
    public List<GameObject> searchObject = new List<GameObject>();
    public List<GameObject> cooltimeObject = new List<GameObject>();
    public List<float> cool = new List<float>();
    public bool canPossess;
    public bool leave;
    public bool canPossessText;
    private bool transparent;
    private bool normal;
    private bool changeTime;
    private bool away;
    //private bool enemyNormal;
    private float setColor = 1.0f;
    [SerializeField] private float cooltime = 5.0f;
    private float speed = 2.0f;
    private float cameraFirstSpeed = 7.0f;
    private float cameraSpeed = 0.0f;
    private float cameraAccelerate = 15.0f;
    private float cameraSpeedMax = 50.0f;
    private float possessTimeMax = 10.0f;
    public float possessTime = 0.0f;
    public GameObject mainCamera;
    public GameObject relayCamera;
    private GameObject possessCamera;
    private List<Color> defColor = new List<Color>();
    private Vector3 beforePossessPos;
    [SerializeField] private Transparent transparentScript;
    public Player player;
    public enum Player
    {
        cannotLook,
        canLook,
        def
    }
    // Start is called before the first frame update
    void Start()
    {
        player = Player.def;
        changeTime = true;
        possess = false;
        canPossess = false;
        transparent = false;
        normal = false;
        away = false;
        for (int i = 0; i < PlayerBody.Length; i++)
        {

            defColor.Add(PlayerBody[i].GetComponent<Renderer>().material.color);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("BeforeBegin").GetComponent<BeforeBegin>().begin == true)
        {
            return;
        }
        switch (player)
        {
            case Player.canLook:
                FromTransparent();
                break;
            case Player.cannotLook:
                ToTransparent();
                break;
            case Player.def:
                break;
        }
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton0)) && possess == false && canPossess == false && searchObject.Count > 0
        && (!cooltimeObject.Contains(searchObject[0])) && ghostCatch.grab == false && ghostCatch.bossGrab == false && ghostCatch.mode == GhostCatch.Mode.CanGrab)
        {
            InputAndCanPossess();
            player = Player.cannotLook;
        }
        else if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton0)) && possess == true && canPossess == false && away == false)
        {
            GhostLeaveFromPossessObject();
        }
        else if (!((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton0))) && possess == true && canPossess == false)
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
        if (dis < 0.1f && cameraDis < 0.1f)
        {
            relayCamera.SetActive(false);
            possessCamera.SetActive(true);
            PlayerController.transform.parent = possessObject.gameObject.transform;
            if (possessObject.tag == "Box")
            {

            }
            cameraSpeed = 0.0f;
            cooltimeObject.Add(possessObject);
            cool.Add(cooltime);
            canPossess = false;
        }
    }
    //加速
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
        if (possessObject.tag == "Box")
        {
            dis = Vector3.Distance(PlayerController.transform.position, beforePossessPos);
            float a = MagneticAccelerate(dis);
            PlayerController.transform.position = Vector3.MoveTowards(PlayerController.transform.position, beforePossessPos, Time.deltaTime * speed * a);
        }
        else
        {
            dis = 0.0f;
        }

        if (dis < 0.1f && cameraDis < 0.1f)
        {
            PlayerController.GetComponent<CharacterMovementScript>().enabled = true;
            PlayerController.layer = LayerMask.NameToLayer("Player");

            // EnemyBehaviour プレイヤー判定用
            PlayerController.transform.Find("PlayerBody").Find("PlayerTrigger").gameObject.layer = LayerMask.NameToLayer("PlayerTrigger");


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
            player = Player.def;
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
            player = Player.def;
        }
    }
    //とりつくとき
    private void InputAndCanPossess()
    {
        PlayerController.layer = LayerMask.NameToLayer("Disappear");

        // EnemyBehaviour プレイヤー判定用
        PlayerController.transform.Find("PlayerBody").Find("PlayerTrigger").gameObject.layer = LayerMask.NameToLayer("Default");

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
    }
    //とりついたものから離れるとき
    private void GhostLeaveFromPossessObject()
    {
        player = Player.canLook;
        leave = true;
        away = true;
        relayCamera.SetActive(true);
        possessCamera.SetActive(false);
        changeTime = false;
    }




    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Box")
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
        if (other.tag == "Box")
        {

            if (searchObject.Contains(other.gameObject))
            {
                searchObject.Remove(other.gameObject);
            }
        }
    }

}