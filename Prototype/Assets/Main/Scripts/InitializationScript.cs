using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationScript : MonoBehaviour
{
    [SerializeField] MenuScript menuScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            menuScript.ResetStage();
        }
    }
}
