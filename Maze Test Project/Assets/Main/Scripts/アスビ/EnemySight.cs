using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public float angle = 30f;
    public float chaseAngle = 1f;
    float height = 2f;
    public Color color = Color.cyan;

    [Header("SightRange以内")]
    public bool inView;
    public bool detected;
    
    [Header("Editor View")]
    public bool enableGizmos;
    Mesh mesh;

    private void Start()
    {
        //enableGizmos = true;
    }

    void Update()
    {
        // EnemyBehaviour 追いかける
        if (inView)
        {
            detected = DetectPlayer();
        }
    }

    // 敵の行動、索敵の当たり判定にプレイヤーの位置を確認する。True の場合、追走を行う
    bool DetectPlayer()
    {
        Vector3 direction = EnemyBehaviour.player.position - transform.position;
        direction.y = 0;

        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle * chaseAngle)
        {
            return false;
        }

        return true;
    }

    // FOV にビジュアル
    Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        int numTriangles = 8;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle * chaseAngle, 0) * Vector3.forward * EnemyBehaviour.sightRange;
        Vector3 bottomRight = Quaternion.Euler(0, angle * chaseAngle, 0) * Vector3.forward * EnemyBehaviour.sightRange;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

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

    private void OnValidate()
    {
        mesh = CreateMesh();
    }

    private void OnDrawGizmos()
    {
        if (mesh && enableGizmos)
        {
            Gizmos.color = color;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }
    }
}
