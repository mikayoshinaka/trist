using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawUIManager : MonoBehaviour
{
    int currentNumber;

    private void Start()
    {
        currentNumber = 0;
    }

    public void SetNumber(int NewNumber)
    {
        transform.GetChild(currentNumber).gameObject.SetActive(false);
        transform.GetChild(NewNumber).gameObject.SetActive(true);
        currentNumber = NewNumber;
    }
}
