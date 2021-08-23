using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySearchScript : MonoBehaviour
{
    public GameObject enemy;
    public NavMeshAgent agent;
    public static Transform player;
    public LayerMask playerMask;
    public LayerMask stageMask;

    // Distraction
    public static bool distraction;
    bool searching;
    Vector3 searchPos;

    // Patrol
    public Vector3 patrolPoint;
    bool patrolSet;
    public float patrolRange = 10f;
    float timer;
    public float timeLimit;
    enum PatrolType
    {
        Random,
        Post
    }
    PatrolType patrol;
    public Transform[] patrolPost = new Transform[3];
    int postNum;

    // States
    public static float sightRange = 7.5f, attackRange = 2f;
    public bool playerInSightRange, playerInAttackRange;

    //人形用
    public bool monkeyChase;
    float time;
    public float monkeyChaseTime = 4.0f;
    //プレイヤーHP用
    [SerializeField] ManagementScript managementScript;
    private int playerHP;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerController").transform;
        enemy = this.gameObject;
        //Debug.Log(enemy.name);
        agent = GetComponent<NavMeshAgent>();
        playerMask = LayerMask.GetMask("Player");
        stageMask = LayerMask.GetMask("Stages");

        distraction = false;
        searching = false;

        patrol = PatrolType.Random;     // patrol type at start
        patrolSet = false;
        postNum = 0;
        timer = 0f;
        timeLimit = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (distraction)
        {
            SearchDistraction();

            // todo SetChase() and set distraction to false
        }
        else
        {

            //人形用
            if (monkeyChase)
            {
                Chasing();
                MonkeyTime();
            }
            else
            {
                // PATROLLING
                SetPatrol();

                // CHASING
                SetChase();
            }

            // ATTACKING
            if (playerInSightRange && playerInAttackRange)
            {
                Attacking();

                managementScript.PlayerMinusHP();
                playerHP = ManagementScript.GetPlayerHP();
                if (playerHP <= 0)
                {
                    SceneManagerScript.gameOver = true;
                }
            }
        }
    }

    void SearchDistraction()
    {
        if (!searching)
        {
            agent.SetDestination(player.position);
            searchPos = new Vector3(player.position.x, player.position.y, transform.position.z);
            searching = true;
        }

        float posX = transform.position.x;
        float posY = transform.position.y;
        float checkX = (posX * posX) - (searchPos.x * searchPos.x);
        float checkY = (posY * posY) - (searchPos.y * searchPos.y);
        if (checkX >= -0.5f && checkX <= 0.5 && checkY >= -0.5f && checkY <= 0.5f)
        {
            searching = false;
            distraction = false;
        }
    }

    void SetPatrol()
    {
        if ((!playerInSightRange && !playerInAttackRange) || !enemy.GetComponent<AISightScript>().detected)
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                if (patrol == PatrolType.Random)
                {
                    patrol = PatrolType.Post;
                }
                else
                {
                    patrol = PatrolType.Random;
                }
            }

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
            if (postNum > 2)
            {
                postNum = 0;
            }

            agent.SetDestination(patrolPost[postNum].position);
            postNum++;
            patrolSet = true;
            timer = 0f;
        }

        if (this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color != Color.blue)
        {
            this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
        }
    }

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
        if (this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color != Color.yellow)
        {
            this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.yellow;
        }
    }

    void Attacking()
    {
        agent.SetDestination(player.position);
        this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
    }
    //人形用追加
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
