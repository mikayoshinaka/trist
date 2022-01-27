using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BeforeBegin : MonoBehaviour
{
    public bool begin = false;
    public GameObject mainCamera;
    public GameObject beginFirstCamera;
    public GameObject beginSecondCamera;
    public GameObject beginFirstCameraEndPos;
    public GameObject beginSecondCameraEndPos;
    [SerializeField] private float firstCameraSpeed=1.0f;
    [SerializeField] private float secondCameraSpeed=1.0f;
    private float timer;
    [SerializeField] private float firstCameraTime=2.0f;
    [SerializeField] private float secondCameraTime=2.0f;

    public Image fadeImage;
    [SerializeField] float fadeSpeed = 1.0f;
    float red, green, blue, alfa;
    bool isFadeOut = false;
    private GameObject BGM;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        begin = true;
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;
        BGM = GameObject.Find("BGM").transform.gameObject;
        if (BGM.GetComponent<BGM>().audioSource.clip == null)
        {
            if (SceneManager.GetActiveScene().name == "MazeScene")
            {
                BGM.GetComponent<BGM>().Stage1BGM();
            }
            else
            {
                BGM.GetComponent<BGM>().Stage2BGM();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //始まる前のカメラ移動
        timer += Time.deltaTime;
        if (timer<firstCameraTime)
        {
            mainCamera.SetActive(false);
            beginFirstCamera.SetActive(true);
            beginSecondCamera.SetActive(false);
            beginFirstCamera.transform.position = Vector3.MoveTowards(beginFirstCamera.transform.position, beginFirstCameraEndPos.transform.position, firstCameraSpeed*Time.deltaTime);
        }
        else if(timer < secondCameraTime+firstCameraTime)
        {
            beginFirstCamera.SetActive(false);
            beginSecondCamera.SetActive(true);
            beginSecondCamera.transform.position = Vector3.MoveTowards(beginSecondCamera.transform.position, beginSecondCameraEndPos.transform.position, firstCameraSpeed * Time.deltaTime);
        }
        else if(isFadeOut==false)
        {
            beginSecondCamera.transform.position = Vector3.MoveTowards(beginSecondCamera.transform.position, beginSecondCameraEndPos.transform.position, firstCameraSpeed * Time.deltaTime);
            StartFadeOut();
        }
        else
        {
            StartFadeIn();
        }

    }
    void StartFadeOut()
    {
        fadeImage.enabled = true;
        alfa += Time.deltaTime * fadeSpeed;
        SetAlpha();
        if (alfa >= 1)
        {
            beginFirstCamera.SetActive(false);
            beginSecondCamera.SetActive(false);
            mainCamera.SetActive(true);
            isFadeOut = true;
        }

    }
    void StartFadeIn()
    {
        alfa -= Time.deltaTime * fadeSpeed;
        SetAlpha();
        if (alfa <= 0)
        {
            begin = false;
        }

    }
    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }
}
