using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public GameObject playerController;

    public enum EnemyMode
    {
        Mode_Defensive,
        Mode_Offensive
    }
    public EnemyMode enemyMode;




    public GameObject[] enemies;
    public bool attacking;
    public float attackCooldown = 3f;
    public float worldStopTimer = 2f;
    
    private void Start()
    {
        playerController = GameObject.Find("PlayerController");
        attacking = false;
    }

    private void Update()
    {
       
    }
}
