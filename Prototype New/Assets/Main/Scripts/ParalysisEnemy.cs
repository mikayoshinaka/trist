using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ParalysisEnemy : MonoBehaviour
{
    public List<GameObject> paralysisObj = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (paralysisObj.GetComponent<NavMeshAgent>().isActiveAndEnabled)
        //{
        //    paralysisObj.GetComponent<EnemyBehaviour>().enabled = false;
        //    paralysisObj.GetComponent<NavMeshAgent>().isStopped = true;
        //}
    }

    public void Paralysis(GameObject enemy)
    {
        if(!paralysisObj.Contains(enemy))
        {
            paralysisObj.Add(enemy);
        }
        
    }
}
