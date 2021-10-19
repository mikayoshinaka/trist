using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollLight : MonoBehaviour
{
    public Light lampLight;
    public GhostChange ghostChange;
    public EnemiesManager enemiesManager;

    // 当たり判定用
    List<string> enemyList = new List<string>();
    string[] enemyCheck;
    public bool enemyInSight;
    public float lightRange = 1f;
    LayerMask enemyMask;

    Mesh mesh;
    public bool showGizmos;

    private void Start()
    {
        lampLight = transform.Find("LampLight").GetComponent<Light>();
        lightRange = lampLight.range / 2;
        ghostChange = GameObject.Find("Ghost").GetComponent<GhostChange>();
        enemiesManager = GameObject.Find("Enemies").GetComponent<EnemiesManager>();

        enemyMask = LayerMask.GetMask("Enemy");
        
        //showGizmos = true;
    }

    private void Update()
    {
        // 当たり判定

        if (ghostChange.possess && ghostChange.possessObject == this.gameObject)
        {
            enemyInSight = Physics.CheckSphere(transform.position, lightRange, enemyMask);
            if (enemyInSight)
            {
                // 当たり判定処理
                DetectEnemy();
                //Debug.Log(enemyList.Count);
            }
        }
        else if (!ghostChange.possess)  // enemyList　Clear
        {
            if (enemyList.Count != 0)
            {
                string[] enemies = enemyList.ToArray();
                for (int i = 0; i < enemies.Length; i++)
                {
                    GameObject.Find(enemies[i]).GetComponent<EnemySearchScript>().StopFlash();
                }
                enemyList.Clear();
            }
        }

        // 敵を見つかった時
        if (enemyList.Count > 0)
        {
            enemyCheck = enemyList.ToArray();
            for (int i = 0; i < enemyCheck.Length; i++)
            {
                for (int j = 0; j < enemiesManager.enemies.Length; j++)
                {
                    if (enemyCheck[i] == enemiesManager.enemies[j].name)
                    {
                        if (!enemiesManager.enemies[j].GetComponent<EnemySearchScript>().flash)
                        {
                            enemiesManager.enemies[j].GetComponent<EnemySearchScript>().flash = true;
                        }
                    }
                }
            }
        }
    }

    // オブジェクトが判定内にある場合の処理
    void DetectEnemy()
    {
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, lightRange, hitColliders, enemyMask);

        for (int i = 0; i < numColliders; i++)
        {
            Vector3 lampDirection = Vector3.Scale(transform.forward, new Vector3(1, 0, 1));
            Vector3 targetDirection = Vector3.Scale((hitColliders[i].transform.position - transform.position), new Vector3(1, 0, 1)).normalized;

            float deltaAngle = Vector3.Angle(targetDirection, lampDirection);
            // Debug.Log(deltaAngle);

            string enemyName = hitColliders[i].transform.parent.name;

            if (deltaAngle > lampLight.spotAngle / 3)
            {
                // 判定以外
                if (enemyList.Contains(enemyName))
                {
                    Debug.Log("Remove [" + enemyName + "]");
                    GameObject.Find(enemyName).GetComponent<EnemySearchScript>().StopFlash();
                    enemyList.Remove(enemyName);
                }
                continue;
            }

            //Debug.Log(enemyName);

            if (enemyList.Contains(enemyName))
            {
                continue;
            }
            else
            {
                //Debug.Log(enemyName);

                // 判定以内
                Debug.Log("Add [" + enemyName + "]");
                enemyList.Add(enemyName);
            }
        }
    }

    // FOVのビジュアル
    Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        int numTriangles = 8;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -lampLight.spotAngle / 2, 0) * Vector3.forward * lightRange;
        Vector3 bottomRight = Quaternion.Euler(0, lampLight.spotAngle / 2, 0) * Vector3.forward * lightRange;

        Vector3 topCenter = bottomCenter + Vector3.up;
        Vector3 topLeft = bottomLeft + Vector3.up;
        Vector3 topRight = bottomRight + Vector3.up;

        int vert = 0;

        //左側
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //右側
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        //前側
        vertices[vert++] = bottomLeft;
        vertices[vert++] = bottomRight;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = topLeft;
        vertices[vert++] = bottomLeft;

        //上側
        vertices[vert++] = topCenter;
        vertices[vert++] = topLeft;
        vertices[vert++] = topRight;

        //下側
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomLeft;

        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public void OnValidate()
    {
        if (showGizmos)
        {
            mesh = CreateMesh();
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, lightRange);

            Gizmos.color = new Color(0, 0, 1, 0.1f);
            Quaternion lampRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            Gizmos.DrawMesh(mesh, transform.position, lampRotation);
        }
    }
}
