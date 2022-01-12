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
    [SerializeField] GhostCatch ghostCatch;

    [Header("�����蔻��")]
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
        // �����蔻��
        // ���G����
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange * chaseRange, playerMask);

        // �U������
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (gimmickAction)
        {
            // Gimmick_Update();
        }
        else if (moveAway)
        {
            // �v���C���[���瓦����
            MoveAway();
        }
        else if (donyoriManager.enemyMode == DonyoriManager.EnemyMode.Mode_Offensive)
        {
            // �Ƌ��T��
            SearchObject();

            // �Ƌ���Ƃ��
            Possessing();
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
            agent.speed = donyoriManager.speed;
            chaseRange = 1f;
            enemySight.chaseAngle = 1f;
        }
        // �X�s�[�h�オ��
        else if (enemyState == EnemyState.Locate)
        {
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
            }
        }
        else if (enemySight.inView)
        {
            enemySight.inView = false;
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
        }   
    }

    // �v���C���[�Ɠ����������ɓ�����
    void Defense()
    {
        Runaway(3f);
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

    #region �ǂ�����ʍs��

    // �ʏ�ɖ߂�
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
        agent.speed = donyoriManager.speed;
    }

    // �Ƌ��T��
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

    // ���o�O�C��
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

    Coroutine BookSplat;
    IEnumerator OnBookSplat()
    {
        GameObject booksplatter = targetPossession.transform.Find("BookSplatter").gameObject;
        cooldown = true;
        yield return new WaitForSeconds(0.5f);
        booksplatter.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        // �����蔻��
        if (enemySight.detected && playerInSightRange)
        {
            player.transform.Find("PlayerBody").GetComponent<ParalysisPlayer>().paralysis = true;
        }

        yield return new WaitForSeconds(2f);
        booksplatter.SetActive(false);
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
        DonyoriReset();
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
        DonyoriReset();
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

    #region Dark Yellow
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
        DonyoriReset();
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
        DonyoriReset();
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
        DonyoriReset();
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
