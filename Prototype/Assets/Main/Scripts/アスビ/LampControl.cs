using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampControl : MonoBehaviour
{
    [Header("Assign Manually")]
    public GameObject lamp;
    public Light lampLight;
    public LightCollider lightCollider;   
    bool move;
    public float speed = 100f;
    float horizontalAngle;
    float verticalAngle;

    private void Start()
    {
        horizontalAngle = this.gameObject.transform.localEulerAngles.y;
        verticalAngle = this.gameObject.transform.localEulerAngles.x;
    }

    private void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        // 操作処理
        LightControl();
    }

    // ランプの操作
    void LightControl()
    {
        float horizontal = Input.GetAxisRaw("RHorizontal");
        float vertical = Input.GetAxisRaw("RVertical");

        move = horizontal == 0 && vertical == 0 ? false : true;

        if (move)
        {
            if (horizontal > 0)
            {
                horizontalAngle += speed * Time.deltaTime;
            }
            else if (horizontal < 0)
            {
                horizontalAngle -= speed * Time.deltaTime;
            }

            if (vertical > 0 && verticalAngle <= 90f)
            {
                verticalAngle += speed * Time.deltaTime;
            }
            else if (vertical < 0 && verticalAngle > 0f)
            {
                verticalAngle -= speed * Time.deltaTime;
            }

            this.transform.localRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0.0f).normalized;
        }

        // Light.intensityと判定範囲の調整
        if (verticalAngle >= 0f && verticalAngle <= 90f)
        {
            lampLight.intensity = 1f + (90f - verticalAngle) / 90f * 19f;
            lightCollider.lightRange = 1f + (90f - verticalAngle) / 90f * 19f;
            lightCollider.OnValidate();
        }
    }
}
