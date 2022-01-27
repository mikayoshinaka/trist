using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class selectSceneScript : MonoBehaviour
{
    public Image fadeImage;
    [SerializeField] float fadeSpeed = 1.0f;
    float red, green, blue, alfa;
    bool isFadeOut = false;
    private int around;
    private float inputHorizontal;
    [SerializeField] GameObject[] image = new GameObject[2];
    private GameObject menuSoundScript;
    private GameObject BGM;
    // Start is called before the first frame update
    void Start()
    {
        around = 1;
        menuSoundScript = GameObject.Find("MenuSound").transform.gameObject;
        BGM = GameObject.Find("BGM").transform.gameObject;
        if (BGM.GetComponent<BGM>().audioSource.clip == null)
        {
            BGM.GetComponent<BGM>().TitleSelectBGM();
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        alfa += Time.deltaTime * fadeSpeed;
        SetAlpha();
        if (alfa >= 1)
        {
            isFadeOut = false;
            if(PlayerPrefs.GetInt("SceneNumber")==1)
            {
                BGM.GetComponent<BGM>().Stage1BGM();
                SceneManager.LoadScene("MazeScene");
            }
            else
            {
                BGM.GetComponent<BGM>().Stage2BGM();
                SceneManager.LoadScene("Stage 2");
            }
        }

    }
    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
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

        if (around == 1 && (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.B))&&isFadeOut == false)
        {
            isFadeOut = true;
            menuSoundScript.GetComponent<MenuSoundScript>().Decide();
            PlayerPrefs.SetInt("SceneNumber", 1);
        }
        else if (around == 2 && (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.B))&&isFadeOut == false)
        {
            isFadeOut = true;
            menuSoundScript.GetComponent<MenuSoundScript>().Decide();
            PlayerPrefs.SetInt("SceneNumber", 2);
        }

    }
   
}
