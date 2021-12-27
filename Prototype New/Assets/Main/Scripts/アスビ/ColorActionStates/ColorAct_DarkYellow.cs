using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ColorAct_DarkYellow : ColorActState
{
    NavMeshAgent agent;
    CharacterMovementScript characterMovementScript;
    GameObject dashBarrier;
    bool dashing;

    // Cooldown
    GameObject cooldownBar;
    ColorActionCooldown colorActionCooldown;

    // エフェクト
    ColorActionObjects colorActionObjects;

    public override void EnterState(ColorAction colorAct)
    {
        //Debug.Log(this);

        // カメラ設定
        //GameObject currentCamera = GameObject.Find("Cameras").transform.Find("ZoomInCamera").gameObject;
        //currentCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_FollowOffset = new Vector3(15f, 15f, 0f);

        agent = colorAct.GetComponent<NavMeshAgent>();
        characterMovementScript = colorAct.GetComponent<CharacterMovementScript>();

        GameObject gimmickObject = colorAct.transform.Find("GimmickObjects").gameObject;
        gimmickObject.SetActive(true);

        dashBarrier = gimmickObject.transform.Find("DarkYellow_Dash").gameObject;
        dashing = false;

        // Cooldown
        cooldownBar = GameObject.Find("Camera Canvas").transform.Find("GimmickCooldownBar").gameObject;
        colorActionCooldown = cooldownBar.GetComponent<ColorActionCooldown>();

        // エフェクト
        colorActionObjects = colorAct.GetComponent<ColorActionObjects>();
    }

    public override void UpdateState(ColorAction colorAct)
    {
        if (!colorActionCooldown.cooldown && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton5)) && !dashing)
        {
            Gimmick_DarkYellow(colorAct);

            cooldownBar.SetActive(true);
            colorActionCooldown.StartCooldown(3f, ColorActionCooldown.ColorState.darkyellow);
        }
    }

    private void Gimmick_DarkYellow(ColorAction colorAct)
    {
        if (Dash != null)
        {
            colorAct.StopCoroutine(Dash);
        }
        Dash = colorAct.StartCoroutine(Dashing(colorAct));
    }

    // コライダー
    private void DashCollider(ColorAction colorAct)
    {
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(colorAct.transform.position, 5f, hitColliders, colorAct.enemyMask);
        for (int i = 0; i < numColliders; i++)
        {
            GameObject enemy = hitColliders[i].transform.parent.gameObject;
            if (!enemy.GetComponent<EnemyBehaviour>().gimmickAction)
            {
                enemy.GetComponent<EnemyBehaviour>().Gimmick_DarkYellow();

                // エフェクト
                GameObject effect = MonoBehaviour.Instantiate(colorActionObjects.colorHitEffect, enemy.transform.position, enemy.transform.rotation, enemy.transform);
                effect.GetComponent<UnityEngine.VFX.VisualEffect>().SetGradient("Gradient", colorActionCooldown.PickGradient(ColorActionCooldown.ColorState.darkyellow));

                MonoBehaviour.Destroy(effect, 2f);
            }
        }
    }

    // ダッシュギミック処理
    Coroutine Dash;
    IEnumerator Dashing(ColorAction colorAct)
    {
        dashing = true;
        dashBarrier.SetActive(true);
        characterMovementScript.playerInterupt = true;

        float speed = 15f;
        float timer = 0f;
        float timeLimit = 0.5f;

        while (timer < timeLimit)
        {
            timer += Time.deltaTime;
            agent.Move(colorAct.transform.forward * speed / timeLimit * Time.deltaTime);
            DashCollider(colorAct);
            yield return null;
        }

        dashing = false;
        dashBarrier.SetActive(false);
        characterMovementScript.playerInterupt = false;
    }

    public override void DrawGizmosState(ColorAction colorAct)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(colorAct.transform.position, 5f);
    }
}
