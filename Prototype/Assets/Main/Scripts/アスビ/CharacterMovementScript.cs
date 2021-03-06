using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementScript : MonoBehaviour
{
    public CharacterController controller;
    public GhostChange ghostChange;
    public Transform playerCamera;

    public DoorView doorView;

    bool move, fly;
    public float speed = 5f;
    public float flySpeed = 2f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public bool playerInterupt;

    private void Start()
    {
        ghostChange = GameObject.Find("Ghost").GetComponent<GhostChange>();
        doorView = GameObject.Find("Door Gimmick").GetComponent<DoorView>();
        playerInterupt = false;
    }

    void Update()
    {
        //小野澤ゲームオーバー用
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        move = horizontal == 0 && vertical == 0 ? false : true;
        fly = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift) ? true : false;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 移動処理
        if (!playerInterupt && !doorView.gimmickPlay && !ghostChange.canPossess && !ghostChange.leave)
        {
            if (move)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                if (Input.GetKey(KeyCode.Space))
                {
                    moveDir.y += flySpeed / 2;
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    moveDir.y -= flySpeed / 2;
                }
                controller.Move(moveDir * speed * Time.deltaTime);
            }

            // 上下移動
            if (fly && !move)
            {
                Vector3 flying = new Vector3(0, flySpeed * 2, 0);
                if (Input.GetKey(KeyCode.Space))
                {
                    controller.Move(flying * Time.deltaTime);
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    controller.Move(-flying * Time.deltaTime);
                }
            }
        }

        // EnemySearchScript Distraction
        //if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton1))
        //{
        //    EnemySearchScript.distraction = true;
        //}
    }

    // 敵の方向に向かう
    public void FaceEnemy(Transform enemy)
    {
        Vector3 direction = Vector3.Scale((enemy.position - transform.position), new Vector3(1, 0, 1)).normalized;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
