using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public bool timerPlay;
    public float countdown = 180f;

    // Raw UI
    public RawUIManager hundreds;
    public RawUIManager tens;
    public RawUIManager ones;

    //�����V �I���p
    [SerializeField] private DollSave dollSave;

    void Start()
    {
        hundreds = transform.Find("Hundreds").GetComponent<RawUIManager>();
        tens = transform.Find("Tens").GetComponent<RawUIManager>();
        ones = transform.Find("Ones").GetComponent<RawUIManager>();
    }

    void Update()
    {
        //�����V�@�J�n�p
        if (GameObject.Find("BeforeBegin").GetComponent<BeforeBegin>().begin == true|| dollSave.isFadeOut == true)
        {
            return;
        }
        if (countdown >= 0 && timerPlay)
        {
            Countdown();
        }        
    }

    void Countdown()
    {
        string number = Mathf.Round(countdown).ToString();
        int digit = number.Length;

        // �R��
        if (digit > 2)
        {
            hundreds.SetNumber(number[0] - '0');
        }
        else
        {
            hundreds.SetNumber(0);
        }

        // �Q��
        if (digit > 1)
        {
            tens.SetNumber(number[digit - 2] - '0');
        }
        else
        {
            tens.SetNumber(0);
        }

        // �P��
        if (countdown > 0)
        {
            ones.SetNumber(number[digit - 1] - '0');
        }
        else
        {
            ones.SetNumber(0);
        }

        countdown -= Time.deltaTime;
    }
}
