using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ParalysisPlayer : MonoBehaviour
{

    private float time;
    [SerializeField] private float timeMax = 2.0f;
    public bool paralysis;
    // Start is called before the first frame update
    void Start()
    {
        time = 0.0f;
        paralysis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (paralysis == true && time < timeMax)
        {
            time += Time.deltaTime;
            this.gameObject.transform.parent.GetComponent<CharacterMovementScript>().enabled = false;
            this.gameObject.transform.parent.GetComponent<NavMeshAgent>().isStopped = true;
        }
        else if (paralysis == true && time >= timeMax)
        {
            paralysis = false;
            time = 0.0f;
            this.gameObject.transform.parent.GetComponent<CharacterMovementScript>().enabled = true;
            this.gameObject.transform.parent.GetComponent<NavMeshAgent>().isStopped = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireBall")
        {
            paralysis = true;
        }
    }
}
