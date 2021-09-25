using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorScript : MonoBehaviour
{
    public bool open;
    private bool opened;
    private Animator Animator;
    private Animator RightAnimator;
    [SerializeField] GameObject goal;
    [SerializeField] GameObject doorEnemy;
    [SerializeField] GameObject door;
    [SerializeField] GameObject door2;
    [SerializeField] GameObject enemyAppearColor;
    // Start is called before the first frame update
    void Start()
    {
        open = false;
        opened = false;
        Animator = door.GetComponent<Animator>();
        if (door2==null) {

        }
        else
        {
            RightAnimator = door2.GetComponent<Animator>();
        }
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
        if (open)
        {
            Animator.SetBool("Open", !Animator.GetBool("Open"));
            if (door2 == null)
            {

            }
            else
            {
                RightAnimator.SetBool("Open", !RightAnimator.GetBool("Open"));
            }
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
            enemyAppearColor.GetComponent<EnemyAppearColor>().SetAppearEnemy(doorEnemy);
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
