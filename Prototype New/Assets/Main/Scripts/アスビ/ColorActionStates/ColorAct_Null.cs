using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAct_Null : ColorActState
{
    // 入る処理
    public override void EnterState(ColorAction colorAct)
    {
        //Debug.Log(this);

        // GimmickObject Reset
        GameObject gimmickObjects = colorAct.transform.Find("GimmickObjects").gameObject;
        for (int i = 0; i < gimmickObjects.transform.childCount; i++)
        {
            gimmickObjects.transform.GetChild(i).gameObject.SetActive(false);
        }
        gimmickObjects.SetActive(false);

        // GimmickCanvas Reset
        GameObject gimmickUI = GameObject.Find("Camera Canvas").transform.Find("GimmickUI").gameObject;
        for (int i = 0; i < gimmickUI.transform.childCount; i++)
        {
            gimmickUI.transform.GetChild(i).gameObject.SetActive(false);
        }
        gimmickUI.SetActive(false);

        GameObject cooldownBar = GameObject.Find("Camera Canvas").transform.Find("GimmickCooldownBar").gameObject;
        cooldownBar.GetComponent<ColorActionCooldown>().ResetCooldown();
        cooldownBar.SetActive(false);


        // 透明化用
        SphereCollider transparentCollider = GameObject.Find("SearchArea").GetComponent<SphereCollider>();
        transparentCollider.radius = 3f;
    }

    // Update 処理
    public override void UpdateState(ColorAction colorAct)
    {
        
    }

    // Gizmos 処理
    public override void DrawGizmosState(ColorAction colorAct)
    {   

    }
}
