using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LiquidGhostScript : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemyBody;
    public NavMeshAgent agent;
    public static Transform player;
    public float upRange = 3f;
    Vector3 enemyVertical;
    public float attackRange = 2f;
    public LayerMask playerMask;
    public LayerMask stageMask;

    public bool PlayerInAttackRange;
    bool stateChange;

    // Patrolling
    //bool patrolSet;
    //public float patrolRange = 2f;
    //public Vector3 patrolPoint;
    //float timer, timeLimit;

    [SerializeField] ManagementScript managementScript;
    private int playerHP;

    public EnemiesManager enemiesManager;

    //小野澤　攻撃時に見える用
    [SerializeField] private GameObject enemyLiquidBody;
    public GhostChange ghostChange;
    // Start is called before the first frame update
    void Start()
    {
        enemy = this.gameObject;
        enemyBody = this.gameObject.transform.GetChild(0).gameObject;
        agent = enemy.GetComponent<NavMeshAgent>();
        player = GameObject.Find("PlayerController").transform;
        playerMask = LayerMask.GetMask("Player");
        stageMask = LayerMask.GetMask("Stages");
        enemyVertical = new Vector3(0, upRange, 0);
        stateChange = false;
        //patrolSet = false;

        //timer = 0f;
        //timeLimit = 5f;
        //攻撃中透明度変化　小野澤用
        ghostChange = GameObject.Find("Ghost").GetComponent<GhostChange>();

        enemiesManager = GameObject.Find("Enemies").GetComponent<EnemiesManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //小野澤ゲームオーバー用
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        // 攻撃判定
        PlayerInAttackRange = Physics.CheckCapsule(enemy.transform.position, enemy.transform.position + enemyVertical, attackRange, playerMask);

        // 攻撃処理
        if (PlayerInAttackRange)
        {
            if (!stateChange)
            {
                //enemyBody.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, enemyBody.GetComponent<Renderer>().material.color.a);
                stateChange = true;

                Attacking();

                // 驚かす処理、タイマー等、全ての敵が止まっている
                StartCoroutine(SurpriseAction(enemiesManager.worldStopTimer));

                //    攻撃タイマー　（プレイヤーを無視するタイマー）
                StartCoroutine(AttackCooldown(enemiesManager.attackCooldown));
            }
        }
        if (!PlayerInAttackRange && stateChange)
        {
            //enemyBody.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 1.0f, enemyBody.GetComponent<Renderer>().material.color.a);
            stateChange = false;
        }
        //else
        //{
        //    SetPatrol();
        //}
    }

    #region 攻撃

    void Attacking()
    {
        player.GetComponent<CharacterMovementScript>().FaceEnemy(this.transform);
    }

    IEnumerator SurpriseAction(float timer)
    {
        player.GetComponent<CharacterMovementScript>().playerInterupt = true;
        enemiesManager.attacking = true;
        //  キャラが見えるようにする　小野澤用
        ghostChange.AttackTransparent(enemyLiquidBody);

        yield return new WaitForSeconds(timer);

        player.GetComponent<CharacterMovementScript>().playerInterupt = false;
        enemiesManager.attacking = false;

        //　ダメージを受ける 攻撃後　小野澤用
        managementScript.PlayerMinusHP();
        playerHP = ManagementScript.GetPlayerHP();
        ghostChange.AttackedTransparent(enemyLiquidBody);
        if (playerHP <= 0)
        {
            SceneManagerScript.gameOver = true;
        }
    }

    // 攻撃タイマー
    IEnumerator AttackCooldown(float timer)
    {
        player.gameObject.layer = LayerMask.NameToLayer("Default");

        yield return new WaitForSeconds(timer);

        player.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    #endregion

    #region 巡回
    //void SetPatrol()
    //{
    //    if (!patrolSet)
    //    {
    //        Patrolling();
    //    }

    //    // Timer
    //    if (patrolSet)
    //    {
    //        timer += Time.deltaTime;
    //        if (timer >= timeLimit)
    //        {
    //            patrolSet = false;
    //        }
    //    }

    //    //if (patrolPoint == transform.position)
    //    //{
    //    //    patrolSet = false;
    //    //}
    //}

    //void Patrolling()
    //{
    //    float pointX = Random.Range(-patrolRange, patrolRange);
    //    float pointZ = Random.Range(-patrolRange, patrolRange);
    //    patrolPoint = new Vector3(transform.position.x + pointX, transform.position.y, transform.position.z + pointZ);

    //    if (Physics.Raycast(patrolPoint, -transform.up, 10f, stageMask))
    //    {
    //        agent.SetDestination(patrolPoint);
    //        patrolSet = true;
    //        timer = 0f;
    //    }
    //}

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
