using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAction : MonoBehaviour
{

    //  仮スクリプト　//


    public enum ColorGimmick
    {
        gimmick_Null,
        gimmick_Red,
        gimmick_Blue,
        gimmick_Yellow,
        gimmick_DarkRed,
        gimmick_DarkBlue,
        gimmick_DarkYellow,
        gimmick_Purple,
        gimmick_Green,
        gimmick_Orange
    }
    [SerializeField] private ColorGimmick colorGimmick;

    private void Start()
    {
        colorGimmick = ColorGimmick.gimmick_Null;
    }

    public void ChooseColorAction(ColorGimmick newGimmick)
    {
        colorGimmick = newGimmick;

        switch (colorGimmick)
        {
            case ColorGimmick.gimmick_Null:
                break;
            case ColorGimmick.gimmick_Red:
                break;
            case ColorGimmick.gimmick_Blue:
                break;
            case ColorGimmick.gimmick_Yellow:
                break;
            case ColorGimmick.gimmick_DarkRed:
                break;
            case ColorGimmick.gimmick_DarkBlue:
                break;
            case ColorGimmick.gimmick_DarkYellow:
                break;
            case ColorGimmick.gimmick_Purple:
                break;
            case ColorGimmick.gimmick_Green:
                break;
            case ColorGimmick.gimmick_Orange:
                break;
        }
    }
}
