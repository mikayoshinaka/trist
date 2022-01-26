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

    // 透明化用
    SphereCollider transparentCollider;

    // Cooldown
    GameObject cooldownBar;
    ColorActionCooldown colorActionCooldown;

    // UI
    GameObject gimmickUI;
    GameObject pressUI;
    GameObject waitUI;

    public override void EnterState(ColorAction colorAct)
    {
        //Debug.Log(this);

        // カメラ設定
        //GameObject currentCamera = GameObject.Find("Cameras").transform.Find("ZoomInCamera").gameObject;
        //currentCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_FollowOffset = new Vector3(15f, 15f, 0f);

        // 透明化用
        transparentCollider = GameObject.Find("SearchArea").GetComponent<SphereCollider>();

        colorActionObjects = colorAct.GetComponent<ColorActionObjects>();
        GameObject gimmickObject = colorAct.transform.Find("GimmickObjects").gameObject;
        gimmickObject.SetActive(true);

        // LineRenderer
        targetPoint = gimmickObject.transform.Find("Purple_LineRenderer").gameObject;
        trajectory = targetPoint.GetComponent<LineRenderer>();
        trajectory.gameObject.SetActive(true);

        instantiatedObjects = GameObject.Find("InstantiatedObjects");
        shooting = false;

        // Cooldown
        cooldownBar = GameObject.Find("Camera Canvas").transform.Find("GimmickCooldownBar").gameObject;
        colorActionCooldown = cooldownBar.GetComponent<ColorActionCooldown>();

        // UI
        gimmickUI = GameObject.Find("Camera Canvas").transform.Find("GimmickUI").gameObject;
        gimmickUI.SetActive(true);
        pressUI = gimmickUI.transform.Find("Press").gameObject;
        pressUI.SetActive(true);
        pressUI.GetComponent<UnityEngine.UI.Image>().sprite = gimmickUI.GetComponent<ColorActionUI>().purple;
        waitUI = gimmickUI.transform.Find("Wait").gameObject;
    }

    public override void UpdateState(ColorAction colorAct)
    {
        // UI
        if (!colorActionCooldown.cooldown && waitUI.activeInHierarchy)
        {
            waitUI.SetActive(false);
            pressUI.SetActive(true);
        }

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

        targetPoint.transform.RotateAround(colorAct.transform.position, Vector3.up, horizontal * 90 * Time.deltaTime);
        Vector3 direction = (targetPoint.transform.position - colorAct.transform.position).normalized;
        float distance = Vector3.Distance(colorAct.transform.position, targetPoint.transform.position);
        float saveDistance = distance;
        distance -= vertical * moveSpeed * Time.deltaTime;
        if (distance < 3 || distance > 24f)
        {
            distance = saveDistance;
        }
        targetPoint.transform.position = colorAct.transform.position + direction * distance;

        // 透明化用

        // REMAP
        // min + (input - inputmin) * (max - min) / (inputmax - inputmin)
        transparentCollider.radius = 3 + (distance - 9) * (8 - 3) / (24 - 9);
        transparentCollider.radius += 0.5f; // Offset
        if (transparentCollider.radius < 3)
        {
            transparentCollider.radius = 3f;
        }
        if (transparentCollider.radius > 10)
        {
            transparentCollider.radius = 10f;
        }
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
        if (!colorActionCooldown.cooldown && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton5)) && !shooting)
        {
            cooldownBar.SetActive(true);
            colorActionCooldown.StartCooldown(5f, ColorActionCooldown.ColorState.purple);

            //小野澤　サウンド用
            targetPoint.GetComponent<SGSEOneShot>().canSE = true;

            cannonball = MonoBehaviour.Instantiate(colorActionObjects.cannonball, origin, colorAct.transform.rotation, instantiatedObjects.transform);

            cannonballPoint = new Vector3[vertex];
            cannonballPoint = points.ToArray();

            if (FireCoroutine != null)
            {
                colorAct.StopCoroutine(FireCoroutine);
            }
            FireCoroutine = colorAct.StartCoroutine(FiringCannonball(colorAct));

            // UI
            pressUI.gameObject.SetActive(false);
            waitUI.gameObject.SetActive(true);
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
        //小野澤　サウンド用
        GameObject.Find("CannonBallSound").GetComponent<CannonBallHitSE>().HitBall();
        
        // 爆発エフェクト
        GameObject explosion = MonoBehaviour.Instantiate(colorActionObjects.cannonExplosion, cannonball.transform.position, cannonball.transform.rotation, instantiatedObjects.transform);
        MonoBehaviour.Destroy(explosion, 1f);

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
            if (enemy.tag == "NormalGhost")
            {
                enemy.GetComponent<EnemyBehaviour>().Gimmick_Run(2f);
            }
            else if (enemy.tag == "DonyoriGhost")
            {
                enemy.GetComponent<DonyoriBehaviour>().Gimmick_Run(2f);
            }

            // エフェクト
            GameObject effect = MonoBehaviour.Instantiate(colorActionObjects.colorHitEffect, enemy.transform.position, enemy.transform.rotation, enemy.transform);
            effect.GetComponent<UnityEngine.VFX.VisualEffect>().SetGradient("Gradient", colorActionCooldown.PickGradient(ColorActionCooldown.ColorState.purple));

            MonoBehaviour.Destroy(effect, 2f);
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
