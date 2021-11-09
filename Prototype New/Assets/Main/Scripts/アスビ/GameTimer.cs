using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    Text text;

    public float countdown = 180f;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown >= 0)
        {
            countdown -= Time.deltaTime;
            text.text = ": " + Mathf.Round(countdown);
        }        
    }
}
