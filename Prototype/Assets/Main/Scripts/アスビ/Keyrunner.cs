using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Keyrunner : MonoBehaviour
{
    public NavMeshAgent agent;
    public static Transform player;
    public LayerMask playerMask;
    public LayerMask stageMask;

    public float detectRange = 5f;
    public bool playerInRange;
    
    public enum KeyRunnerType
    {
        type1,
        type2,
        type3,
        type4
    };
    [Header("逃げ回る人形の種類")]
    public KeyRunnerType keyRunnerType;

    private Transform keyrunnerSpot;
    private Transform[] spot;
    private bool switchSpot;

    private Transform liquidGhosts;
    private Transform[] ghostSpot;
    private int currentSpot;
    public float type3Timer = 20f;

    private Transform doorFrontSpot;
    private Transform[] doorSpot;
    public float type4Timer = 15f;

    private void Start()
    {
        player = GameObject.Find("PlayerController").transform;
        agent = GetComponent<NavMeshAgent>();
        playerMask = LayerMask.GetMask("Player");
        stageMask = LayerMask.GetMask("Stages");
        
        // Type 1 & Type 2
        keyrunnerSpot = GameObject.Find("KeyrunnerSpot").transform;
        spot = new Transform[keyrunnerSpot.childCount];
        for (int i = 0; i < spot.Length; i++)
        {
            spot[i] = keyrunnerSpot.GetChild(i);
        }
        switchSpot = false;

        // Type 2
        if (keyRunnerType == KeyRunnerType.type2)
        {
            agent.acceleration *= 1.5f;
        }

        // Type 3
        liquidGhosts = GameObject.Find("LiquidGhosts").transform;
        ghostSpot = new Transform[liquidGhosts.childCount];
        for (int i = 0; i < ghostSpot.Length; i++)
        {
            ghostSpot[i] = liquidGhosts.GetChild(i);
        }
        currentSpot = 0;

        // Type 4
        doorFrontSpot = GameObject.Find("DoorFrontSpot").transform;
        doorSpot = new Transform[doorFrontSpot.childCount];
        for (int i = 0; i < spot.Length; i++)
        {
            doorSpot[i] = doorFrontSpot.GetChild(i);
        }
    }

    private void Update()
    {
        // 鍵が見えるとき
        if (GetComponent<MeshRenderer>().enabled)
        {
            playerInRange = Physics.CheckSphere(transform.position, detectRange, playerMask);

            // プレイヤーから逃げる
            if (playerInRange && keyRunnerType != KeyRunnerType.type3)
            {
                RunAway(keyRunnerType);
            }
        }

        // Type3の場合
        if (keyRunnerType == KeyRunnerType.type3)
        {
            RunAway(keyRunnerType);
        }
    }

    #region KeyRunnerTypeのメソッド

    void RunAway(KeyRunnerType keyRunnerType)
    {
        if (keyRunnerType == KeyRunnerType.type1)
        {
            KeyRunnerType_1();
        }
        else if (keyRunnerType == KeyRunnerType.type2)
        {
            KeyRunnerType_2();
        }
        else if (keyRunnerType == KeyRunnerType.type3)
        {
            KeyRunnerType_3();
        }
        else if (keyRunnerType == KeyRunnerType.type4)
        {
            KeyRunnerType_4();
        }
    }

    // 普通に追いかけたら逃げるタイプ
    void KeyRunnerType_1()
    {
        //Debug.Log("KeyRunnerType_1");

        Vector3 direction = Vector3.Scale((transform.position - player.transform.position), new Vector3(1, 0, 1)).normalized;
        if (!switchSpot)
        {
            ChooseMoveSpot(direction);
        }
    }

    // 1 よりも素早く逃げるタイプ
    void KeyRunnerType_2()
    {
        //Debug.Log("KeyRunnerType_2");

        Vector3 direction = Vector3.Scale((transform.position - player.transform.position), new Vector3(1, 0, 1)).normalized;
        agent.speed *= 1.5f;
        if (!switchSpot)
        {
            ChooseMoveSpot(direction);
        }
    }

    // どんよりおばけの影に隠れながら逃げるタイプ。毎 timer秒 で他のどんよりおばけの影に隠れる。
    void KeyRunnerType_3()
    {
        //Debug.Log("KeyRunnerType_3");

        if (!switchSpot)
        {
            ChooseGhostSpot();
        }
    }

    // 近づいたら他のドア目の前に瞬間移動して逃げるタイプ。連続発動は出来ない。
    void KeyRunnerType_4()
    {
        //Debug.Log("KeyRunnerType_4");

        if (!switchSpot)
        {
            ChooseTeleportSpot();
        }
    }

    #endregion

    #region Type1 & Type2   逃げる方向の計算

    //　逃げ回る人形の移動方向（Spot 1 ~ 4）
    void ChooseMoveSpot(Vector3 direction)
    {
        int chosenSpot = 0;
        float chosenSpotAngle = 0f;

        for (int i = 0; i < spot.Length; i++)
        {
            Vector3 spotDirection = spot[i].position - transform.position;
            float angleToSpot = Vector3.Angle(direction, spotDirection);
            //Debug.Log(angleToSpot);
            if (i == 0)
            {
                chosenSpot = i;
                chosenSpotAngle = angleToSpot;
            }
            else
            {
                if (angleToSpot < chosenSpotAngle)
                {
                    chosenSpot = i;
                    chosenSpotAngle = angleToSpot;
                }
            }
        }

        agent.SetDestination(spot[chosenSpot].position);
        StartCoroutine(SwitchSpotTimer());
    }

    //　移動方向を変えるタイマー
    IEnumerator SwitchSpotTimer()
    {
        switchSpot = true;
        yield return new WaitForSeconds(3f);
        switchSpot = false;
    }

    #endregion

    #region Type 3  どんよりお化け方向への移動

    void ChooseGhostSpot()
    {
        int nextSpot = ++currentSpot;
        nextSpot %= ghostSpot.Length;

        agent.SetDestination(ghostSpot[nextSpot].position);
        currentSpot = nextSpot;

        StartCoroutine(SwitchGhostSpot());
    }

    IEnumerator SwitchGhostSpot()
    {
        switchSpot = true;
        yield return new WaitForSeconds(type3Timer);
        switchSpot = false;
    }

    #endregion

    #region Type 4  ドア前に瞬間移動 

    void ChooseTeleportSpot()
    {
        int nextSpot = ++currentSpot;
        nextSpot %= doorSpot.Length;

        agent.Warp(doorSpot[nextSpot].position);
        currentSpot = nextSpot;

        StartCoroutine(TeleportTimer());
    }

    IEnumerator TeleportTimer()
    {
        switchSpot = true;
        yield return new WaitForSeconds(type4Timer);
        switchSpot = false;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
