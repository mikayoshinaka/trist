using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ParalysisEnemy : MonoBehaviour
{
    private float time;
    private float timeMax = 3.0f;
    bool paralysis;
    // Start is called before the first frame update
    void Start()
    {
        time = 0.0f;
        paralysis = false;
    }

    // Update is called once per frame
    void Update()
    {
       if(paralysis==true && time < timeMax)
       {
            time += Time.deltaTime;
            if (this.gameObject.tag == "NormalGhost")
            {
                this.gameObject.GetComponent<EnemyBehaviour>().enabled = false;
            }
            else if (this.gameObject.tag == "DonyoriGhost")
            {
                this.gameObject.GetComponent<DonyoriBehaviour>().enabled = false;
            }

            this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
       }
       else if(paralysis == true && time >= timeMax)
       {
            paralysis = false;
            time = 0.0f;
            if (this.gameObject.tag == "NormalGhost")
            {
                this.gameObject.GetComponent<EnemyBehaviour>().enabled = true;
            }
            else if (this.gameObject.tag == "DonyoriGhost")
            {
                this.gameObject.GetComponent<DonyoriBehaviour>().enabled = true;
            }
            this.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
       }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AttackBook")
        {
            paralysis = true;
        }
    }
}
