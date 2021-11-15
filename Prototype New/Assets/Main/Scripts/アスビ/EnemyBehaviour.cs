using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public static Transform player;
    public EnemiesManager enemiesManager;
    public GameObject enemy;
    public GameObject enemyBody;
    public EnemySight enemySight;
    public NavMeshAgent agent;
    public Animator enemyAnimator;
    public GameObject enemyCanvas;

    [Header("当たり判定")]
    public float patrolRange = 10f;
    public static float sightRange = 7.5f;
    public static float attackRange = 1.5f;
    public LayerMask playerMask;
    public LayerMask stageMask;
    public bool playerInSightRange, playerInAttackRange;

    public GhostCatch ghostCatch;
    private enum EnemyState
    {
        Patrol,
        Locate,
        Attack
    }
    private EnemyState enemyState;

    [Header("Patrol")]
    public Vector3 patrolPoint;
    public bool patrolSet;

    [Header("Locate")]
    public bool moveAway;
    private bool chasing;
    private float chaseRange = 1f;

    [Header("Gimmick")]
    public bool gimmickAction;

    [Header("Editor View")]
    public bool enableGizmos;

    void Start()
    {
        player = GameObject.Find("PlayerController").transform;
        enemiesManager = transform.parent.GetComponent<EnemiesManager>();
        enemy = this.gameObject;
        enemyBody = enemy.transform.Find("EnemyBody").gameObject;
        enemySight = GetComponent<EnemySight>();

        agent = GetComponent<NavMeshAgent>();
        playerMask = LayerMask.GetMask("PlayerTrigger");
        stageMask = LayerMask.GetMask("Stages");

        agent.speed = enemiesManager.speed;
        patrolRange = enemiesManager.patrolRange;
        sightRange = enemiesManager.sightRange;
        attackRange = enemiesManager.attackRange;

        enemyAnimator = enemyBody.GetComponent<Animator>();
        enemyCanvas = enemy.transform.Find("EnemyCanvas").gameObject;

        enemyState = EnemyState.Patrol;
        patrolSet = false;
        moveAway = false;
        gimmickAction = false;
        //enableGizmos = true;
    }

    void Update()
    {
        // 当たり判定
        // 索敵判定
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange * chaseRange, playerMask);

        // 攻撃判定
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (gimmickAction)
        {
            Gimmick_Update();
        }
        else if (moveAway)
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

    void ActionAdjustment()
    {
        // ノーマルスピード
        if (enemyState == EnemyState.Patrol)
        {
            chasing = false;
            agent.speed = enemiesManager.speed;
            chaseRange = 1f;
            enemySight.chaseAngle = 1f;
        }
        // スピード上がる
        else if (enemyState == EnemyState.Locate)
        {
            chasing = true;
            agent.speed = enemiesManager.chaseSpeed;
            chaseRange = 2f;
            enemySight.chaseAngle = 2f;
        }
    }

    #region 自由移動

    // 設定
    void Patrol()
    {
        if ((!playerInSightRange && !playerInAttackRange) || !enemySight.detected)
        {
            if (enemyState != EnemyState.Patrol)
            {
                enemyState = EnemyState.Patrol;
                ActionAdjustment();
                enemyAnimator.SetBool("Running", false);
            }

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

    // 設定
    void Locate()
    {
        if (playerInSightRange && !playerInAttackRange)
        {
            enemySight.inView = true;
            if (enemySight.detected)
            {
                if (enemyState != EnemyState.Locate)
                {
                    enemyState = EnemyState.Locate;
                }

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
        else if (enemySight.inView)
        {
            enemySight.inView = false;
        }
    }

    // プレイヤーを追いかける
    void Chase()
    {
        if (!chasing)
        {
            ActionAdjustment();
            if (SurprisedFinding != null)
            {
                StopCoroutine(SurprisedFinding);
            }
            SurprisedFinding = StartCoroutine(SurprisedOnFound());
        }

        agent.SetDestination(player.position);
    }

    Coroutine SurprisedFinding;
    IEnumerator SurprisedOnFound()
    {
        agent.isStopped = true;
        enemyCanvas.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        agent.isStopped = false;
        enemyCanvas.SetActive(false);

        enemyAnimator.SetBool("Running", true);
    }

    #endregion

    #region 攻撃

    // 設定
    void Action()
    {
        if (playerInSightRange && playerInAttackRange)
        {
            if (enemyState != EnemyState.Attack)
            {
                enemyState = EnemyState.Attack;
            }

            if (enemiesManager.enemyMode == EnemiesManager.EnemyMode.Mode_Defensive)
            {
                Defense();
            }
            else if (enemiesManager.enemyMode == EnemiesManager.EnemyMode.Mode_Offensive)
            {
                Offense();
            }
        }   
    }

    // プレイヤーと当たった時に逃げる
    void Defense()
    {
        Runaway();
    }

    // プレイヤーを攻撃するギミック
    void Offense()
    {

        //
        // プレイヤーと当たった時 //
        if (ghostCatch.mode==GhostCatch.Mode.Carry) {
            ghostCatch.SetState(GhostCatch.Mode.Attacked);
        }
        // ここにスポーン処理を呼ぶ //
        // 

        enemiesManager.gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Collect);
    }

    #endregion

    #region 離れる行動

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
        enemyAnimator.SetBool("Running", true);
        agent.isStopped = true;
        SetAwayDirection();
        moveAway = true;
        yield return new WaitForSeconds(3f);
        enemyAnimator.SetBool("Running", false);
        agent.isStopped = false;
        moveAway = false;
    }

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

    #region 色ギミック

    public void Gimmick_Update()
    {

    }

    #region Dark Red
    public void Gimmick_DarkRed()
    {
        gimmickAction = true;
        agent.isStopped = true;
        if (DarkRedCoroutine != null)
        {
            StopCoroutine(DarkRedCoroutine);
        }
        DarkRedCoroutine = StartCoroutine(OnDarkRed());

        enemyAnimator.SetBool("Surprised", true);
    }
    Coroutine DarkRedCoroutine;
    IEnumerator OnDarkRed()
    {
        yield return new WaitForSeconds(3f);
        gimmickAction = false;
        agent.isStopped = false;
    }
    #endregion

    #region Dark Blue
    public void Gimmick_DarkBlue()
    {
        gimmickAction = true;
        agent.isStopped = true;
        if (DarkBlueCoroutine != null)
        {
            StopCoroutine(DarkBlueCoroutine);
        }
        DarkBlueCoroutine = StartCoroutine(OnDarkBlue());
    }
    Coroutine DarkBlueCoroutine;
    IEnumerator OnDarkBlue()
    {
        float timer = 0f;
        float timeLimit = 2f;
        SetAwayDirection();
        while (timer < timeLimit)
        {
            timer += Time.deltaTime;
            MoveAway();
            yield return null;
        }
        gimmickAction = false;
        agent.isStopped = false;
    }
    #endregion

    #region DarkYellow
    public void Gimmick_DarkYellow()
    {
        gimmickAction = true;
        agent.isStopped = true;
        if (DarkYellowCoroutine != null)
        {
            StopCoroutine(DarkYellowCoroutine);
        }
        DarkYellowCoroutine = StartCoroutine(OnDarkYellow());

        enemyAnimator.SetBool("Surprised", true);
    }
    Coroutine DarkYellowCoroutine;
    IEnumerator OnDarkYellow()
    {
        yield return new WaitForSeconds(1.5f);
        gimmickAction = false;
        agent.isStopped = false;
    }

    #endregion

    #region Purple
    public void Gimmick_Purple()
    {
        gimmickAction = true;
        agent.isStopped = true;
        if (PurpleCoroutine != null)
        {
            StopCoroutine(PurpleCoroutine);
        }
        PurpleCoroutine = StartCoroutine(onPurple());
    }
    Coroutine PurpleCoroutine;
    IEnumerator onPurple()
    {
        float timer = 0f;
        float timeLimit = 2f;
        SetAwayDirection();
        while (timer < timeLimit)
        {
            timer += Time.deltaTime;
            MoveAway();
            yield return null;
        }
        gimmickAction = false;
        agent.isStopped = false;
    }
    #endregion

    #region Orange
    public void Gimmick_Orange()
    {
        gimmickAction = true;
        agent.isStopped = true;
        if (OrangeCoroutine != null)
        {
            StopCoroutine(OrangeCoroutine);
        }
        OrangeCoroutine = StartCoroutine(OnOrange());

        enemyAnimator.SetBool("Surprised", true);
    }
    Coroutine OrangeCoroutine;
    IEnumerator OnOrange()
    {
        yield return new WaitForSeconds(3f);
        gimmickAction = false;
        agent.isStopped = false;
    }
    #endregion

    #endregion

    private void OnDrawGizmos()
    {
        if (enableGizmos)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, patrolRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange * chaseRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
