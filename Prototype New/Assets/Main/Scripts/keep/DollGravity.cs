using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollGravity : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 move;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        move = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Gravity();
    }

    public void Gravity()
    {
        move.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(move * Time.deltaTime);
    }
}
