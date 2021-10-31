using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollSave : MonoBehaviour
{
    [SerializeField] private GameObject catchArea;
    public List<GameObject> dolls = new List<GameObject>();
    bool within;
    // Start is called before the first frame update
    void Start()
    {
        within = false;
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKey(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton0)) && within ==true&&catchArea.GetComponent<GhostCatch>().doll!=null)
        {
            dolls.Add(catchArea.GetComponent<GhostCatch>().doll);
            catchArea.GetComponent<GhostCatch>().ReSetCatch();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBody")
        {
            within = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerBody")
        {
            within = false;
        }
    }
}
