using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
    private GameObject menuSoundScript;
    // Start is called before the first frame update
    void Start()
    {
        menuSoundScript = GameObject.Find("MenuSound").transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.B))
        {
            menuSoundScript.GetComponent<MenuSoundScript>().Decide();
            SceneManager.LoadScene("Title");
        }
    }
}
