using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovementScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform playerCamera;
    public Animator animator;

    bool move;
    [Header ("Move")]
    public float speed = 10f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Gameplay")]
    public bool playerInterupt;
    //小野澤 終了用
    [SerializeField] private DollSave dollSave;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerCamera = GameObject.Find("Cameras").transform.Find("Main Camera");
        animator = transform.Find("PlayerBody").GetComponent<Animator>();
        playerInterupt = false;
    }

    void Update()
    {
        //小野澤　開始用 終了用
        if (GameObject.Find("BeforeBegin").GetComponent<BeforeBegin>().begin == true|| dollSave.bossIn == true )
        {
            return;
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        move = horizontal == 0 && vertical == 0 ? false : true;

        if (!move && animator.GetBool("Moving"))
        {
            animator.SetBool("Moving", false);
        } 

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 移動処理
        if (!playerInterupt)
        {
            if (move)
            {
                if (!animator.GetBool("Moving"))
                {
                    animator.SetBool("Moving", true);
                }

                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //ボス掴み用　小野澤
                if (GameObject.Find("CatchArea").GetComponent<GhostCatch>().bossGrab == true && GameObject.Find("BossEnemy").GetComponent<BossEnemy>().bossHP > 0)
                {
                    transform.LookAt(new Vector3(GameObject.Find("BossEnemy").transform.position.x, this.transform.position.y, GameObject.Find("BossEnemy").transform.position.z));
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                agent.Move(moveDir * speed * Time.deltaTime);
            }
        }
    }

    // 敵の方向に向かう
    public void FaceEnemy(Transform enemy)
    {
        Vector3 direction = Vector3.Scale((enemy.position - transform.position), new Vector3(1, 0, 1)).normalized;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
