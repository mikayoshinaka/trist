using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GHOSTSTOP : MonoBehaviour
{
    public GhostChange ghostChange;

    private void Start()
    {
        ghostChange = GameObject.Find("Ghost").GetComponent<GhostChange>();
    }

    private void Update()
    {
        ghostChange.possessTime = 0f;
    }
}
