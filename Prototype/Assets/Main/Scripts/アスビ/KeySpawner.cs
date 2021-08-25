using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    public GameObject key;
    public float spawnPointRadius = 16f;
    public int keyPicked = 0;
    public int maxKey = 4;

    public GameObject[] Doors;
    public GameObject[] spawnEnemies;

    private void Start()
    {
        keyPicked = 0;
        SpawnKey();
    }

    public void SpawnKey()
    {
        float pointX = Random.Range(-spawnPointRadius, spawnPointRadius);
        float pointZ = Random.Range(-spawnPointRadius, spawnPointRadius);

        Instantiate(key, new Vector3(transform.position.x + pointX, transform.position.y, transform.position.z + pointZ), transform.rotation, transform);
    }

    public void DoorSpawn()
    {
        if(keyPicked < Doors.Length - 1)
        {
            Doors[keyPicked].GetComponent<Renderer>().material.color = Color.red;
        }
        else if (keyPicked == Doors.Length - 1)
        {
            Doors[keyPicked].GetComponent<Renderer>().material.color = Color.green;
            Doors[keyPicked].GetComponent<BoxCollider>().enabled = true;
        }

        if (keyPicked < spawnEnemies.Length)
        {
            spawnEnemies[keyPicked].SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, spawnPointRadius);
    }
}
