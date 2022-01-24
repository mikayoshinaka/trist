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
    [SerializeField] GameObject orangeDoll;
    [SerializeField] GameObject purpleDoll;
    [SerializeField] GameObject blueDoll;
    [SerializeField] GameObject greenDoll;
    [SerializeField] GameObject redDoll;
    [SerializeField] GameObject yellowDoll;
    [SerializeField] GameObject dollPos;
    [SerializeField] RectTransform orangePos;
    [SerializeField] RectTransform purplePos;
    [SerializeField] RectTransform bluePos;
    [SerializeField] RectTransform greenPos;
    [SerializeField] RectTransform redPos;
    [SerializeField] RectTransform yellowPos;
    [SerializeField] private float dollMoveSpeed=2.0f;
    private int dollCount;
    private int dollCountOne;
    private int dollCountTen;
    private int dollCountHundred;
    private int fallDollCount;
    private GameObject BGM;
    public bool orange, purple, blue, green, red, yellow;
    public  List<int> dollColor = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        fallDollCount = 0;
        hundred.transform.GetChild(0).gameObject.SetActive(false);
        ten.transform.GetChild(0).gameObject.SetActive(false);
        one.transform.GetChild(0).gameObject.SetActive(false);
        DollCount();
        BGM = GameObject.Find("BGM").transform.gameObject;
        if (BGM.GetComponent<BGM>().audioSource.clip == null)
        {
            BGM.GetComponent<BGM>().ClearResultBGM();
        }
        dollColor = DollSave.getDollColor();
        orange = false;
        purple = false;
        blue = false;
        green = false;
        red = false;
        yellow = false;
        DollColorExist();

    }

    // Update is called once per frame
    void Update()
    {
        if(fallDollCount==0)
        {
            if(orange==true)
            {
                if (orangeDoll.activeSelf==false)
                {
                    orangeDoll.SetActive(true);
                }
                DollMove(orangePos);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if(fallDollCount == 1)
        {
            if (purple == true)
            {
                if (purpleDoll.activeSelf == false)
                {
                    purpleDoll.SetActive(true);
                }
                DollMove(purplePos);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 2)
        {
            if (blue == true)
            {
                if (blueDoll.activeSelf == false)
                {
                    blueDoll.SetActive(true);
                }
                DollMove(bluePos);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 3)
        {
            if (green == true)
            {
                if (greenDoll.activeSelf == false)
                {
                    greenDoll.SetActive(true);
                }
                DollMove(greenPos);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 4)
        {
            if (red == true)
            {
                if (redDoll.activeSelf == false)
                {
                    redDoll.SetActive(true);
                }
                DollMove(redPos);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 5)
        {
            if (yellow == true)
            {
                if (yellowDoll.activeSelf == false)
                {
                    yellowDoll.SetActive(true);
                }
                DollMove(yellowPos);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 6)
        {
            if (endUI.activeSelf==false) {
                endUI.SetActive(true);
            }
        }
    }
    void DollColorExist()
    {
        for(int i=0;i<dollColor.Count;i++)
        {
            if(dollColor[i]==1)
            {
                orange = true;
            }
            else if(dollColor[i] == 2)
            {
                purple = true;
            }
            else if (dollColor[i] == 3)
            {
                blue = true;
            }
            else if (dollColor[i] == 4)
            {
                green = true;
            }
            else if (dollColor[i] == 5)
            {
                red = true;
            }
            else if (dollColor[i] == 6)
            {
                yellow = true;
            }
        }
    }

    void DollMove(RectTransform doll)
    {
        doll.transform.position = Vector3.MoveTowards(doll.transform.position, dollPos.transform.position, dollMoveSpeed * Time.deltaTime);
        float dis = (doll.transform.position - dollPos.transform.position).magnitude;
        if (dis < 0.1f)
        {
            fallDollCount++;
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
