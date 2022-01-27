using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class TitleScript : MonoBehaviour
{
    public Image fadeImage;
    [SerializeField]float fadeSpeed = 1.0f;
    float red, green, blue, alfa;
    bool isFadeOut = false;
    private GameObject menuSoundScript;
    private GameObject BGM;
    private int around;
    private float inputHorizontal;
    [SerializeField] GameObject[] image = new GameObject[2];
    [SerializeField] GameObject video;
    private VideoPlayer videoPlayer;
    bool playVideo;
    // Start is called before the first frame update
    void Start()
    {
        red = fadeImage.color.r;
        green= fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;
        menuSoundScript = GameObject.Find("MenuSound").transform.gameObject;
        BGM = GameObject.Find("BGM").transform.gameObject;
        BGM.GetComponent<BGM>().TitleSelectBGM();
        around = 1;
        videoPlayer = video.transform.GetChild(1).GetComponent<VideoPlayer>();
        playVideo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playVideo)
        {
            return;
        }
        if (isFadeOut)
        {
            StartFadeOut();
        }
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        StickMove();
        StickPosition();
        SceneMove();
    }

    void StartFadeOut()
    {
        fadeImage.enabled = true;
        alfa += Time.deltaTime*fadeSpeed;
        SetAlpha();
        if (alfa>=1)
        {
            isFadeOut = false;
            SceneManager.LoadScene("SelectScene");
        }

    }
    void SetAlpha()
    {
        fadeImage.color = new Color(red,green,blue,alfa);
    }

    private void StickMove()
    {
        if (isFadeOut)
        {
            return;
        }
        if (inputHorizontal >= 0.5f && around == 1)
        {
            around = 2;
            menuSoundScript.GetComponent<MenuSoundScript>().Select();
        }
        else if (inputHorizontal < -0.5f && around == 2)
        {
            around = 1;
            menuSoundScript.GetComponent<MenuSoundScript>().Select();
        }
    }
    private void StickPosition()
    {
        if (around == 1)
        {
            image[0].SetActive(true);
            image[1].SetActive(false);

        }
        else if (around == 2)
        {
            image[0].SetActive(false);
            image[1].SetActive(true);
        }

    }

    private void SceneMove()
    {

        if (around == 1 && (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.B)) && isFadeOut == false)
        {
            isFadeOut = true;
            menuSoundScript.GetComponent<MenuSoundScript>().Decide();
        }
        else if (around == 2 && (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.B)) && isFadeOut == false)
        {
            //遊び方
            menuSoundScript.GetComponent<MenuSoundScript>().Decide();
            video.SetActive(true);
            videoPlayer.Prepare();
            playVideo = true;
            videoPlayer.prepareCompleted += VideoStart;
            videoPlayer.loopPointReached += VideoStop;
        }
       

    }
    //遊び方
    void VideoStart(UnityEngine.Video.VideoPlayer vp)
    {
        BGM.GetComponent<BGM>().HowTo();
        vp.Play();
    }
    void VideoStop(UnityEngine.Video.VideoPlayer vp)
    {
        BGM.GetComponent<BGM>().TitleSelectBGM();
        vp.Stop();
        playVideo = false;
        video.SetActive(false);
    }
}
