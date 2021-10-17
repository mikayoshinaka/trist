using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollLight : MonoBehaviour
{
    public Light lampLight;

    // 当たり判定用
    List<string> enemyList = new List<string>();
    private bool clearEnemy;
    public bool enemyInSight;
    public float lightRange = 1f;
    LayerMask enemyMask;
    Mesh mesh;
    public bool showGizmos;

    private void Start()
    {
        lampLight = transform.Find("LampLight").GetComponent<Light>();
        lightRange = lampLight.range / 2;

        enemyMask = LayerMask.GetMask("Enemy");
        clearEnemy = false;

        showGizmos = true;
    }

    //private void Update()
    //{
    //    // 当たり判定

    //    enemyInSight = Physics.CheckSphere(transform.position, lightRange, enemyMask);
    //    if (enemyInSight)
    //    {
    //        // リストをクリアする
    //        ClearEnemy();

    //        // 当たり判定処理
    //        DetectEnemy();

    //        // DetectEnemyのリストにより、ギミックを行う
    //        TriggerEnemy();

    //        if (!clearEnemy)
    //        {
    //            clearEnemy = true;
    //        }
    //    }
    //    else if (clearEnemy)
    //    {
    //        // リストをクリアする
    //        ClearEnemy();
    //        clearEnemy = false;
    //    }
    //}

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

            if (deltaAngle > lampLight.spotAngle / 3)
            {
                continue;
            }

            string enemyName = hitColliders[i].transform.parent.name;
            //Debug.Log(enemyName);

            if (enemyList.Contains(enemyName))
            {
                continue;
            }
            else
            {
                //Debug.Log(enemyName);
                enemyList.Add(enemyName);
            }
        }
    }

    void ClearEnemy()
    {
        // todo 敵のスクリプトに統合

        string[] enemyName = enemyList.ToArray();

        for (int i = 0; i < enemyName.Length; i++)
        {
            GameObject.Find(enemyName[i]).transform.Find("EnemyBody").GetComponent<MeshRenderer>().enabled = false;
        }

        enemyList.Clear();
    }

    void TriggerEnemy()
    {
        // todo 敵のスクリプトに統合

        string[] enemyName = enemyList.ToArray();

        for (int i = 0; i < enemyName.Length; i++)
        {
            GameObject.Find(enemyName[i]).transform.Find("EnemyBody").GetComponent<MeshRenderer>().enabled = true;
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
