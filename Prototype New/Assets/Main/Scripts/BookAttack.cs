using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAttack : MonoBehaviour
{
    private float time;
    private float timeMax=3.0f;
    [SerializeField] ParalysisEnemy paralysisEnemy;
    bool attack;
    // Start is called before the first frame update
    void Start()
    {
        time = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.GetComponent<Rigidbody>().isKinematic==false)
        {
            attack = true;
            time += Time.deltaTime;
        }
        if(time>=timeMax)
        {
            attack = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy"&&attack==true)
        {
           
        }
    }


   
}
