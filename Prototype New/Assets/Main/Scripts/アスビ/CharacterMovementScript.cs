using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementScript : MonoBehaviour
{
    public CharacterController controller;   
    public Transform playerCamera;

    bool move, fly;
    [Header ("Move")]
    public float speed = 10f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Move")]
    public bool enableFlying;
    public float flySpeed = 2f;

    [Header("Gameplay")]
    public bool playerInterupt;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GameObject.Find("Cameras").transform.Find("Main Camera");
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
        if (enableFlying)
        {
            fly = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift) ? true : false;
        }

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 移動処理
        if (!playerInterupt)
        {
            if (move)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
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
    }

    // 敵の方向に向かう
    public void FaceEnemy(Transform enemy)
    {
        Vector3 direction = Vector3.Scale((enemy.position - transform.position), new Vector3(1, 0, 1)).normalized;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
