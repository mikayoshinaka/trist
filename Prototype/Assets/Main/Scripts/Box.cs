using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private float radius=0.0f;
    private float angle=0.0f;
    private float y = 0.0f;
    [SerializeField] private float speed = 100.0f;
    private float x = 0.0f;
    private float z = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        angle = this.gameObject.transform.localEulerAngles.y;
        y = this.gameObject.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }
        if (Input.GetAxisRaw("RHorizontal") < 0)
        {
            angle -= Time.deltaTime * speed;
        }
        else if (0 < Input.GetAxisRaw("RHorizontal"))
        {
            angle += Time.deltaTime * speed;
        }
        if (Input.GetKey("right"))
        {
            angle += Time.deltaTime * speed;
        }
        else if (Input.GetKey("left"))
        {
            angle -= Time.deltaTime * speed;

        }
        if (angle>360.0f)
       {
            angle -= 360.0f;
       }
       else if(angle<0.0f)
       {
            angle += 360.0f;
       }
            x = (radius * Mathf.Sin((angle) * Mathf.Deg2Rad));
            z = (radius * Mathf.Cos((angle) * Mathf.Deg2Rad));
        this.transform.localPosition = new Vector3(x, y, z);
        this.transform.localRotation = Quaternion.Euler(0.0f,angle,0.0f);
    }
}
