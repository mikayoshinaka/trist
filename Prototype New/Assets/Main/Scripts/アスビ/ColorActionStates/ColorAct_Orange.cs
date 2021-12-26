using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAct_Orange : ColorActState
{
    GameObject energyLine;
    LineRenderer lineRenderer;

    // チャージ用
    bool charging;
    float power = 2f;
    float angle = 30f;
    List<GameObject> enemyAnchor = new List<GameObject>();
   
    // EnergyZap
    ColorActionObjects colorActionObjects;
    GameObject instantiatedObjects;
    GameObject[] energyZap;
    GameObject[] energyZapLeap;
    int leapCount;
    int maxLeap;
    bool zapping;

    // 透明化用
    SphereCollider transparentCollider;

    // Cooldown
    GameObject cooldownBar;
    ColorActionCooldown colorActionCooldown;

    public override void EnterState(ColorAction colorAct)
    {
        Debug.Log(this);

        // カメラ設定
        //GameObject currentCamera = GameObject.Find("Cameras").transform.Find("ZoomInCamera").gameObject;
        //currentCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_FollowOffset = new Vector3(15f, 15f, 0f);

        // 透明化用
        transparentCollider = GameObject.Find("SearchArea").GetComponent<SphereCollider>();
        transparentCollider.radius = 5f;

        colorActionObjects = colorAct.GetComponent<ColorActionObjects>();
        GameObject gimmickObject = colorAct.transform.Find("GimmickObjects").gameObject;
        gimmickObject.SetActive(true);

        energyLine = gimmickObject.transform.Find("Orange_LineRenderer").gameObject;
        energyLine.SetActive(false);
        lineRenderer = energyLine.GetComponent<LineRenderer>();

        instantiatedObjects = GameObject.Find("InstantiatedObjects");

        power = 2f;
        leapCount = 0;
        maxLeap = 5;
        energyZapLeap = new GameObject[maxLeap];

        charging = false;
        zapping = false;

        // Cooldown
        cooldownBar = GameObject.Find("Camera Canvas").transform.Find("GimmickCooldownBar").gameObject;
        colorActionCooldown = cooldownBar.GetComponent<ColorActionCooldown>();
    }

    public override void UpdateState(ColorAction colorAct)
    {
        Gimmick_Orange(colorAct);
    }

    private void Gimmick_Orange(ColorAction colorAct)
    {
        if (!zapping)
        {
            // チャージ処理
            if (!colorActionCooldown.cooldown && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton5)))
            {
                energyLine.SetActive(true);
                charging = true;
            }
            if (charging)
            {
                ChargeEnergy(colorAct);
            }

            // 攻撃処理
            if (!colorActionCooldown.cooldown && (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton5)))
            {
                FireEnergy(colorAct);

                cooldownBar.SetActive(true);
                colorActionCooldown.StartCooldown(3f, ColorActionCooldown.ColorState.orange);
            }
        }
    }

    // チャージ処理
    private void ChargeEnergy(ColorAction colorAct)
    {
        float horizontal = Input.GetAxisRaw("RHorizontal");
        float rotate = horizontal * 180f * Time.deltaTime;
        colorAct.transform.Rotate(Vector3.up, rotate);

        if (power <= 10f)
        {
            power += 4f * Time.deltaTime;
        }

        Vector3 offset = new Vector3(0, 1f, 0);
        Vector3 origin = colorAct.transform.position + offset;
        Vector3 direction = origin + colorAct.transform.forward * power;

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, direction);
    }

    // 攻撃処理
    private void FireEnergy(ColorAction colorAct)
    {
        // 当たり判定
        EnergyAnchorSet(colorAct);

        if (enemyAnchor.Count > 0)
        {
            // 攻撃のビジュアル
            ExecuteEnergy(colorAct);
        }
        
        energyLine.SetActive(false);
        charging = false;
        power = 2f;
    }

    // 当たり判定
    private void EnergyAnchorSet(ColorAction colorAct)
    {
        enemyAnchor.Clear();

        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(colorAct.transform.position, power, hitColliders, colorAct.enemyMask);

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

            if (enemy.GetComponent<EnemyBehaviour>().gimmickAction)
            {
                continue;
            }

            enemyAnchor.Add(enemy);
            enemy.GetComponent<EnemyBehaviour>().Gimmick_Orange();
        }

        EnergyLeap(colorAct);
    }
    private void EnergyLeap(ColorAction colorAct)
    {
        leapCount = 0;

        GameObject[] enemyList = enemyAnchor.ToArray();
        for (int i = 0; i < enemyList.Length; i++)
        {
            int maxColliders = 10;
            Collider[] hitColliders = new Collider[maxColliders];
            int numColliders = Physics.OverlapSphereNonAlloc(enemyList[i].transform.position, power * 1.5f, hitColliders, colorAct.enemyMask);

            for (int j = 0; j < numColliders; j++)
            {
                GameObject enemy = hitColliders[j].transform.parent.gameObject;
                if (enemyAnchor.Contains(enemy))
                {
                    continue;
                }
                if (enemy.GetComponent<EnemyBehaviour>().gimmickAction)
                {
                    continue;
                }

                if (leapCount < 5)
                {
                    leapCount++;
                    energyZapLeap[i] = MonoBehaviour.Instantiate(colorActionObjects.energyZap, instantiatedObjects.transform);
                    LineRenderer zapLine = energyZapLeap[i].GetComponent<LineRenderer>();

                    Vector3 offset = new Vector3(0, 0.5f, 0);
                    Vector3 origin = enemyList[i].transform.position + offset;
                    Vector3 target = enemy.transform.position + offset;

                    zapLine.SetPosition(0, origin);
                    zapLine.SetPosition(1, target);

                    enemy.GetComponent<EnemyBehaviour>().Gimmick_Orange();
                }
            }
        }
    }

    // 攻撃のビジュアル
    private void ExecuteEnergy(ColorAction colorAct)
    {
        GameObject[] enemyList = enemyAnchor.ToArray();
        energyZap = new GameObject[enemyList.Length];

        for (int i = 0; i < enemyList.Length; i++)
        {
            energyZap[i] = MonoBehaviour.Instantiate(colorActionObjects.energyZap, instantiatedObjects.transform);
        }

        if (ZapCoroutine != null)
        {
            colorAct.StopCoroutine(ZapCoroutine);
        }
        ZapCoroutine = colorAct.StartCoroutine(EnergyZap(colorAct, enemyList));
    }
    Coroutine ZapCoroutine;
    IEnumerator EnergyZap(ColorAction colorAct, GameObject[] enemyList)
    {
        float timer = 0f;
        float timeLimit = 2f;

        zapping = true;

        while (timer < timeLimit)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < enemyList.Length; i++)
            {
                LineRenderer zapLine = energyZap[i].GetComponent<LineRenderer>();

                Vector3 offset = new Vector3(0, 1f, 0);
                Vector3 origin = colorAct.transform.position + offset;
                Vector3 target = enemyList[i].transform.position;
                target.y = colorAct.transform.position.y + offset.y;

                zapLine.SetPosition(0, origin);
                zapLine.SetPosition(1, target);
            }
            yield return null;
        }

        zapping = false;
        for (int i = 0; i < energyZap.Length; i++)
        {
            MonoBehaviour.Destroy(energyZap[i]);
        }

        for (int i = 0; i < maxLeap; i++)
        {
            MonoBehaviour.Destroy(energyZapLeap[i]);
        }

        if (instantiatedObjects.transform.childCount != 0)
        {
            for (int i = 0; i < instantiatedObjects.transform.childCount; i++)
            {
                MonoBehaviour.Destroy(instantiatedObjects.transform.GetChild(i).gameObject);
            }
        }
    }

    public override void DrawGizmosState(ColorAction colorAct)
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawRay(colorAct.transform.position, colorAct.transform.forward * power);
        Gizmos.DrawWireSphere(colorAct.transform.position, power);
    }
}
