using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchAreaTransparent : MonoBehaviour
{
    public List<GameObject> areaEnemy = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       
       
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "YellowEnemyBody" || other.tag == "BlueEnemyBody" || other.tag == "RedEnemyBody" || other.tag == "BossEnemyBody")
        {
            Debug.Log("hit");
            if (!areaEnemy.Contains(other.gameObject))
            {
                areaEnemy.Add(other.gameObject);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "YellowEnemyBody" || other.tag == "BlueEnemyBody" || other.tag == "RedEnemyBody" || other.tag == "BossEnemyBody")
        {
            if (areaEnemy.Contains(other.gameObject))
            {
                areaEnemy.Remove(other.gameObject);
            }
        }
    }
}
