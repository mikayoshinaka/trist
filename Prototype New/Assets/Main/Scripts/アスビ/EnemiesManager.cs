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
    public float angularSpeed = 240f;
    public float accelSpeed = 8f;
    public float patrolRange = 10f;
    public float sightRange = 8f;
    public float attackRange = 1.5f;

    [Header("Respawn")]
    public int respawnAmount = 20;
    public GameObject enemySpawner;
    public bool enableGizmos;

    [Header("Enemy Prefab")]
    public GameObject enemyRedPrefab;
    public GameObject enemyBluePrefab;
    public GameObject enemyYellowPrefab;

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
        enemySpawner = GameObject.Find("EnemySpawner");
        attacking = false;
        enableGizmos = false;
    }

    // 敵をスポーンする処理
    public void RespawnEnemy()
    {
        int currentEnemy = transform.childCount;
        int newSpawn = respawnAmount - currentEnemy;

        for (int i = 0; i < newSpawn; i++)
        {
            float enemyRange = Random.Range(0, 3);
            GameObject enemy = null;
            if (enemyRange == 0)
            {
                enemy = enemyRedPrefab;
            }
            else if (enemyRange == 1)
            {
                enemy = enemyBluePrefab;
            }
            else if (enemyRange == 2)
            {
                enemy = enemyYellowPrefab;
            }

            int zoneRange = Random.Range(0, enemySpawner.transform.childCount);
            Transform zone = enemySpawner.transform.GetChild(zoneRange);

            Instantiate(enemy, zone.position, zone.rotation, this.transform);
        }
    }

    private void OnDrawGizmos()
    {
        if (enableGizmos)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < enemySpawner.transform.childCount; i++)
            {
                Gizmos.DrawWireSphere(enemySpawner.transform.GetChild(i).position, 5f);
            }
        } 
    }
}
