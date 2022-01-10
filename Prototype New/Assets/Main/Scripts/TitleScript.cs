using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TitleScript : MonoBehaviour
{
    public Image fadeImage;
    [SerializeField]float fadeSpeed = 1.0f;
    float red, green, blue, alfa;
    bool isFadeOut = false;
    private GameObject menuSoundScript;
    // Start is called before the first frame update
    void Start()
    {
        red = fadeImage.color.r;
        green= fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;
        menuSoundScript = GameObject.Find("MenuSound").transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton1)&&isFadeOut==false)
        {
            isFadeOut = true;
            menuSoundScript.GetComponent<MenuSoundScript>().Decide();
        }
        if (isFadeOut) {
            StartFadeOut();
        }
    }

    void StartFadeOut()
    {
        fadeImage.enabled = true;
        alfa += Time.deltaTime*fadeSpeed;
        SetAlpha();
        if (alfa>=1)
        {
            isFadeOut = false;
            SceneManager.LoadScene("MazeScene");
        }

    }
    void SetAlpha()
    {
        fadeImage.color = new Color(red,green,blue,alfa);
    }
}
