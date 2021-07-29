using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostChange : MonoBehaviour
{
    public GameObject PlayerParent;
    public GameObject PlayerController;
    public GameObject PlayerBody;
    private GameObject possessObject;
    private bool possess = false;
    public List<GameObject> searchObject = new List<GameObject>();
    public List<GameObject> cooltimeObject = new List<GameObject>();
    public List<float> cool = new List<float>();
    private bool canPossess = false;
    private bool  leave= false;
    private bool transparent = false;
    private bool normal = false;
    private float cooltime = 10.0f;
    private float speed = 2.0f;
    private float possessTimeMax = 7.0f;
    private float possessTime = 0.0f;
    public GameObject mainCamera;
    private  GameObject possessCamera;
    private Color defColor;
   // private const float pi = 3.141592653589793238f;
    private int m = 100;
    //比例定数
    private float k;
    private float F;
    //点電荷
    private float Q=0.25f;
    // Start is called before the first frame update
    void Start()
    {
        k = 6.33f*10f*10f*10f*10f;
        defColor = PlayerBody.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1)) && possess==false && canPossess == false && searchObject.Count>0&&(!cooltimeObject.Contains(searchObject[0]))&&normal==false) 
        {
            //仮に変更しただけ
            PlayerController.layer = LayerMask.NameToLayer("Disappear");

            PlayerController.GetComponent<CharacterMovementScript>().enabled = false;
            possessObject = searchObject[0];
            possessCamera = possessObject.transform.GetChild(0).gameObject;
            possess = true;
            canPossess = true;
            transparent = true;
        }
        else if((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1)) && possess == true && canPossess == false&&transparent==false)
        {
            leave = true;
            normal = true;
        }
        else if(!((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1))) && possess == true && canPossess == false)
        {
            GhostChangeTime();
        }
        if(canPossess)
        {
            Vector3 possessPos =possessObject.transform.position;
            ToPossess(possessPos);
        }
        if(leave)
        {
            FromPossess();
        }
        if(searchObject.Count>1)
        {
            Sort();
        }
        if(cool.Count>0)
        {
            TimeCount();
        }
        if(transparent==true)
        {
            ToTransparent();
        }
        if(normal==true)
        {
            FromTransparent();
        }
    }
    //とりつく動き
    private void ToPossess(Vector3 toPos)
    {
        float dis = Vector3.Distance(PlayerController.transform.position, toPos);
        F =k*Q/(dis*dis);
        //加速度
        float a = F / m;
        PlayerController.transform.position = Vector3.MoveTowards(PlayerController.transform.position, toPos, Time.deltaTime * speed*a);
        if (dis<0.03f)
        {
            mainCamera.SetActive(false);
            possessCamera.SetActive(true);
            PlayerController.transform.parent = possessObject.gameObject.transform;
            if (possessObject.tag == "Monkey") {
                possessObject.GetComponent<MonkeyDoll>().enabled = true;
            }
            else if (possessObject.tag == "Box")
            {

            }
            cooltimeObject.Add(possessObject);
            cool.Add(cooltime);
            canPossess = false;
        }
    }
    //離れる動き
    private void FromPossess()
    {
        PlayerController.GetComponent<CharacterMovementScript>().enabled = true;
        if (possessObject.tag == "Monkey")
        {
            possessObject.GetComponent<MonkeyDoll>().enabled = false;
        }
        else if(possessObject.tag=="Box")
        {

        }
        PlayerController.transform.parent = PlayerParent.transform;
        possess = false;
        leave = false;
        mainCamera.SetActive(true);
        possessCamera.SetActive(false);
        possessTime = 0.0f;
    }
    //とりつく時間
    private void GhostChangeTime()
    {
        possessTime += Time.deltaTime;
        if(possessTime>=possessTimeMax)
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
            mainCamera.SetActive(true);
            possessCamera.SetActive(false);
            possessTime = 0.0f;
        }
    }
    //取り付ける者の範囲内距離ソート
    private void Sort()
    {
        GameObject temp;
        for(int i=0;i<searchObject.Count-1 ;i++ )
        {
            for (int j=searchObject.Count-1; j>i;j-- ) 
            {
                if (Vector3.Distance(PlayerController.transform.position, searchObject[j-1].transform.position)
                    > Vector3.Distance(PlayerController.transform.position, searchObject[j].transform.position)) {
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
            if(cool[i]<=0)
            {
                cooltimeObject.Remove(cooltimeObject[i]);
                cool.Remove(cool[i]);
            }
        }
    }
    //透明になる
    private void ToTransparent()
    {
        PlayerBody.GetComponent<Renderer>().material.SetOverrideTag("RenderType", "Transparent");
        PlayerBody.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        PlayerBody.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        PlayerBody.GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
        PlayerBody.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
        PlayerBody.GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
        PlayerBody.GetComponent<Renderer>().material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        PlayerBody.GetComponent<Renderer>().material.renderQueue = 3000;
        if (PlayerBody.GetComponent<Renderer>().material.color.a>0.0f) {
            PlayerBody.GetComponent<Renderer>().material.color -= new Color(0.0f, 0.0f, 0.0f, Time.deltaTime);
        }
        else
        {
            PlayerBody.GetComponent<Renderer>().material.color = new Color(defColor.r, defColor.g, defColor.b, 0.0f);
            transparent = false;
        }

    }
    //透明から元に戻る
    private void FromTransparent()
    {
        if (PlayerBody.GetComponent<Renderer>().material.color.a < 1.0f)
        {
            PlayerBody.GetComponent<Renderer>().material.color += new Color(0.0f, 0.0f, 0.0f, Time.deltaTime);
        }
        else
        {
            PlayerBody.GetComponent<Renderer>().material.SetOverrideTag("RenderType", "");
            PlayerBody.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            PlayerBody.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            PlayerBody.GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
            PlayerBody.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            PlayerBody.GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
            PlayerBody.GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            PlayerBody.GetComponent<Renderer>().material.renderQueue = -1;
            PlayerBody.GetComponent<Renderer>().material.color = new Color(defColor.r, defColor.g, defColor.b, 1.0f);
            normal = false;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monkey" || other.tag=="Box")
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
