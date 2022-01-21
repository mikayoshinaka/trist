using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClearUI : MonoBehaviour
{
    [SerializeField] GameObject endUI;
    [SerializeField] GameObject hundred;
    [SerializeField] GameObject ten;
    [SerializeField] GameObject one;
    private float timer;
    [SerializeField] private float timeMax = 2.0f;
    private int dollCount;
    private int dollCountOne;
    private int dollCountTen;
    private int dollCountHundred;
    private GameObject BGM;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        hundred.transform.GetChild(0).gameObject.SetActive(false);
        ten.transform.GetChild(0).gameObject.SetActive(false);
        one.transform.GetChild(0).gameObject.SetActive(false);
        DollCount();
        BGM = GameObject.Find("BGM").transform.gameObject;
        if (BGM.GetComponent<BGM>().audioSource.clip == null)
        {
            BGM.GetComponent<BGM>().ClearResultBGM();
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer>timeMax&&endUI.activeSelf==false) {
            endUI.SetActive(true);
        }
    }

    void DollCount()
    {
        dollCount = PlayerPrefs.GetInt("dollCount");
        dollCountHundred = dollCount / 100;
        dollCountTen = (dollCount-(dollCountHundred*100))/10;
        dollCountOne = dollCount- ((dollCountHundred * 100) + (dollCountTen * 10));
        if (dollCount<10)
        {
            hundred.SetActive(false);
            ten.SetActive(false);
        }
        else if(dollCount < 100)
        {
            hundred.SetActive(false);
        }
        if(hundred.activeSelf==true)
        {
            hundred.transform.GetChild(dollCountHundred).gameObject.SetActive(true);
        }
        if(ten.activeSelf == true)
        {
            ten.transform.GetChild(dollCountTen).gameObject.SetActive(true);
        }
        one.transform.GetChild(dollCountOne).gameObject.SetActive(true);
    }
}
