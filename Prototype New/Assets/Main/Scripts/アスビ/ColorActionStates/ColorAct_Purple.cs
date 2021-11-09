using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAct_Purple : ColorActState
{
    GameObject targetPoint;
    LineRenderer trajectory;
    Vector3 origin, peak, target;
    List<Vector3> points = new List<Vector3>();
    int vertex = 20;
    float moveSpeed = 15f;
    float cannonSpeed = 10f;

    // Cannonball
    ColorActionObjects colorActionObjects;
    GameObject instantiatedObjects;
    GameObject cannonball;
    Vector3[] cannonballPoint;
    bool shooting;
    float colliderSize = 5f;

    public override void EnterState(ColorAction colorAct)
    {
        Debug.Log(this);

        // カメラ設定
        //GameObject currentCamera = GameObject.Find("Cameras").transform.Find("ZoomInCamera").gameObject;
        //currentCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_FollowOffset = new Vector3(15f, 15f, 0f);

        colorActionObjects = colorAct.GetComponent<ColorActionObjects>();
        GameObject gimmickObject = colorAct.transform.Find("GimmickObjects").gameObject;
        gimmickObject.SetActive(true);

        // LineRenderer
        targetPoint = gimmickObject.transform.Find("Purple_LineRenderer").gameObject;
        trajectory = targetPoint.GetComponent<LineRenderer>();
        trajectory.gameObject.SetActive(true);

        instantiatedObjects = GameObject.Find("InstantiatedObjects");
        shooting = false;
    }

    public override void UpdateState(ColorAction colorAct)
    {
        Gimmick_Purple(colorAct);
    }

    private void Gimmick_Purple(ColorAction colorAct)
    {
        // 狙う操作
        AimController(colorAct);

        // 狙った場所のビジュアル
        Trajectory(colorAct);

        // 弾を撃つ処理
        Shoot(colorAct);
    }

    // 操作
    private void AimController(ColorAction colorAct)
    {
        float horizontal = Input.GetAxisRaw("RHorizontal");
        float vertical = Input.GetAxisRaw("RVertical");
        Vector3 direction = new Vector3(vertical, 0f, horizontal).normalized;

        targetPoint.transform.position += direction * moveSpeed * Time.deltaTime;
    }

    // 操作のビジュアル
    private void Trajectory(ColorAction colorAct)
    {
        origin = colorAct.transform.position;
        target = targetPoint.transform.position;
        peak = (origin + target) / 2;
        float height = Vector3.Distance(origin, target);
        peak += new Vector3(0, height / 1.5f, 0);

        points.Clear();
        for (float ratio = 0; ratio <= 1; ratio += 1f / vertex)
        {
            var pointA = Vector3.Lerp(origin, peak, ratio);
            var pointB = Vector3.Lerp(peak, target, ratio);
            var bezierPoint = Vector3.Lerp(pointA, pointB, ratio);
            points.Add(bezierPoint);
        }
        trajectory.positionCount = points.Count;
        trajectory.SetPositions(points.ToArray());
    }

    // 攻撃処理
    private void Shoot(ColorAction colorAct)
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton5)) && !shooting)
        {
            cannonball = MonoBehaviour.Instantiate(colorActionObjects.cannonball, origin, colorAct.transform.rotation, instantiatedObjects.transform);

            cannonballPoint = new Vector3[vertex];
            cannonballPoint = points.ToArray();

            if (FireCoroutine != null)
            {
                colorAct.StopCoroutine(FireCoroutine);
            }
            FireCoroutine = colorAct.StartCoroutine(FiringCannonball(colorAct));
        }
    }
    Coroutine FireCoroutine;
    IEnumerator FiringCannonball(ColorAction colorAct)
    {
        shooting = true;
        int flag = 1;

        while (shooting && flag < cannonballPoint.Length)
        {
            cannonball.transform.position = Vector3.MoveTowards(cannonball.transform.position, cannonballPoint[flag], cannonSpeed * 1.5f * Time.deltaTime);
            float distance = Vector3.Distance(cannonball.transform.position, cannonballPoint[flag]);
            if (distance < 0.1f)
            {
                flag++;
            }

            yield return null;
        }

        // 敵に当たった時
        CannonCollider(colorAct);

        shooting = false;
        MonoBehaviour.Destroy(cannonball);
    }

    // コライダー
    private void CannonCollider(ColorAction colorAct)
    {
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(cannonball.transform.position, colliderSize, hitColliders, colorAct.enemyMask);
        for (int i = 0; i < numColliders; i++)
        {
            GameObject enemy = hitColliders[i].transform.parent.gameObject;
            enemy.GetComponent<EnemyBehaviour>().Gimmick_Purple();
        }
    }

    public override void DrawGizmosState(ColorAction colorAct)
    {
        if (shooting)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cannonballPoint[19], colliderSize);
        }
    }
}
