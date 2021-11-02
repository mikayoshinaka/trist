using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAction : MonoBehaviour
{
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
    
    // State Machine

    ColorActState colorActState;
    public ColorAct_Null colorAct_Null = new ColorAct_Null();
    public ColorAct_Red colorAct_Red = new ColorAct_Red();
    public ColorAct_Blue colorAct_Blue = new ColorAct_Blue();
    public ColorAct_Yellow colorAct_Yellow = new ColorAct_Yellow();
    public ColorAct_DarkRed colorAct_DarkRed = new ColorAct_DarkRed();
    public ColorAct_DarkBlue colorAct_DarkBlue = new ColorAct_DarkBlue();
    public ColorAct_DarkYellow colorAct_DarkYellow = new ColorAct_DarkYellow();
    public ColorAct_Purple colorAct_Purple = new ColorAct_Purple();
    public ColorAct_Green colorAct_Green = new ColorAct_Green();
    public ColorAct_Orange colorAct_Orange = new ColorAct_Orange();


    [HideInInspector]
    public LayerMask enemyMask;
    public bool enableGizmos;

    private void Start()
    {
        colorGimmick = ColorGimmick.gimmick_Null;

        colorActState = colorAct_Null;
        colorActState.EnterState(this);

        enemyMask = LayerMask.GetMask("Enemy");
        enableGizmos = true;
    }

    private void Update()
    {
        colorActState.UpdateState(this);
    }

    public ColorGimmick CheckCurrentGimmick()
    {
        return colorGimmick;
    }

    public void ChooseColorAction(ColorGimmick newGimmick)
    {
        colorGimmick = newGimmick;
        switch (colorGimmick)
        {
            case ColorGimmick.gimmick_Null:
                colorActState = colorAct_Null;
                break;
            case ColorGimmick.gimmick_Red:
                colorActState = colorAct_Red;
                break;
            case ColorGimmick.gimmick_Blue:
                colorActState = colorAct_Blue;
                break;
            case ColorGimmick.gimmick_Yellow:
                colorActState = colorAct_Yellow;
                break;
            case ColorGimmick.gimmick_DarkRed:
                colorActState = colorAct_DarkRed;
                break;
            case ColorGimmick.gimmick_DarkBlue:
                colorActState = colorAct_DarkBlue;
                break;
            case ColorGimmick.gimmick_DarkYellow:
                colorActState = colorAct_DarkYellow;
                break;
            case ColorGimmick.gimmick_Purple:
                colorActState = colorAct_Purple;
                break;
            case ColorGimmick.gimmick_Green:
                colorActState = colorAct_Green;
                break;
            case ColorGimmick.gimmick_Orange:
                colorActState = colorAct_Orange;
                break;
        }
        colorActState.EnterState(this);
    }

    private void OnDrawGizmos()
    {
        if (enableGizmos)
        {
            colorActState.DrawGizmosState(this);
        }
    }
}
