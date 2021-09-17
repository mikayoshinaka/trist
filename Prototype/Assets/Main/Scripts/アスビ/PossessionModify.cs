﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionModify : MonoBehaviour
{
    public GhostChange ghostChange;
    private bool layerChanged;
    int playerLayer;

    public RaycastInvisibleScript raycastInvisible;

    private GameObject parentObj;
    private void Start()
    {
        ghostChange = GameObject.Find("Ghost").GetComponent<GhostChange>();
        layerChanged = false;
        playerLayer = LayerMask.NameToLayer("Player");

        raycastInvisible = GameObject.Find("Main Camera").GetComponent<RaycastInvisibleScript>();
    }

    // とりつく時、とりつくものLayerを変更する処理
    private void Update()
    {
        if (ghostChange.possess && !layerChanged)
        {
            parentObj = ghostChange.possessObject;
            if (parentObj.tag == "Monkey")
            {
                parentObj.layer = playerLayer;
            }

            raycastInvisible.ResetTransparency();
            layerChanged = true;
        }
        else if (!ghostChange.possess && layerChanged)
        {
            if (parentObj.tag == "Monkey")
            {
                parentObj.layer = 0;
            }

            layerChanged = false;
        }
    }
}
