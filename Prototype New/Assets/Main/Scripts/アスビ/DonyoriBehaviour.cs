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
    [SerializeField] GameObject enemyCanvas;
    [SerializeField] GameObject enemyAura;
    [SerializeField] GhostCatch ghostCatch;

    [Header("�����蔻��")]
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
        donyoriManager = transform.parent.GetComponent<DonyoriManager>();
        enemy = this.gameObject;
        enemyBody = enemy.transform.Find("EnemyBody").gameObject;
        enemySight = GetComponent<EnemySight>();

        agent = GetComponent<NavMeshAgent>();
        playerMask = LayerMask.GetMask("PlayerTrigger");
        stageMask = LayerMask.GetMask("Stages");
        upstageMask = LayerMask.GetMask("UpStages");

        agent.speed = donyoriManager.speed;
        patrolRange = donyoriManager.patrolRange;
        sightRange = donyoriManager.sightRange;
        attackRange = donyoriManager.attackRange;

        enemyAnimator = enemyBody.GetComponent<Animator>();
        enemyCanvas = enemy.transform.Find("EnemyCanvas").gameObject;
        enemyAura = enemy.transform.Find("EnemyAura").gameObject;
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
        // �����蔻��
        // ���G����
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange * chaseRange, playerMask);

        // �U������
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (gimmickAction)
        {
            // Gimmick_Update();
        }
        else if (mazeGimmick)
        {
            // ���H
            //MazeUpdate();
        }
        else if (moveAway)
        {
            // �v���C���[���瓦����
            MoveAway();
        }
        else
        {
            // ���R�ړ�
            Patrol();

            // ���G
            Locate();

            // �U��
            Action();
        }
    }

    // �G�̃p�����[�^�[����
    void ActionAdjustment()
    {
        // �m�[�}���X�s�[�h
        if (enemyState == EnemyState.Patrol)
        {
            chasing = false;
            agent.speed = donyoriManager.speed;
            chaseRange = 1f;
            enemySight.chaseAngle = 1f;
        }
        // �X�s�[�h�オ��
        else if (enemyState == EnemyState.Locate)
        {
            chasing = true;
            agent.speed = donyoriManager.chaseSpeed;
            chaseRange = 2f;
            enemySight.chaseAngle = 3f;
        }
    }

    #region ���R�ړ�

    // �ݒ�
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

    // �ړ��ݒ�
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

    // �ړ��ύX�^�C�}�[
    Coroutine patrolling;
    IEnumerator Patrolling()
    {
        yield return new WaitForSeconds(4f);
        patrolSet = false;
    }

    #endregion

    #region ���G

    // �ݒ�
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
                else if (donyoriManager.enemyMode == DonyoriManager.EnemyMode.Mode_Offensive)
                {
                    // Chase();
                }
            }
        }
        else if (enemySight.inView)
        {
            enemySight.inView = false;
        }
    }

    // �v���C���[��ǂ�������
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

    #region �U��

    // �ݒ�
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
            else if (donyoriManager.enemyMode == DonyoriManager.EnemyMode.Mode_Offensive)
            {
                // Offense();
            }
        }   
    }

    // �v���C���[�Ɠ����������ɓ�����
    void Defense()
    {
        Runaway(3f);
    }

    // �v���C���[���U������M�~�b�N
    void Offense()
    {

        //
        // �v���C���[�Ɠ��������� //
        if (ghostCatch.mode==GhostCatch.Mode.Carry) {
            ghostCatch.SetState(GhostCatch.Mode.Attacked);
        }
        // �����ɃX�|�[���������Ă� //
        // 

        donyoriManager.gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Collect);
    }

    #endregion

    #region �����s��

    // �v���C���[���瓦����
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

    // ������^�C�}�[
    Coroutine runningAway;
    IEnumerator RunningAway(float time)
    {
        enemyAnimator.SetBool("Running", true);
        agent.isStopped = true;
        SetAwayDirection();
        moveAway = true;
        yield return new WaitForSeconds(time);
        enemyAnimator.SetBool("Running", false);
        if (enemyAura.activeInHierarchy)
        {
            enemyAura.SetActive(false);
        }
        agent.isStopped = false;
        moveAway = false;
    }

    // �v���C���[���痣������
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

    #region �F�M�~�b�N

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
                    target.GetComponent<EnemyBehaviour>().Gimmick_DarkRed();
                }
                else if (target.tag == "DonyoriGhost")
                {
                    target.GetComponent<DonyoriBehaviour>().Gimmick_DarkRed();
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

}
