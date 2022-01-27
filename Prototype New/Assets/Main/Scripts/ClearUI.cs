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
    [SerializeField] GameObject orangeDoll2;
    [SerializeField] GameObject purpleDoll2;
    [SerializeField] GameObject blueDoll2;
    [SerializeField] GameObject greenDoll2;
    [SerializeField] GameObject redDoll2;
    [SerializeField] GameObject yellowDoll2;
    [SerializeField] GameObject bossDoll;
    [SerializeField] GameObject orangeEndPos;
    [SerializeField] GameObject purpleEndPos;
    [SerializeField] GameObject blueEndPos;
    [SerializeField] GameObject greenEndPos;
    [SerializeField] GameObject redEndPos;
    [SerializeField] GameObject yellowEndPos;
    [SerializeField] GameObject orangeEndPos2;
    [SerializeField] GameObject purpleEndPos2;
    [SerializeField] GameObject blueEndPos2;
    [SerializeField] GameObject greenEndPos2;
    [SerializeField] GameObject redEndPos2;
    [SerializeField] GameObject yellowEndPos2;
    [SerializeField] GameObject bossEndPos;
    RectTransform orangePos;
     RectTransform purplePos;
     RectTransform bluePos;
     RectTransform greenPos;
     RectTransform redPos;
     RectTransform yellowPos;
     RectTransform orange2Pos;
     RectTransform purple2Pos;
     RectTransform blue2Pos;
     RectTransform green2Pos;
     RectTransform red2Pos;
     RectTransform yellow2Pos;
     RectTransform bossPos;
    [SerializeField] private float dollMoveSpeed=2.0f;
    private int dollCount;
    private int dollCountOne;
    private int dollCountTen;
    private int dollCountHundred;
    private int fallDollCount;
    private GameObject BGM;
    public bool orange, purple, blue, green, red, yellow, orange2, purple2, blue2, green2, red2, yellow2;
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
        orange2 = false;
        purple2 = false;
        blue2 = false;
        green2 = false;
        red2 = false;
        yellow2 = false;
        orangePos = orangeDoll.GetComponent<RectTransform>();
        purplePos = purpleDoll.GetComponent<RectTransform>();
        bluePos = blueDoll.GetComponent<RectTransform>();
        greenPos = greenDoll.GetComponent<RectTransform>();
        redPos = redDoll.GetComponent<RectTransform>();
        yellowPos = yellowDoll.GetComponent<RectTransform>();
        orange2Pos = orangeDoll2.GetComponent<RectTransform>();
        purple2Pos = purpleDoll2.GetComponent<RectTransform>();
        blue2Pos = blueDoll2.GetComponent<RectTransform>();
        green2Pos = greenDoll2.GetComponent<RectTransform>();
        red2Pos = redDoll2.GetComponent<RectTransform>();
        yellow2Pos = yellowDoll2.GetComponent<RectTransform>();
        bossPos=bossDoll.GetComponent<RectTransform>();
        DollColorExist();

    }

    // Update is called once per frame
    void Update()
    {
        //存在するなら動かす
        if(fallDollCount==0)
        {
            if(orange==true)
            {
                if (orangeDoll.activeSelf==false)
                {
                    orangeDoll.SetActive(true);
                }
                DollMove(orangePos,orangeEndPos);
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
                DollMove(purplePos, purpleEndPos);
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
                DollMove(bluePos, blueEndPos);
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
                DollMove(greenPos, greenEndPos);
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
                DollMove(redPos, redEndPos);
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
                DollMove(yellowPos, yellowEndPos);
            }
            else
            {
                fallDollCount++;
            }
        }
        if (fallDollCount == 6)
        {
            if (orange2 == true)
            {
                if (orangeDoll2.activeSelf == false)
                {
                    orangeDoll2.SetActive(true);
                }
                DollMove(orange2Pos, orangeEndPos2);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 7)
        {
            if (purple2 == true)
            {
                if (purpleDoll2.activeSelf == false)
                {
                    purpleDoll2.SetActive(true);
                }
                DollMove(purple2Pos, purpleEndPos2);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 8)
        {
            if (blue2 == true)
            {
                if (blueDoll2.activeSelf == false)
                {
                    blueDoll2.SetActive(true);
                }
                DollMove(blue2Pos, blueEndPos2);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 9)
        {
            if (green2 == true)
            {
                if (greenDoll2.activeSelf == false)
                {
                    greenDoll2.SetActive(true);
                }
                DollMove(green2Pos, greenEndPos2);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 10)
        {
            if (red2 == true)
            {
                if (redDoll2.activeSelf == false)
                {
                    redDoll2.SetActive(true);
                }
                DollMove(red2Pos, redEndPos2);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 11)
        {
            if (yellow2 == true)
            {
                if (yellowDoll2.activeSelf == false)
                {
                    yellowDoll2.SetActive(true);
                }
                DollMove(yellow2Pos, yellowEndPos2);
            }
            else
            {
                fallDollCount++;
            }
        }
        else if (fallDollCount == 12)
        {
            if (bossDoll.activeSelf == false)
            {
                bossDoll.SetActive(true);
            }
            DollMove(bossPos, bossEndPos);
        }
        else if (fallDollCount == 13)
        {
            if (endUI.activeSelf==false) {
                endUI.SetActive(true);
            }
        }
        
    }
    //捕まえた色を判別
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
            else if (dollColor[i] == 7)
            {
                orange2 = true;
            }
            else if (dollColor[i] == 8)
            {
                purple2 = true;
            }
            else if (dollColor[i] == 9)
            {
                blue2 = true;
            }
            else if (dollColor[i] == 10)
            {
                green2 = true;
            }
            else if (dollColor[i] == 11)
            {
                red2 = true;
            }
            else if (dollColor[i] == 12)
            {
                yellow2 = true;
            }
        }
    }
    //人形を動かす
    void DollMove(RectTransform doll,GameObject dollPos)
    {
        doll.transform.position = Vector3.MoveTowards(doll.transform.position, dollPos.transform.position, dollMoveSpeed * Time.deltaTime);
        float dis = (doll.transform.position - dollPos.transform.position).magnitude;
        if (dis < 0.1f)
        {
            fallDollCount++;
        }
        
    }
    //人形の数を数える
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
