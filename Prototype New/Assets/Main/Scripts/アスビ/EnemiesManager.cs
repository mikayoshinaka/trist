﻿using System.Collections;
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
    public int childCount = 5;
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

    private void Start()
    {
        gameStateManager = GameObject.Find("GameState").GetComponent<GameStateManager>();
        playerController = GameObject.Find("PlayerController");
        enemySpawner = GameObject.Find("EnemySpawner");
        enableGizmos = false;
    }

    public void SetMode(EnemyMode mode)
    {
        enemyMode = mode;

        if (enemyMode == EnemyMode.Mode_Defensive)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.GetComponent<EnemyBehaviour>().gimmickAction)
                {
                    child.GetComponent<EnemyBehaviour>().gimmickAction = false;
                }
            }
        }
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
            enemy.SetActive(true);
            //小野澤　クリア用
            GameObject.Find("ClearScene").GetComponent<ClearScene>().enemy.Add(enemy);
        }
    }

    // 仮バグ修正
    public void ClearGimmick()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<EnemyBehaviour>().gimmickAction)
            {
                transform.GetChild(i).GetComponent<EnemyBehaviour>().gimmickAction = false;
            }

            if (transform.GetChild(i).GetComponent<EnemyBehaviour>().mazeGimmick)
            {
                transform.GetChild(i).GetComponent<EnemyBehaviour>().ClearMazeAction();
            }

            // エフェクト
            if (transform.GetChild(i).transform.childCount > childCount)
            {
                for (int j = childCount; j < transform.GetChild(i).transform.childCount; j++)
                {
                    Destroy(transform.GetChild(i).GetChild(j).gameObject);
                }
            }
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
