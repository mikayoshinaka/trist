using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAct_DarkRed : ColorActState
{
    List<GameObject> enemyList = new List<GameObject>();
    float distance = 20f;
    float angle = 30f;

    public override void EnterState(ColorAction colorAct)
    {
        Debug.Log(this);

        // カメラ設定
        GameObject currentCamera = GameObject.Find("Cameras").transform.Find("ZoomInCamera").gameObject;
        currentCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_FollowOffset = new Vector3(15f, 15f, 0f);

        // ライト
        GameObject gimmickObject = colorAct.transform.Find("GimmickObjects").gameObject;
        gimmickObject.SetActive(true);
        GameObject lightSource = gimmickObject.transform.Find("DarkRed_LightSource").gameObject;
        lightSource.SetActive(true);

        enemyList.Clear();
        OnValidate();
    }

    public override void UpdateState(ColorAction colorAct)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enemyList.Clear();
            Gimmick_DarkRed(colorAct);
        }
    }

    private void Gimmick_DarkRed(ColorAction colorAct)
    {
        // ライトの当たり判定
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(colorAct.transform.position, distance, hitColliders, colorAct.enemyMask);

        for (int i = 0; i < numColliders; i++)
        {
            Vector3 forwarDirection = Vector3.Scale(colorAct.transform.forward, new Vector3(1, 0, 1));
            Vector3 targetDirection = Vector3.Scale((hitColliders[i].transform.position - colorAct.transform.position), new Vector3(1, 0, 1)).normalized;

            float deltaAngle = Vector3.Angle(targetDirection, forwarDirection);

            GameObject enemy = hitColliders[i].transform.parent.gameObject;

            if (deltaAngle > angle)
            {
                continue;
            }

            if (enemyList.Contains(enemy))
            {
                continue;
            }
            else
            {
                // 判定以内
                //Debug.Log("Attack [" + enemy + "]");
                enemyList.Add(enemy);

                // 攻撃
                enemy.GetComponent<EnemyBehaviour>().Gimmick_DarkRed();
            }
        }
    }

    #region FOV

    Mesh mesh;

    // FOVのビジュアル
    Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        int numTriangles = 8;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle / 2, 0) * Vector3.forward * distance;

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
        mesh = CreateMesh();
    }

    #endregion

    public override void DrawGizmosState(ColorAction colorAct)
    {
        Gizmos.color = Color.red;
        Quaternion rotation = Quaternion.Euler(0, colorAct.transform.eulerAngles.y, 0);
        Gizmos.DrawMesh(mesh, colorAct.transform.position, rotation);
    }
}
