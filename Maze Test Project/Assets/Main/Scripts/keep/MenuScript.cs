using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour
{
     private int around;
     private int upAndDown;
    private int beforeReleaseAround;
    private int beforeReleaseUpAndDown;
    private float inputHorizontal;
    private float inputVertical;
    private bool stage2 = false;
    private bool cannotInput = false;
    [SerializeField] Text stage1Text;
    [SerializeField] Text stage2Text;
    [SerializeField] GameObject[] image = new GameObject[4];
    [SerializeField] GameObject cannotPushStage2Image;
    // Start is called before the first frame update
    void Start()
    {
        around = 1;
        upAndDown = 1;
        beforeReleaseAround = 1;
        beforeReleaseUpAndDown = 1;
        if (PlayerPrefs.GetInt("version")==2)
        {
            stage2 = true;
            cannotPushStage2Image.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DenoteStage();
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        StickMove();
        StickPosition();
        SceneMove();
    }


    private void StickMove()
    {
        if(inputHorizontal>=0.5f&&around==1)
        {
            beforeReleaseAround += 1;
            if (stage2 == false)
            {
                BeforeReleaseStage2();
            }
            if (cannotInput)
            {
                cannotInput = false;
                return;
            }
            around += 1;
        }
        else if(inputHorizontal < -0.5f&&around == 2)
        {    
            beforeReleaseAround -= 1;
            if (stage2 == false)
            {
                BeforeReleaseStage2();
            }
            if(cannotInput)
            {
                cannotInput = false;
                return;
            }
            around -= 1;
        }
        else if (inputVertical >= 0.5f && upAndDown == 2)
        {
            
            beforeReleaseUpAndDown -= 1;
            if (stage2 == false)
            {
                BeforeReleaseStage2();
            }
            if (cannotInput)
            {
                cannotInput = false;
                return;
            }
            upAndDown -= 1;
        }
        else if (inputVertical < -0.5f && upAndDown == 1)
        {
            
            beforeReleaseUpAndDown += 1;
            if (stage2 == false)
            {
                BeforeReleaseStage2();
            }
            if (cannotInput)
            {
                cannotInput = false;
                return;
            }
            upAndDown += 1;
        }

    }


    private void StickPosition()
    {
        if (around == 1 && upAndDown == 1)
        {
            image[0].SetActive(true);
            image[1].SetActive(false);
            image[2].SetActive(false);
            image[3].SetActive(false);
        }
        else if (around == 2 && upAndDown == 1)
        {
            image[0].SetActive(false);
            image[1].SetActive(true);
            image[2].SetActive(false);
            image[3].SetActive(false);
        }
        else if (around == 1 && upAndDown == 2)
        {
            image[0].SetActive(false);
            image[1].SetActive(false);
            image[2].SetActive(true);
            image[3].SetActive(false);
        }
        else if (around == 2 && upAndDown == 2)
        {
            image[0].SetActive(false);
            image[1].SetActive(false);
            image[2].SetActive(false);
            image[3].SetActive(true);
        }
    }
    private void BeforeReleaseStage2()
    {
        if(beforeReleaseAround==2&&beforeReleaseUpAndDown==1)
        {
            beforeReleaseAround = around;
            beforeReleaseUpAndDown = upAndDown;
            cannotInput = true;
        }
    }
    private void SceneMove()
    {
        if (around == 1 && upAndDown == 1 && (Input.GetKeyDown(KeyCode.JoystickButton1)|| Input.GetKey(KeyCode.B)))
        {
            if (PlayerPrefs.GetInt("stage") == 1) 
            {
                PlayerPrefs.SetInt("stage", 2);
                SceneManager.LoadScene("Stage 2");
            }
            else if (PlayerPrefs.GetInt("stage") == 2)
            {
                PlayerPrefs.SetInt("stage", 1);
                SceneManager.LoadScene("Stage 2'");
            }
            else
            {
                PlayerPrefs.SetInt("stage", 2);
                SceneManager.LoadScene("Stage 2");
            }
        }
        else if (around == 2 && upAndDown == 1 && (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.B)))
        {
            if (PlayerPrefs.GetInt("stage") == 1)
            {
                PlayerPrefs.SetInt("stage", 2);
                SceneManager.LoadScene("Stage 3");
            }
            else if (PlayerPrefs.GetInt("stage") == 2)
            {
                PlayerPrefs.SetInt("stage", 1);
                SceneManager.LoadScene("Stage 3'");
            }
            else
            {
                PlayerPrefs.SetInt("stage", 2);
                SceneManager.LoadScene("Stage 3");
            }
        }
        else if (around == 1 && upAndDown == 2 && (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.B)))
        {
            SceneManager.LoadScene("Stage 1");
        }
        else if (around == 2 && upAndDown == 2 && (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.B)))
        {
            SceneManager.LoadScene("Title");
        }
    }
    private void DenoteStage()
    {
        if (PlayerPrefs.GetInt("stage") == 1)
        {
            stage1Text.text = "stage1";
            stage2Text.text = "stage2";
        }
        else if(PlayerPrefs.GetInt("stage") == 2)
        {
            stage1Text.text = "stage1'";
            stage2Text.text = "stage2'";
        }
    }
    public void ResetStage()
    {
        stage2 = false;
        cannotPushStage2Image.SetActive(true);
        around = 1;
        upAndDown = 1;
        beforeReleaseAround = 1;
        beforeReleaseUpAndDown = 1;
        PlayerPrefs.SetInt("stage", 1);
    }
}
