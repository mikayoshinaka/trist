using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAnimatorController : MonoBehaviour
{
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();    
    }

    void End_Surprised()
    {
        animator.SetBool("Surprised", false);
    }
}
