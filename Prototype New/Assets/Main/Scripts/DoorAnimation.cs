using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{

    public bool open;
    private bool opened;

    private Animator leftAnimator;
    private Animator rightAnimator;
    [SerializeField] GameObject goal;
    [SerializeField] GameObject doorEnemy;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;
    // Start is called before the first frame update
    void Start()
    {
        open = false;
        opened = false;
        leftAnimator = leftDoor.GetComponent<Animator>();   
        rightAnimator = rightDoor.GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }
        if (open && !opened)
        {
            OpenTheDoor();
        }
    }

    public void MonkeyOpenDoor()
    {
        opened = true;
        doorEnemy.SetActive(true);
        open = true;
        goal.SetActive(true);
    }

    private void OpenTheDoor()
    {
        leftAnimator.SetBool("Open", !leftAnimator.GetBool("Open"));
        rightAnimator.SetBool("Open", !rightAnimator.GetBool("Open"));
        MonkeyOpenDoor();
    }
}
