using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelf : MonoBehaviour
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
    public List<GameObject> enterObject = new List<GameObject>();
    [SerializeField] private float coefficient;
    [SerializeField] private float explosionSpeed;
    public bool fly;
    AudioSource audioSource;
    public AudioClip bookAttackSE;
    public GameObject bookSound;
    // Start is called before the first frame update
    void Start()
    {
        fly = false;
        horizontalAngle = this.gameObject.transform.localEulerAngles.y;
        audioSource = bookSound.GetComponent<AudioSource>();
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
        if((Input.GetKey(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton1)) && fly ==false)
        {
            fly = true;
            audioSource.PlayOneShot(bookAttackSE);
            FlyBook();
        }

        this.transform.localRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0.0f);
    }
    //視点移動の角度
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
    //本を飛ばす
    private void FlyBook()
    {
        Vector3[] velocity = new Vector3[enterObject.Count];
        for (int i = 0; i < enterObject.Count; i++)
        {
            velocity[i] = explosionSpeed * transform.forward;
            enterObject[i].GetComponent<Rigidbody>().isKinematic = false;
            enterObject[i].transform.parent = null;
            GameObject.Find("ClearScene").GetComponent<ClearScene>().flewBook.Add(enterObject[i]);
            enterObject[i].tag = "AttackBook";
            enterObject[i].GetComponent<Rigidbody>().AddForce(coefficient * velocity[i]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Book")
        {
            Debug.Log("hit");
            if (!enterObject.Contains(other.gameObject))
            {
                enterObject.Add(other.gameObject);

            }
        }


    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Book")
        {
            if (enterObject.Contains(other.gameObject))
            {
                enterObject.Remove(other.gameObject);

            }
        }
    }
}
