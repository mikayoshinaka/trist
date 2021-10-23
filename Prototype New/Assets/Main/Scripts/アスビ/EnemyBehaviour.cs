using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public static Transform player;
    public EnemiesManager enemiesManager;
    public GameObject enemy;
    public NavMeshAgent agent;

    [Header("当たり判定")]
    public float patrolRange = 10f;
    public static float sightRange = 7.5f;
    public static float attackRange = 2f;
    public LayerMask playerMask;
    public LayerMask stageMask;
    public bool playerInSightRange, playerInAttackRange;
    
    [Header("Patrol")]
    public Vector3 patrolPoint;
    public bool patrolSet;

    [Header("Locate")]
    public bool moveAway;


    [Header("Editor View")]
    public bool enableGizmos;

    void Start()
    {
        player = GameObject.Find("PlayerController").transform;
        enemiesManager = transform.parent.GetComponent<EnemiesManager>();
        enemy = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        playerMask = LayerMask.GetMask("Player");
        stageMask = LayerMask.GetMask("Stages");

        patrolSet = false;
        moveAway = false;
    }

    void Update()
    {
        // 当たり判定
        // 索敵判定
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);

        // 攻撃判定
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (moveAway)
        {
            // プレイヤーから逃げる
            MoveAway();
        }
        else
        {
            // 自由移動
            Patrol();

            // 索敵
            Locate();

            // 攻撃
            Action();
        }
    }

    #region 自由移動
    void Patrol()
    {
        if ((!playerInSightRange && !playerInAttackRange) || !enemy.GetComponent<EnemySight>().detected)
        {
            if (!patrolSet)
            {
                SetPatrol();
            }

            if (Vector3.Distance(patrolPoint, transform.position) < 0.5f)
            {
                patrolSet = false;
            }
        }
        else if (patrolSet)
        {
            patrolSet = false;
        }
    }

    // 移動設定
    void SetPatrol()
    {
        float pointX = Random.Range(-patrolRange, patrolRange);
        float pointZ = Random.Range(-patrolRange, patrolRange);
        patrolPoint = new Vector3(transform.position.x + pointX, transform.position.y, transform.position.z + pointZ);

        if (Physics.Raycast(patrolPoint, -transform.up, 10f, stageMask))
        {
            agent.SetDestination(patrolPoint);
            patrolSet = true;

            if (patrolling != null)
            {
                StopCoroutine(patrolling);
            }
            patrolling = StartCoroutine(Patrolling());
        }
    }

    // 移動変更タイマー
    Coroutine patrolling;
    IEnumerator Patrolling()
    {
        yield return new WaitForSeconds(4f);
        patrolSet = false;
    }

    #endregion

    #region 索敵
    void Locate()
    {
        if (playerInSightRange && !playerInAttackRange)
        {
            enemy.GetComponent<EnemySight>().inView = true;
            if (enemy.GetComponent<EnemySight>().detected)
            {
                if (enemiesManager.enemyMode == EnemiesManager.EnemyMode.Mode_Defensive)
                {
                    Runaway();
                }
                else if (enemiesManager.enemyMode == EnemiesManager.EnemyMode.Mode_Offensive)
                {
                    Chase();
                }
            }
        }
        else if (enemy.GetComponent<EnemySight>().inView)
        {
            enemy.GetComponent<EnemySight>().inView = false;
        }
    }

    // プレイヤーから逃げる
    void Runaway()
    {
        if (!moveAway)
        {
            if (runningAway != null)
            {
                StopCoroutine(runningAway);
            }
            runningAway = StartCoroutine(RunningAway());
        }
    }

    // 逃げるタイマー
    Coroutine runningAway;
    IEnumerator RunningAway()
    {
        agent.isStopped = true;
        SetAwayDirection();
        moveAway = true;
        yield return new WaitForSeconds(3f);
        agent.isStopped = false;
        moveAway = false;
    }

    // プレイヤーを追いかける
    void Chase()
    {
        agent.SetDestination(player.position);
    }

    #endregion

    #region 離れる行動

    // プレイヤーから離れる方向
    private Vector3 awayDirection;
    void SetAwayDirection()
    {
        awayDirection = Vector3.Scale(transform.position - player.transform.position, new Vector3(1f, 0f, 1f)).normalized;
    }
    void MoveAway()
    {        
        agent.Move(awayDirection * agent.speed * 2f * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(awayDirection, transform.up);
    }

    #endregion

    #region 攻撃
    void Action()
    {

    }

    void Defense()
    {

    }

    void Offense()
    {

    }

    #endregion

    private void OnDrawGizmos()
    {
        if (enableGizmos)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, patrolRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
