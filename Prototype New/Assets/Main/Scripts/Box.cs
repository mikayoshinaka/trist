using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private float horizontalAngle = 0.0f;
    private float verticalAngle = 0.0f;
    private float setHorizontalAngle = 0.0f;
    private float setVerticalAngle = 0.0f;
    //private float y = 0.0f;
    [SerializeField] private float speed = 100.0f;
    //private float x = 0.0f;
    //private float z = 0.0f;
    private float underVerticalAngleLimit = 40.0f;
    private float upperVerticalAngleLimit = 10.0f;
    private float HorizontalAngleLimit = 90.0f;
    // Start is called before the first frame update
    void Start()
    {
        horizontalAngle = this.gameObject.transform.localEulerAngles.y;
        //y = this.gameObject.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }
        if (Input.GetAxisRaw("RHorizontal") < 0 || Input.GetKey("left"))
        {
            setHorizontalAngle -= Time.deltaTime * speed;
        }
        else if (0 < Input.GetAxisRaw("RHorizontal") || Input.GetKey("right"))
        {
            setHorizontalAngle += Time.deltaTime * speed;
        }
        if (Input.GetAxisRaw("RVertical") < 0 || Input.GetKey("up"))
        {
            setVerticalAngle -= Time.deltaTime * speed;
        }
        else if (0 < Input.GetAxisRaw("RVertical") || Input.GetKey("down"))
        {
            setVerticalAngle += Time.deltaTime * speed;
        }

        LimitAngle();
        //    x = ( Mathf.Sin((horizontalAngle) * Mathf.Deg2Rad));
        //    z = ( Mathf.Cos((horizontalAngle) * Mathf.Deg2Rad));
        //this.transform.localPosition = new Vector3(x, y, z);
        this.transform.localRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0.0f);
    }
    private void LimitAngle()
    {
        if (setHorizontalAngle > HorizontalAngleLimit || setHorizontalAngle < -HorizontalAngleLimit)
        {
            setHorizontalAngle = horizontalAngle;
        }
        else
        {
            horizontalAngle = setHorizontalAngle;
        }

        if (setVerticalAngle > underVerticalAngleLimit || setVerticalAngle < -upperVerticalAngleLimit)
        {
            setVerticalAngle = verticalAngle;
        }
        else
        {
            verticalAngle = setVerticalAngle;
        }
        if (horizontalAngle > 180.0f)
        {
            horizontalAngle -= 360.0f;
            setHorizontalAngle -= 360.0f;
        }
        else if (horizontalAngle < -180.0f)
        {
            horizontalAngle += 360.0f;
            setHorizontalAngle += 360.0f;
        }
    }

    //private void DefCamera()
    //{

    //}


}
