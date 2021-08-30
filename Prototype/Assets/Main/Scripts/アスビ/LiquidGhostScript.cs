using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LiquidGhostScript : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemyBody;
    public NavMeshAgent agent;
    public float upRange = 3f;
    Vector3 enemyVertical;
    public float attackRange = 2f;
    public LayerMask playerMask;
    public LayerMask stageMask;

    public bool PlayerInAttackRange;
    bool stateChange;

    // Patrolling
    bool patrolSet;
    public float patrolRange = 2f;
    public Vector3 patrolPoint;
    float timer, timeLimit;

    // Start is called before the first frame update
    void Start()
    {
        enemy = this.gameObject;
        enemyBody = this.gameObject.transform.GetChild(0).gameObject;
        agent = enemy.GetComponent<NavMeshAgent>();
        playerMask = LayerMask.GetMask("Player");
        stageMask = LayerMask.GetMask("Stages");
        enemyVertical = new Vector3(0, upRange, 0);
        stateChange = false;
        patrolSet = false;
        
        timer = 0f;
        timeLimit = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInAttackRange = Physics.CheckCapsule(enemy.transform.position, enemy.transform.position + enemyVertical, attackRange, playerMask);

        if (PlayerInAttackRange)
        {
            if (!stateChange)
            {
                enemyBody.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, enemyBody.GetComponent<Renderer>().material.color.a);
                stateChange = true;
            }
        }
        if (!PlayerInAttackRange && stateChange)
        {
            enemyBody.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 1.0f, enemyBody.GetComponent<Renderer>().material.color.a);
            stateChange = false;
        }
        else
        {
            SetPatrol();
        }
    }

    void SetPatrol()
    {
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

        //if (patrolPoint == transform.position)
        //{
        //    patrolSet = false;
        //}
    }

    void Patrolling()
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
