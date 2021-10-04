using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public bool attacking;
    public float attackCooldown = 3f;
    public float worldStopTimer = 2f;
    bool attacked;
    float timerHalt;

    public GameObject playerController;
    public GameObject ghost;
    public GhostChange ghostChange;

    private void Start()
    {
        attacking = false;
        attacked = false;
        playerController = GameObject.Find("PlayerController");
        ghost = playerController.transform.Find("Ghost").gameObject;
        ghostChange = ghost.GetComponent<GhostChange>();
    }

    private void Update()
    {
        // とりつく時、驚かされた場合

        if (attacking && ghostChange.possess && !attacked)
        {
            GameObject.Find(ghostChange.possessObject.name).GetComponent<MonkeyDoll>().enabled = false;
            attacked = true;
            timerHalt = ghostChange.possessTime;
            //Debug.Log(timerHalt);
        }
        else if (attacked && !attacking && ghostChange.possess)
        {
            GameObject.Find(ghostChange.possessObject.name).GetComponent<MonkeyDoll>().enabled = true;
            attacked = false;
        }

        if (attacking && ghostChange.possess)
        {
            MonkeyDollTimerHalt();
        }
    }

    void MonkeyDollTimerHalt()
    {
        ghostChange.possessTime = timerHalt;
    }
}
