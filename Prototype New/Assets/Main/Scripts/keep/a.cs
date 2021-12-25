using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class a : MonoBehaviour
{
    private int around;
    private float inputHorizontal;
    [SerializeField] GameObject[] image = new GameObject[2];
    Scene nowScene;
    // Start is called before the first frame update
    void Start()
    {
        around = 1;
        nowScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
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
        }
        else if (inputHorizontal < -0.5f && around == 2)
        {
            around = 1;
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
        if (around == 1 && (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.B)))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nowScene.name);
        }
        else if (around == 2 && (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.B)))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Title");
        }
       
        
    }
}
