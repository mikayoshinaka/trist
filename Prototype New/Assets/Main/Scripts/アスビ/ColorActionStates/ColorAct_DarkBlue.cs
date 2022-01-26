using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAct_DarkBlue : ColorActState
{
    GameObject barrier;
    float radius;
    bool barrierPlaying;

    // Cooldown
    GameObject cooldownBar;
    ColorActionCooldown colorActionCooldown;

    // エフェクト
    ColorActionObjects colorActionObjects;

    // UI
    GameObject gimmickUI;
    GameObject pressUI;
    GameObject waitUI;

    public override void EnterState(ColorAction colorAct)
    {
        //Debug.Log(this);

        radius = 1f;
        barrierPlaying = false;

        // カメラ設定
        //GameObject currentCamera = GameObject.Find("Cameras").transform.Find("ZoomInCamera").gameObject;
        //currentCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_FollowOffset = new Vector3(15f, 15f, 0f);

        // バリアー
        GameObject gimmickObject = colorAct.transform.Find("GimmickObjects").gameObject;
        gimmickObject.SetActive(true);
        barrier = gimmickObject.transform.Find("DarkBlue_Barrier").gameObject;

        // Cooldown
        cooldownBar = GameObject.Find("Camera Canvas").transform.Find("GimmickCooldownBar").gameObject;
        colorActionCooldown = cooldownBar.GetComponent<ColorActionCooldown>();

        // エフェクト
        colorActionObjects = colorAct.GetComponent<ColorActionObjects>();

        // UI
        gimmickUI = GameObject.Find("Camera Canvas").transform.Find("GimmickUI").gameObject;
        gimmickUI.SetActive(true);
        pressUI = gimmickUI.transform.Find("Press").gameObject;
        pressUI.SetActive(true);
        pressUI.GetComponent<UnityEngine.UI.Image>().sprite = gimmickUI.GetComponent<ColorActionUI>().darkBlue;
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

        if (!colorActionCooldown.cooldown && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton5)))
        {
            if (!barrierPlaying)
            {
                Gimmick_DarkBlue(colorAct);

                cooldownBar.SetActive(true);
                colorActionCooldown.StartCooldown(5f, ColorActionCooldown.ColorState.darkblue);

                // UI
                pressUI.gameObject.SetActive(false);
                waitUI.gameObject.SetActive(true);
            }
        }
    }

    private void Gimmick_DarkBlue(ColorAction colorAct)
    {
        // バリアーのオブジェクト処理
        if (Barrier != null)
        {
            colorAct.StopCoroutine(Barrier);
        }
        Barrier = colorAct.StartCoroutine(BarrierPlay(colorAct));
    }

    // バリアーの当たり判定
    private void BarrierCollider(ColorAction colorAct)
    {
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(colorAct.transform.position, radius / 2, hitColliders, colorAct.enemyMask);
        for (int i = 0; i < numColliders; i++)
        {
            GameObject enemy = hitColliders[i].transform.parent.gameObject;
            if ((enemy.tag == "NormalGhost" && !enemy.GetComponent<EnemyBehaviour>().gimmickAction) || (enemy.tag == "DonyoriGhost" && !enemy.GetComponent<DonyoriBehaviour>().gimmickAction))
            {
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
                effect.GetComponent<UnityEngine.VFX.VisualEffect>().SetGradient("Gradient", colorActionCooldown.PickGradient(ColorActionCooldown.ColorState.darkblue));

                MonoBehaviour.Destroy(effect, 2f);
            }
        }
    }

    // バリアーのオブジェクト拡大処理
    Coroutine Barrier;
    IEnumerator BarrierPlay(ColorAction colorAct)
    {
        barrierPlaying = true;
        barrier.SetActive(true);

        float timer = 0f;
        float timeLimit = 0.5f;
        while (timer < timeLimit)
        {
            timer += Time.deltaTime;
            radius += 19 / timeLimit * Time.deltaTime;
            barrier.transform.localScale = new Vector3(radius, radius, radius);
            
            // バリアーの当たり判定
            BarrierCollider(colorAct);
            
            yield return null;
        }

        radius = 1f;
        barrier.transform.localScale = new Vector3(radius, radius, radius);
        barrierPlaying = false;
        barrier.SetActive(false);
    }

    public override void DrawGizmosState(ColorAction colorAct)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(barrier.transform.position, radius / 2);
    }
}
