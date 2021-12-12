using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonkeyDoll : MonoBehaviour
{
    public CharacterController controller;
    public GameObject[] enemy;
    public GameObject doorEnemy;
    [SerializeField] GameObject openDoor;
    [SerializeField] private float speed=10.0f;
    [SerializeField] private float rotateSpeed = 100.0f;
    public Camera Camera;
    private float inputHorizontal;
    private float inputVertical;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        if(Input.GetAxisRaw("RHorizontal")<0)
        {
            transform.Rotate(0.0f, -rotateSpeed * Time.deltaTime, 0.0f);
        }
        else if(0<Input.GetAxisRaw("RHorizontal") )
        {
            transform.Rotate(0.0f, rotateSpeed * Time.deltaTime, 0.0f);
        }
        if(Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0.0f,-rotateSpeed * Time.deltaTime, 0.0f);
        }
        else if (Input.GetKey(KeyCode.R))
        {
            transform.Rotate(0.0f,rotateSpeed * Time.deltaTime, 0.0f);
        }
        else if (Input.GetKey(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (openDoor.GetComponent<DoorAnimation>().open == false)
            {
                openDoor.GetComponent<DoorAnimation>().open = true;
            }
            Sort();
        }
        Vector3 cameraForward = Vector3.Scale(Camera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveForward = cameraForward * inputVertical + Camera.transform.right * inputHorizontal;
        controller.Move(moveForward * speed * Time.deltaTime);
        //transform.position += moveForward * speed * Time.deltaTime;
        

    }

    private void Sort()
    {
        GameObject temp;
        for (int i = 0; i < enemy.Length- 1; i++)
        {
            for (int j = enemy.Length - 1; j > i; j--)
            {
                if (Vector3.Distance(this.transform.position, enemy[j - 1].transform.position)
                    > Vector3.Distance(this.transform.position, enemy[j].transform.position))
                {
                    temp = enemy[j - 1];
                    enemy[j - 1] = enemy[j];
                    enemy[j] = temp;
                }
            }

        }
    }
}
