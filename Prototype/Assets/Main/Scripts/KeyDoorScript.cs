using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorScript : MonoBehaviour
{
    public bool open;
    private bool opened;
    private Animator Animator;
    [SerializeField] GameObject goal;
    [SerializeField] GameObject doorEnemy;
    [SerializeField] GameObject door;
    // Start is called before the first frame update
    void Start()
    {
        open = false;
        opened = false;
        Animator = door.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (open && !opened)
        {
            OpenTheDoor();
        }
        if (open)
        {
            Animator.SetBool("Open", !Animator.GetBool("Open"));
        }
    }

    private void OpenTheDoor()
    {

        opened = true;
        if (doorEnemy == null)
        {

        }
        else
        {
            doorEnemy.SetActive(true);
        }
        if (goal == null)
        {

        }
        else
        {
            goal.SetActive(true);
        }

    }

    public void KeyTouch()
    {
        open = true;
    }
}
