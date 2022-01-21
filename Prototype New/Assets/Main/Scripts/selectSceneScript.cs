using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class selectSceneScript : MonoBehaviour
{
    private int around;
    private float inputHorizontal;
    private float inputVertical;
    [SerializeField] GameObject[] image = new GameObject[3];
    private int beforeAround;
    private GameObject menuSoundScript;
    [SerializeField] GameObject video;
    private VideoPlayer videoPlayer;
    bool playVideo;
    private GameObject BGM;
    // Start is called before the first frame update
    void Start()
    {
        around = 1;
        menuSoundScript = GameObject.Find("MenuSound").transform.gameObject;
        videoPlayer = video.transform.GetChild(1).GetComponent<VideoPlayer>();
        playVideo = false;
        BGM = GameObject.Find("BGM").transform.gameObject;
        if (BGM.GetComponent<BGM>().audioSource.clip == null)
        {
            BGM.GetComponent<BGM>().TitleSelectBGM();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playVideo)
        {
            return;
        }
        inputVertical = Input.GetAxisRaw("Vertical");
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        StickMove();
        StickPosition();
        SceneMove();
    }
    private void StickMove()
    {
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
        else if (inputVertical < -0.5f && (around == 1 || around == 2))
        {
            beforeAround = around;
            around = 3;
            menuSoundScript.GetComponent<MenuSoundScript>().Select();
        }
        else if (inputVertical >= 0.5f && around == 3)
        {

            around = beforeAround;
            menuSoundScript.GetComponent<MenuSoundScript>().Select();
            image[2].SetActive(false);
        }
    }
    private void StickPosition()
    {
        if (around == 1)
        {
            image[0].SetActive(true);
            image[1].SetActive(false);
            image[2].SetActive(false);

        }
        else if (around == 2)
        {
            image[0].SetActive(false);
            image[1].SetActive(true);
            image[2].SetActive(false);
        }
        else if (around == 3)
        {
            image[0].SetActive(false);
            image[1].SetActive(false);
            image[2].SetActive(true);
        }

    }

    private void SceneMove()
    {

        if (around == 1 && (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.B)))
        {
            menuSoundScript.GetComponent<MenuSoundScript>().Decide();
            BGM.GetComponent<BGM>().Stage1BGM();
            PlayerPrefs.SetInt("SceneNumber", 1);
            SceneManager.LoadScene("MazeScene");
        }
        else if (around == 2 && (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.B)))
        {
            BGM.GetComponent<BGM>().Stage2BGM();
            menuSoundScript.GetComponent<MenuSoundScript>().Decide();
            PlayerPrefs.SetInt("SceneNumber", 2);
            SceneManager.LoadScene("Stage 2");
        }
        else if (around == 3 && (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.B)))
        {
            menuSoundScript.GetComponent<MenuSoundScript>().Decide();
            video.SetActive(true);
            videoPlayer.Prepare();
            playVideo = true;
            videoPlayer.prepareCompleted += VideoStart;
            videoPlayer.loopPointReached += VideoStop;
        }

    }
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
