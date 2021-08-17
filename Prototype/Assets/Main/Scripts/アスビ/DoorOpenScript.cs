using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenScript : MonoBehaviour
{
    public GameObject door;
    public GameObject enemy;
    public Material color1;
    public Material color2;
    
    bool once;
    // Start is called before the first frame update
    void Start()
    {
        once = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton3)) && !once)
        {
            once = true;
            //door.GetComponent<Renderer>().material = color2;
            //door.GetComponent<BoxCollider>().enabled = true;

            for (int i = 0; i < door.transform.childCount; i++)
            {
                door.transform.GetChild(i).transform.GetChild(0).GetComponent<Renderer>().material = color2;
                door.transform.GetChild(i).transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
            }
            enemy.SetActive(true);
        }
    }
}
