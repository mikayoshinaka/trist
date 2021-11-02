﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	public float speed = 3.0f;

	private void Start()
    {
        
    }


    private void Update()
    {
		if (Input.GetKey("up"))
		{
			transform.position += transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey("down"))
		{
			transform.position -= transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey("right"))
		{
			transform.position += transform.right * speed * Time.deltaTime;
		}
		if (Input.GetKey("left"))
		{
			transform.position -= transform.right * speed * Time.deltaTime;
		}
	}
}