using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySearchScript : MonoBehaviour
{
    #region Initialize 初期化

    public GameObject enemy;
    public NavMeshAgent agent;
    public static Transform player;
    public LayerMask playerMask;
    public LayerMask stageMask;

    // Patrol
    public Vector3 patrolPoint;
    bool patrolSet;
    public float patrolRange = 10f;
    float timer;
    public float timeLimit;
    public enum PatrolType
    {
        Random,
        Post
    }
    public PatrolType patrol;
    public Transform[] patrolPost;
    int postNum;

    // States
    public static float sightRange = 7.5f, attackRange = 2f;
    public bool playerInSightRange, playerInAttackRange;

    public bool monkeyChase;
    float time;
    public float monkeyChaseTime = 4.0f;

    [SerializeField] ManagementScript managementScript;
    private int playerHP;
    //小野澤　攻撃時に見える用
    [SerializeField] private GameObject enemyBody;

    public GhostChange ghostChange;
    
    // 攻撃
    public EnemiesManager enemiesManager;
    bool moveAway;

    void Start()
    {
        player = GameObject.Find("PlayerController").transform;
        enemy = this.gameObject;
        //Debug.Log(enemy.name);
        agent = GetComponent<NavMeshAgent>();
        playerMask = LayerMask.GetMask("Player");
        stageMask = LayerMask.GetMask("Stages");

        patrolSet = false;
        postNum = 0;
        timer = 0f;
        timeLimit = 5f;

        enemiesManager = GameObject.Find("Enemies").GetComponent<EnemiesManager>();
        moveAway = false;
    }

    #endregion

    void Update()
    {
        //小野澤ゲームオーバー用
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        // 当たり判定

        // 索敵判定
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);

        // 攻撃判定
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        // 敵の移動を中止する
        if (enemiesManager.attacking)
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
            }
        }
        // プレイヤーから離れる
        else if (moveAway)
        {
            MoveAway();
        }
        else
        {
            // 移動を進む
            if (agent.isStopped)
            {
                agent.isStopped = false;
            }

            //lŒ`—p
            if (monkeyChase)
            {
                Chasing();
                MonkeyTime();
            }
            else
            {
                // 巡回
                SetPatrol();

                // 索敵
                SetChase();
            }

            // 攻撃
            if (playerInSightRange && playerInAttackRange)
            {
                // 攻撃処理
                Attacking();

                // 驚かす処理、タイマー等、全ての敵が止まっている
                StartCoroutine(SurpriseAction(enemiesManager.worldStopTimer));

                //    攻撃タイマー　（プレイヤーを無視するタイマー）
                StartCoroutine(AttackCooldown(enemiesManager.attackCooldown));
            }
        }
    }

    #region 巡回

    // 巡回設定
    void SetPatrol()
    {
        if ((!playerInSightRange && !playerInAttackRange) || !enemy.GetComponent<AISightScript>().detected)
        {
            //if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.JoystickButton2))
            //{
            //    if (patrol == PatrolType.Random)
            //    {
            //        patrol = PatrolType.Post;
            //    }
            //    else
            //    {
            //        patrol = PatrolType.Random;
            //    }
            //}

            if (!patrolSet)
            {
                Patrolling();
            }

            // Timer
            if (patrolSet)
            {
                timer += Time.deltaTime;
                if (timer >= timeLimit)
                {
                    patrolSet = false;
                }
            }

            if (patrolPoint == transform.position)
            {
                patrolSet = false;
            }
        }
        else if (patrolSet)
        {
            patrolSet = false;
        }
    }

    // 巡回処理
    void Patrolling()
    {
        if (patrol == PatrolType.Random)
        {
            float pointX = Random.Range(-patrolRange, patrolRange);
            float pointZ = Random.Range(-patrolRange, patrolRange);
            patrolPoint = new Vector3(transform.position.x + pointX, transform.position.y, transform.position.z + pointZ);

            if (Physics.Raycast(patrolPoint, -transform.up, 10f, stageMask))
            {
                agent.SetDestination(patrolPoint);
                patrolSet = true;
                timer = 0f;
            }
        }
        else if (patrol == PatrolType.Post)
        {
            postNum %= patrolPost.Length;

            agent.SetDestination(patrolPost[postNum].position);
            postNum++;
            patrolSet = true;
            timer = 0f;
        }

        //if (this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color != new Color(0.0f, 0.0f, 1.0f, this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color.a))
        //{
        //    this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 1.0f, this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color.a);
        //}
    }

    #endregion

    #region 追走

    // 索敵の設定
    void SetChase()
    {
        if (playerInSightRange && !playerInAttackRange)
        {
            enemy.GetComponent<AISightScript>().inView = true;
            //AISightScript.inView = true;
            if (enemy.GetComponent<AISightScript>().detected)
            {
                Chasing();
            }
        }
        else if (enemy.GetComponent<AISightScript>().inView)
        {
            enemy.GetComponent<AISightScript>().inView = false;
        }
    }

    void Chasing()
    {
        agent.SetDestination(player.position);
        //if (this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color != new Color(1.0f, 0.92f, 0.016f, this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color.a))
        //{
        //    this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1.0f, 0.92f, 0.016f, this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color.a);
        //}
    }

    #endregion

    #region 攻撃

    // 攻撃処理
    void Attacking()
    {
        agent.SetDestination(player.position);
        //this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color.a);

        player.GetComponent<CharacterMovementScript>().FaceEnemy(this.transform);
    }

    IEnumerator SurpriseAction(float timer)
    {
        player.GetComponent<CharacterMovementScript>().playerInterupt = true;
        enemiesManager.attacking = true;
        //  キャラが見えるようにする　小野澤用
        //ghostChange.AttackTransparent(enemyBody);

        yield return new WaitForSeconds(timer);

        player.GetComponent<CharacterMovementScript>().playerInterupt = false;
        enemiesManager.attacking = false;

        // ダメージを受ける 攻撃後　小野澤用
        //managementScript.PlayerMinusHP();
        //playerHP = ManagementScript.GetPlayerHP();
        //ghostChange.AttackedTransparent(enemyBody);

        if (playerHP <= 0)
        {
            SceneManagerScript.gameOver = true;
        }

        // 離れる処理
        StartCoroutine(StayAway(2f));
    }

    // 攻撃後、離れる処理
    IEnumerator StayAway(float timer)
    {
        moveAway = true;
        SetAwayDirection();
        yield return new WaitForSeconds(timer);
        moveAway = false;
    }
    private Vector3 awayDirection;
    void SetAwayDirection()
    {
        awayDirection = Vector3.Scale(transform.position - player.transform.position, new Vector3(1f, 0f, 1f)).normalized;
        Debug.Log(agent.isStopped);
    }
    void MoveAway()
    {
        agent.Move(awayDirection * agent.speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(awayDirection, transform.up);
    }

    // 攻撃タイマー
    IEnumerator AttackCooldown(float timer)
    {
        player.gameObject.layer = LayerMask.NameToLayer("Default");

        yield return new WaitForSeconds(timer);

        player.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    #endregion

    void MonkeyTime()
    {
        time += Time.deltaTime;
        if (time > monkeyChaseTime)
        {
            time = 0.0f;
            monkeyChase = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, patrolRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
