using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DollSave : MonoBehaviour
{
    [SerializeField] private GameObject catchArea;
    public List<GameObject> dolls = new List<GameObject>();
    public List<GameObject> seePoint = new List<GameObject>();
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject endCamera;
    [SerializeField] private GameObject endScene;
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject playerController;
    [SerializeField] private GameObject endPosition;
    [SerializeField] private Transform clearSaveBoxPos;
    public Text text;
    bool within;
    bool shoot;
    public bool bossIn;
    //int catchPoint;
    int i;
    [SerializeField] private float zoomTime = 2.0f;
    private float timer;
    private float time;
    private Animator anim;

    public Image fadeImage;
    [SerializeField] float fadeSpeed = 1.0f;
    float red, green, blue, alfa;
    public bool isFadeOut = false;
    bool  beginCount = false;
    // Start is called before the first frame update
    void Start()
    {
        bossIn = false;
        within = false;
        //catchPoint = 0;
        timer = 0.0f;
        time = 0.0f;
        shoot = false;
        i = 0;
        anim = GameObject.Find("boxbig").GetComponent<Animator>();

        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton0)) && within == true && catchArea.GetComponent<GhostCatch>().mode == GhostCatch.Mode.Carry)
        {
            catchArea.GetComponent<GhostCatch>().mode = GhostCatch.Mode.Shoot;
            GameObject.Find("Enemies").GetComponent<EnemiesManager>().enemyMode = EnemiesManager.EnemyMode.Mode_Defensive;
        }
        if(bossIn&&!(GameObject.Find("CatchArea").GetComponent<GhostCatch>().mode==GhostCatch.Mode.Shoot))
        {
            if (isFadeOut == false)
            {
                StartFadeOut();
            }
            else if (isFadeOut == true && beginCount == false)
            {
                StartFadeIn();
            }
            else
            {
                timer += Time.deltaTime;
                endCamera.SetActive(true);
                AnimStart();
                if (timer > zoomTime)
                {
                    //16è„å¿
                    if (i < dolls.Count)
                    {
                        dolls[i].SetActive(true);
                        dolls[i].transform.parent = null;
                        Vector3 p1Pos = Vector3.zero;
                        Vector3 p2Pos = Vector3.zero;
                        BezierCoordinate(this.transform.position, ref p1Pos, ref p2Pos, seePoint[i].transform.position);
                        SuckedIntoBox(this.transform.position, p1Pos, p2Pos, seePoint[i].transform.position, i);
                        dolls[i].transform.eulerAngles = new Vector3(0, 90, 0);
                        if (shoot == true)
                        {
                            i += 1;
                            shoot = false;
                            time = 0.0f;
                        }
                    }
                    else
                    {
                        endScene.SetActive(true);
                        endUI.SetActive(true);
                    }
                }
            }
        }
        else if (bossIn==false&&GameObject.Find("Timer").GetComponent<GameTimer>().countdown <= 0)
        {
            StartFadeOut();
        }
    }
    //à√Ç≠Ç∑ÇÈ
    void StartFadeOut()
    {
        fadeImage.enabled = true;
        alfa += Time.deltaTime * fadeSpeed;
        SetAlpha();
        if (alfa >= 1)
        {
            if(bossIn)
            {
                isFadeOut = true;
                this.gameObject.transform.position = clearSaveBoxPos.position;
                playerController.transform.position = endPosition.transform.position;
                playerController.transform.LookAt(this.transform.position);
                GameObject.Find("ClearScene").GetComponent<ClearScene>().ClearScenePos();
                //Ç∆ÇËÇ†Ç¶Ç∏
                PlayerPrefs.SetInt("dollCount",dolls.Count);
                SceneManager.LoadScene("Result");
            }
            else
            SceneManager.LoadScene("GameOver");
        }

    }
    void StartFadeIn()
    {
        alfa -= Time.deltaTime * fadeSpeed;
        SetAlpha();
        if (alfa <= 0)
        {
            beginCount = true;
        }

    }
    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }
    //î†Ç…êlå`í«â¡
    public void DollAdd(GameObject doll, int count)
    {
        dolls.Add(doll);
        //catchPoint += count * 100 + (count - 1) * 60;
        //text.text = catchPoint + "Point";
        anim.SetBool("open",false);
    }
    public void AnimStart()
    {
        anim.SetBool("open", true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBody")
        {
            within = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerBody")
        {
            within = false;
        }
    }


    private void BezierCoordinate(Vector3 startPos, ref Vector3 p1, ref Vector3 p2, Vector3 endPos)
    {
        //p1 = new Vector3((startPos.x + ((startPos.x + endPos.x) / 2.0f)) / 2.0f, 6.0f, (startPos.z + ((startPos.z + endPos.z) / 2.0f)) / 2.0f);
        //p2 = new Vector3((endPos.x + ((startPos.x + endPos.x) / 2.0f)) / 2.0f, 6.0f, (endPos.z + ((startPos.z + endPos.z) / 2.0f)) / 2.0f);
        //p1 = new Vector3(((startPos.x + endPos.x) / 2.0f), 6.0f, ((startPos.z + endPos.z) / 2.0f));
        //p2 = new Vector3(((startPos.x + endPos.x) / 2.0f), 6.0f, ((startPos.z + endPos.z) / 2.0f));
        //p1 = new Vector3((endPos.x + ((startPos.x + endPos.x) / 2.0f)) / 2.0f, 4.0f, (endPos.z + ((startPos.z + endPos.z) / 2.0f)) / 2.0f);
        //p1 = new Vector3(player.transform.position.x, 10.0f, player.transform.position.z);
        //p2 = new Vector3(player.transform.position.x, 10.0f, player.transform.position.z);
        p1 = new Vector3(0.0f, 6.0f, 0.0f);
        p2 = new Vector3(0.0f, 6.0f, 0.0f);
    }

    public void SuckedIntoBox(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int i)
    {

        Vector3 b0 = Vector3.zero;
        Vector3 b1 = Vector3.zero;
        Vector3 b2 = Vector3.zero;
        Vector3 b3 = Vector3.zero;
        float ax = 0.0f, ay = 0.0f, az = 0.0f;
        float bx = 0.0f, by = 0.0f, bz = 0.0f;
        float cx = 0.0f, cy = 0.0f, cz = 0.0f;
        Vector3 vec = GetPointAtTime(time, ref ax, ref ay, ref az, ref bx, ref by, ref bz, ref cx, ref cy, ref cz,
                                   ref p0, ref p1, ref p2, ref p3, ref b0, ref b1, ref b2, ref b3);
        dolls[i].transform.position = vec;
        time += Time.deltaTime;
        if (time > 1.0f)
        {
            shoot = true;
        }
    }

    private Vector3 GetPointAtTime(float t, ref float ax, ref float ay, ref float az, ref float bx, ref float by, ref float bz, ref float cx, ref float cy, ref float cz,
                                   ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector3 b0, ref Vector3 b1, ref Vector3 b2, ref Vector3 b3)
    {
        CheckConstant(ref ax, ref ay, ref az, ref bx, ref by, ref bz, ref cx, ref cy, ref cz, ref p0, ref p1, ref p2, ref p3, ref b0, ref b1, ref b2, ref b3);
        float t2 = t * t;
        float t3 = t * t * t;
        float x = ax * t3 + bx * t2 + cx * t + p0.x;
        float y = ay * t3 + by * t2 + cy * t + p0.y;
        float z = az * t3 + bz * t2 + cz * t + p0.z;
        return new Vector3(x, y, z);
    }

    private void SetConstant(ref float ax, ref float ay, ref float az, ref float bx, ref float by, ref float bz, ref float cx, ref float cy, ref float cz, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        cx = 3.0f * ((p0.x + p1.x) - p0.x);
        bx = 3.0f * ((p3.x + p2.x) - (p0.x + p1.x)) - cx;
        ax = p3.x - p0.x - cx - bx;
        cy = 3.0f * ((p0.y + p1.y) - p0.y);
        by = 3.0f * ((p3.y + p2.y) - (p0.y + p1.y)) - cy;
        ay = p3.y - p0.y - cy - by;
        cz = 3.0f * ((p0.z + p1.z) - p0.z);
        bz = 3.0f * ((p3.z + p2.z) - (p0.z + p1.z)) - cz;
        az = p3.z - p0.z - cz - bz;

    }

    private void CheckConstant(ref float ax, ref float ay, ref float az, ref float bx, ref float by, ref float bz, ref float cx, ref float cy, ref float cz,
                               ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector3 b0, ref Vector3 b1, ref Vector3 b2, ref Vector3 b3)
    {
        if (p0 != b0 || p1 != b1 || p2 != b2 || p3 != b3)
        {
            SetConstant(ref ax, ref ay, ref az, ref bx, ref by, ref bz, ref cx, ref cy, ref cz, p0, p1, p2, p3);
            b0 = p0;
            b1 = p1;
            b2 = p2;
            b3 = p3;
        }
    }
}
