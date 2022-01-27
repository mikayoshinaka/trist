using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DonyoriBehaviour : MonoBehaviour
{
    public static Transform player;
    [SerializeField] DonyoriManager donyoriManager;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject enemyBody;
    [SerializeField] EnemySight enemySight;
    public NavMeshAgent agent;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] GameObject enemyAura;
    [SerializeField] GameObject enemySweat;
    [SerializeField] GhostCatch ghostCatch;

    [Header("当たり判定")]
    [SerializeField] float patrolRange = 10f;
    public static float sightRange = 7.5f;
    public static float attackRange = 1.5f;
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask stageMask;
    [SerializeField] LayerMask upstageMask;
    [SerializeField] LayerMask bookstandMask;
    public bool playerInSightRange, playerInAttackRange;

    private enum EnemyState
    {
        Patrol,
        Locate,
        Attack
    }
    private EnemyState enemyState;

    [Header("Patrol")]
    [SerializeField] Vector3 patrolPoint;
    [SerializeField] bool patrolSet;
    [SerializeField] bool searchSet;

    [Header("Locate")]
    [SerializeField] bool moveAway;
    [SerializeField] bool standby;
    [SerializeField] bool cooldown;
    private float chaseRange = 1f;

    [Header("Gimmick")]
    [SerializeField] GameObject targetPossession;
    public bool gimmickAction;
    public bool mazeGimmick;

    [Header("Editor View")]
    [SerializeField] bool enableGizmos;

    void Start()
    {
        player = GameObject.Find("PlayerController").transform;
        donyoriManager = transform.parent.GetComponent<DonyoriManager>();
        enemy = this.gameObject;
        enemyBody = enemy.transform.Find("EnemyBody").gameObject;
        enemySight = GetComponent<EnemySight>();

        agent = GetComponent<NavMeshAgent>();
        playerMask = LayerMask.GetMask("PlayerTrigger");
        stageMask = LayerMask.GetMask("Stages");
        upstageMask = LayerMask.GetMask("UpStages");
        bookstandMask = LayerMask.GetMask("Bookstand");

        agent.speed = donyoriManager.speed;
        patrolRange = donyoriManager.patrolRange;
        sightRange = donyoriManager.sightRange;
        attackRange = donyoriManager.attackRange;

        enemyAnimator = enemyBody.GetComponent<Animator>();
        enemyAura = enemy.transform.Find("EnemyAura").gameObject;
        enemySweat = enemy.transform.Find("EnemySweat").gameObject;
        ghostCatch = player.transform.Find("PlayerBody").Find("CatchArea").GetComponent<GhostCatch>();

        enemyState = EnemyState.Patrol;
        patrolSet = false;
        searchSet = false;
        moveAway = false;
        standby = false;
        cooldown = false;
        gimmickAction = false;
        mazeGimmick = false;
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
            // Gimmick_Update();
        }
        else if (moveAway)
        {
            // プレイヤーから逃げる
            MoveAway();
        }
        else if (donyoriManager.enemyMode == DonyoriManager.EnemyMode.Mode_Offensive)
        {
            // 家具を探す
            SearchObject();

            // 家具をとりつく
            Possessing();
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

    // 敵のパラメーター調整
    void ActionAdjustment()
    {
        // ノーマルスピード
        if (enemyState == EnemyState.Patrol)
        {
            agent.speed = donyoriManager.speed;
            chaseRange = 1f;
            enemySight.chaseAngle = 1f;
        }
        // スピード上がる
        else if (enemyState == EnemyState.Locate)
        {
            agent.speed = donyoriManager.chaseSpeed;
            chaseRange = 2f;
            enemySight.chaseAngle = 3f;
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
                if (agent.isStopped)
                {
                    agent.isStopped = false;
                }
                enemyAnimator.SetBool("Running", false);
                if (enemyAura.activeInHierarchy)
                {
                    enemyAura.SetActive(false);
                }
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

        if (Physics.Raycast(patrolPoint, -transform.up, 10f, stageMask + upstageMask))
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

                if (donyoriManager.enemyMode == DonyoriManager.EnemyMode.Mode_Defensive)
                {
                    Runaway(3f);
                }
            }
        }
        else if (enemySight.inView)
        {
            enemySight.inView = false;
        }
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

            if (donyoriManager.enemyMode == DonyoriManager.EnemyMode.Mode_Defensive)
            {
                Defense();
            }
        }   
    }

    // プレイヤーと当たった時に逃げる
    void Defense()
    {
        Runaway(3f);
    }

    #endregion

    #region 離れる行動

    // プレイヤーから逃げる
    void Runaway(float time)
    {
        if (!moveAway)
        {
            if (runningAway != null)
            {
                StopCoroutine(runningAway);
            }
            runningAway = StartCoroutine(RunningAway(time));
        }
    }

    // 逃げるタイマー
    Coroutine runningAway;
    IEnumerator RunningAway(float time)
    {
        enemyAnimator.SetBool("Running", true);
        agent.isStopped = true;
        SetAwayDirection();
        moveAway = true;

        // 汗マーク
        if (donyoriManager.enemyMode == DonyoriManager.EnemyMode.Mode_Defensive)
        {
            enemySweat.SetActive(true);
        }

        yield return new WaitForSeconds(time);

        // 汗マーク
        if (enemySweat.activeInHierarchy)
        {
            enemySweat.SetActive(false);
        }

        enemyAnimator.SetBool("Running", false);
        if (enemyAura.activeInHierarchy)
        {
            enemyAura.SetActive(false);
        }
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

    #region どんより特別行動

    // 通常に戻る
    public void DonyoriReset()
    {
        if (targetPossession != null)
        {
            donyoriManager.possession.Remove(targetPossession);
            targetPossession = null;
        }

        transform.Find("EnemyBody").gameObject.SetActive(true);
        searchSet = false;
        standby = false;
        cooldown = false;
        GetComponent<NavMeshAgent>().speed = transform.parent.GetComponent<DonyoriManager>().speed;
    }

    // 家具を探す
    void SearchObject()
    {
        if (!standby)
        {
            if (!searchSet)
            {
                int maxColliders = 5;
                Collider[] hitColliders = new Collider[maxColliders];
                int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 50f, hitColliders, bookstandMask);
                float distance = 50f;
                for (int i = 0; i < numColliders; i++)
                {
                    float targetDistance = Vector3.Distance(transform.position, hitColliders[i].transform.position);

                    GameObject target = hitColliders[i].transform.parent.gameObject;
                    if (targetDistance < distance && !donyoriManager.possession.Contains(target) && target.activeInHierarchy)
                    {
                        distance = targetDistance;
                        targetPossession = hitColliders[i].transform.parent.gameObject;
                    }
                }

                if (targetPossession != null)
                {
                    donyoriManager.possession.Add(targetPossession);
                    searchSet = true;
                    agent.speed = donyoriManager.chaseSpeed;
                }
            }

            if (searchSet)
            {
                if (Vector3.Distance(transform.position, targetPossession.transform.position) > 1f)
                {
                    if (agent.isStopped)
                    {
                        agent.isStopped = false;
                    }    
                    agent.SetDestination(targetPossession.transform.position);
                }
                else if (transform.Find("EnemyBody").gameObject.activeInHierarchy)
                {
                    transform.Find("EnemyBody").gameObject.SetActive(false);
                    agent.isStopped = true;

                    // Rotation
                    if (FixRotation != null)
                    {
                        StopCoroutine(FixRotation);
                    }
                    FixRotation = StartCoroutine(OnFixRotation());

                    searchSet = false;
                    standby = true;
                }
            }
        }
    }

    // 仮バグ修正
    Coroutine FixRotation;
    IEnumerator OnFixRotation()
    {
        GameObject target = targetPossession;
        transform.rotation = target.transform.rotation;
        yield return new WaitForSeconds(1f);
        transform.rotation = target.transform.rotation;
    }

    void Possessing()
    {
        if (standby)
        {
            enemySight.inView = true;
            if (enemySight.detected && playerInSightRange && !cooldown)
            {
                if (BookSplat != null)
                {
                    StopCoroutine(BookSplat);
                }
                BookSplat = StartCoroutine(OnBookSplat());
            }
        }
    }

    // 本棚攻撃
    Coroutine BookSplat;
    IEnumerator OnBookSplat()
    {
        GameObject booksplatter = targetPossession.transform.Find("BookSplatter").gameObject;
        cooldown = true;

        yield return new WaitForSeconds(0.5f);
        
        // 攻撃開始
        booksplatter.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // 当たり判定 - 当たった時
        if (enemySight.detected && playerInSightRange)
        {
            player.transform.Find("PlayerBody").GetComponent<ParalysisPlayer>().paralysis = true;
        }

        yield return new WaitForSeconds(2f);
        booksplatter.SetActive(false);
    }

    #endregion

    #region 色ギミック

    // 驚かす
    public void Gimmick_Surprised(float duration)
    {
        gimmickAction = true;
        agent.isStopped = true;
        if (SurprisedCoroutine != null)
        {
            StopCoroutine(SurprisedCoroutine);
        }
        SurprisedCoroutine = StartCoroutine(OnSurprised(duration));

        enemyAnimator.SetBool("Surprised", true);
    }
    Coroutine SurprisedCoroutine;
    IEnumerator OnSurprised(float duration)
    {
        yield return new WaitForSeconds(duration);
        gimmickAction = false;
        agent.isStopped = false;
    }

    // 逃げる
    public void Gimmick_Run(float duration)
    {
        gimmickAction = true;
        agent.isStopped = true;
        if (RunCoroutine != null)
        {
            StopCoroutine(RunCoroutine);
        }
        RunCoroutine = StartCoroutine(OnRun(duration));
    }
    Coroutine RunCoroutine;
    IEnumerator OnRun(float duration)
    {
        float timer = 0f;
        float timeLimit = duration;
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

    // 緑ギミック
    #region Green

    public void Gimmick_Green(bool flag, GameObject target)
    {
        if (!flag)
        {
            gimmickAction = true;
        }
        else
        {
            if (GreenCoroutine != null)
            {
                StopCoroutine(GreenCoroutine);
            }
            GreenCoroutine = StartCoroutine(OnGreen(target));
        }
        DonyoriReset();
    }
    Coroutine GreenCoroutine;
    IEnumerator OnGreen(GameObject target)
    {
        float timer = 0f;
        float timeLimit = 5f;
        agent.speed = donyoriManager.chaseSpeed * 2f;
        while (timer < timeLimit)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > 3f)
            {
                agent.SetDestination(target.transform.position);
            }
            else
            {
                // Temporary
                enemyAnimator.SetBool("Surprised", true);
                agent.speed = donyoriManager.speed;
                agent.isStopped = true;
                if (target.tag == "NormalGhost")
                {
                    target.GetComponent<EnemyBehaviour>().Gimmick_Surprised(3f);
                }
                else if (target.tag == "DonyoriGhost")
                {
                    target.GetComponent<DonyoriBehaviour>().Gimmick_Surprised(3f);
                }

                player.GetComponent<ColorAction>().colorAct_Green.RemoveTarget(target);

                Destroy(transform.Find("PossessAura(Clone)").gameObject);

                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
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
