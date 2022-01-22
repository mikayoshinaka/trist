using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public static Transform player;
    [SerializeField] EnemiesManager enemiesManager;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject enemyBody;
    [SerializeField] EnemySight enemySight;
    public NavMeshAgent agent;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] GameObject enemyCanvas;
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

    [Header("Locate")]
    [SerializeField] bool moveAway;
    private bool chasing;
    private float chaseRange = 1f;

    [Header("Gimmick")]
    public bool gimmickAction;
    public bool mazeGimmick;

    [Header("Editor View")]
    [SerializeField] bool enableGizmos;

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
        upstageMask = LayerMask.GetMask("UpStages");

        agent.speed = enemiesManager.speed;
        patrolRange = enemiesManager.patrolRange;
        sightRange = enemiesManager.sightRange;
        attackRange = enemiesManager.attackRange;

        enemyAnimator = enemyBody.GetComponent<Animator>();
        enemyCanvas = enemy.transform.Find("EnemyCanvas").gameObject;
        enemyAura = enemy.transform.Find("EnemyAura").gameObject;
        enemySweat = enemy.transform.Find("EnemySweat").gameObject;
        ghostCatch = player.transform.Find("PlayerBody").Find("CatchArea").GetComponent<GhostCatch>();

        enemyState = EnemyState.Patrol;
        patrolSet = false;
        moveAway = false;
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
        else if (mazeGimmick)
        {
            // 迷路
            MazeUpdate();
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

    // 敵のパラメーター調整
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

                if (enemiesManager.enemyMode == EnemiesManager.EnemyMode.Mode_Defensive)
                {
                    Runaway(3f);
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

        enemyCanvas.SetActive(false);

        if (!gimmickAction)
        {
            agent.isStopped = false;
            enemyAnimator.SetBool("Running", true);
            enemyAura.SetActive(true);
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
        Runaway(3f);
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
        if (enemiesManager.enemyMode == EnemiesManager.EnemyMode.Mode_Defensive)
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
    }
    Coroutine GreenCoroutine;
    IEnumerator OnGreen(GameObject target)
    {
        float timer = 0f;
        float timeLimit = 5f;
        agent.speed = enemiesManager.chaseSpeed * 2f;
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
                agent.speed = enemiesManager.speed;
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

    #region 迷路ギミック

    enum MazeGimmickState
    {
        None,
        Gather,
        Pickup,
        Transfer,
        Throw
    }
    [SerializeField] MazeGimmickState mazeGimmickState = MazeGimmickState.None;

    enum MazeFurnitureType
    {
        Bookstand,
        Chair,
        Desk
    }
    MazeFurnitureType mazeFurnitureType;

    GameObject targetFurniture;
    GameObject targetObj;
    Vector3 targetPos;
    float throwRange;
    List<Vector3> trajectory = new List<Vector3>();

    // 迷路開始
    public void MazeGimmick(GameObject furniture, GameObject targetPos, LayerMask mask)
    {
        if (mask == LayerMask.GetMask("Bookstand"))
        {
            mazeFurnitureType = MazeFurnitureType.Bookstand;
            throwRange = 5f;
            agent.speed = enemiesManager.chaseSpeed * 5f;
        }
        else if (mask == LayerMask.GetMask("Chair"))
        {
            mazeFurnitureType = MazeFurnitureType.Chair;
            throwRange = 15f;
            agent.speed = enemiesManager.chaseSpeed;
        }
        else if (mask == LayerMask.GetMask("Desk"))
        {
            mazeFurnitureType = MazeFurnitureType.Desk;
            throwRange = 5f;
            agent.speed = enemiesManager.chaseSpeed * 5.5f;
        }
        agent.angularSpeed = enemiesManager.angularSpeed * 2;
        agent.acceleration = 30;

        mazeGimmick = true;
        mazeGimmickState = MazeGimmickState.Gather;
        targetFurniture = furniture;
        targetObj = targetPos;
    }

    // 迷路のUpdate()
    void MazeUpdate()
    {
        // 迷路ギミック
        if (mazeGimmickState == MazeGimmickState.None)
        {
            // なし
        }
        else if (mazeGimmickState == MazeGimmickState.Gather)
        {
            MazeGimmick_Move(targetFurniture);
        }
        else if (mazeGimmickState == MazeGimmickState.Pickup)
        {
            MazeGimmick_Pickup();
        }
        else if (mazeGimmickState == MazeGimmickState.Transfer)
        {
            MazeGimmick_Move(targetObj);
        }
    }

    // 移動処理、家具を探すと持っている処理
    void MazeGimmick_Move(GameObject target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (mazeGimmickState == MazeGimmickState.Gather && distance > 5f)
        {
            agent.SetDestination(target.transform.position);
        }
        else if (mazeGimmickState == MazeGimmickState.Transfer && distance > throwRange)
        {
            agent.SetDestination(target.transform.position);
        }
        else
        {
            agent.isStopped = true;
            if (mazeGimmickState == MazeGimmickState.Gather)
            {
                mazeGimmickState = MazeGimmickState.Pickup;
            }
            else if (mazeGimmickState == MazeGimmickState.Transfer)
            {
                mazeGimmickState = MazeGimmickState.Throw;
                targetFurniture.transform.rotation = target.transform.rotation;
                if (mazeFurnitureType == MazeFurnitureType.Bookstand)
                {
                    targetFurniture.transform.SetParent(GameObject.Find("Furnitures").transform.Find("Bookstands"));
                    targetPos = target.transform.position;
                }
                else if (mazeFurnitureType == MazeFurnitureType.Desk)
                {
                    targetFurniture.transform.SetParent(GameObject.Find("Furnitures").transform.Find("Desks"));
                    targetPos = target.transform.position;
                }
                else if (mazeFurnitureType == MazeFurnitureType.Chair)
                {
                    targetFurniture.transform.SetParent(GameObject.Find("Furnitures").transform.Find("Chairs"));
                    targetPos = player.transform.position;
                    targetPos += new Vector3(0, 1f, 0);
                }

                ThrowTrajectory();
                if (GimmickThrow != null)
                {
                    StopCoroutine(GimmickThrow);
                }
                GimmickThrow = StartCoroutine(MazeGimmick_Throw());
            }
        }
    }

    // 家具を取る処理
    void MazeGimmick_Pickup()
    {
        Vector3 currentPos = targetFurniture.transform.position;
        Vector3 pickupSpot = transform.Find("PickupSpot").transform.position;
        targetFurniture.transform.position = Vector3.MoveTowards(currentPos, pickupSpot, 10f * Time.deltaTime);
        float distance = Vector3.Distance(targetFurniture.transform.position, pickupSpot);
        if (distance < 0.1f)
        {
            targetFurniture.transform.position = pickupSpot;
            targetFurniture.transform.SetParent(this.transform);
            agent.isStopped = false;

            mazeGimmickState = MazeGimmickState.Transfer;
            if (mazeFurnitureType == MazeFurnitureType.Chair)
            {
                targetObj = player.gameObject;   
            }

            if (enemiesManager.enemyMode == EnemiesManager.EnemyMode.Mode_Offensive)
            {
                targetFurniture.transform.Find("Navmesh").gameObject.SetActive(true);
                NavMeshSurface surface = GameObject.Find("NavMeshPlayer").GetComponent<NavMeshSurface>();
                surface.BuildNavMesh();
            }
        }
    }

    // 家具を投げる弾道
    void ThrowTrajectory()
    {
        trajectory.Clear();
        Vector3 origin = transform.position;
        Vector3 target = targetPos;
        Vector3 peak = (origin + target) / 2;
        float height = Vector3.Distance(origin, target);
        peak += new Vector3(0, height / 1.5f, 0);

        for (float ratio = 0; ratio <= 1; ratio += 1f / 5)
        {
            Vector3 pointA = Vector3.Lerp(origin, peak, ratio);
            Vector3 pointB = Vector3.Lerp(peak, target, ratio);
            Vector3 bezierPoint = Vector3.Lerp(pointA, pointB, ratio);
            trajectory.Add(bezierPoint);
        }
    }

    // 投げる処理
    Coroutine GimmickThrow;
    IEnumerator MazeGimmick_Throw()
    {
        Vector3[] trajectoryPoint = trajectory.ToArray();
        int point = 1;

        while (point < trajectoryPoint.Length)
        {
            Vector3 currentPos = targetFurniture.transform.position;
            targetFurniture.transform.position = Vector3.MoveTowards(currentPos, trajectoryPoint[point], 10f * Time.deltaTime);
            float distance = Vector3.Distance(targetFurniture.transform.position, trajectoryPoint[point]);
            if (distance < 0.1f)
            {
                point++;
            }
            yield return null;
        }

        if (enemiesManager.enemyMode == EnemiesManager.EnemyMode.Mode_Offensive)
        {
            if (mazeFurnitureType == MazeFurnitureType.Chair)
            {
                bool hit = Physics.CheckSphere(trajectoryPoint[trajectoryPoint.Length - 1], 2f, playerMask);

                if (hit && ghostCatch.mode == GhostCatch.Mode.Carry)
                {
                    ghostCatch.SetState(GhostCatch.Mode.Attacked);
                    enemiesManager.gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Collect);
                }
            }
        }

        targetFurniture.transform.position = targetPos;
        agent.isStopped = false;
        agent.speed = enemiesManager.speed;
        agent.angularSpeed = enemiesManager.angularSpeed;
        agent.acceleration = enemiesManager.accelSpeed;

        if (enemiesManager.enemyMode == EnemiesManager.EnemyMode.Mode_Offensive)
        {
            targetFurniture.transform.Find("Navmesh").gameObject.SetActive(true);
            NavMeshSurface surface = GameObject.Find("NavMeshPlayer").GetComponent<NavMeshSurface>();
            surface.BuildNavMesh();
        }

        // 終了
        mazeGimmickState = MazeGimmickState.None;
        mazeGimmick = false;
        targetFurniture = null;
        targetObj = null;
        trajectory.Clear();

        // 仮バグ修正
        patrolSet = false;
    }

    public void ClearMazeAction()
    {
        if (mazeGimmickState == MazeGimmickState.Throw)
        {
            return;
        }

        if (mazeFurnitureType == MazeFurnitureType.Bookstand)
        {
            targetFurniture.transform.SetParent(GameObject.Find("Furnitures").transform.Find("Bookstands"));
        }
        else if (mazeFurnitureType == MazeFurnitureType.Desk)
        {
            targetFurniture.transform.SetParent(GameObject.Find("Furnitures").transform.Find("Desks"));
        }
        else if (mazeFurnitureType == MazeFurnitureType.Chair)
        {
            targetFurniture.transform.SetParent(GameObject.Find("Furnitures").transform.Find("Chairs"));
        }

        targetFurniture.transform.position -= new Vector3(0, 1.5f, 0);

        agent.isStopped = false;
        agent.speed = enemiesManager.speed;
        agent.angularSpeed = enemiesManager.angularSpeed;
        agent.acceleration = enemiesManager.accelSpeed;

        mazeGimmickState = MazeGimmickState.None;
        mazeGimmick = false;
        targetFurniture = null;
        targetObj = null;
        trajectory.Clear();
    }

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
