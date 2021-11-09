using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [Header("Game State")]
    public GameStateManager gameStateManager;

    [Header ("Player")]
    public GameObject playerController;

    public enum EnemyMode
    {
        Mode_Defensive,
        Mode_Offensive
    }
    [Header("Enemy Mode")]
    public EnemyMode enemyMode;

    [Header("Enemy Properties")]
    public float speed = 4f;
    public float chaseSpeed = 6f;
    public float patrolRange = 10f;
    public float sightRange = 8f;
    public float attackRange = 1.5f;

    #region 現状利用しない

    [HideInInspector]
    public bool attacking;
    [HideInInspector]
    public float attackCooldown = 3f;
    [HideInInspector]
    public float worldStopTimer = 2f;
    
    #endregion

    private void Start()
    {
        gameStateManager = GameObject.Find("GameState").GetComponent<GameStateManager>();
        playerController = GameObject.Find("PlayerController");
        attacking = false;
    }
}
