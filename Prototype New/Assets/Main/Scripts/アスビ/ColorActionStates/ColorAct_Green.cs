using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAct_Green : ColorActState
{
    GameObject possessZone;
    float radius;
    int maxPossess;
    bool possessing;

    List<GameObject> possessList = new List<GameObject>();
    List<GameObject> targetList = new List<GameObject>();

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

        radius = 5f;
        maxPossess = 5;
        possessing = false;
        possessList.Clear();
        targetList.Clear();

        GameObject gimmickObject = colorAct.transform.Find("GimmickObjects").gameObject;
        gimmickObject.SetActive(true);
        possessZone = gimmickObject.transform.Find("Green_Zone").gameObject;

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
        pressUI.GetComponent<UnityEngine.UI.Image>().sprite = gimmickUI.GetComponent<ColorActionUI>().green;
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
            Gimmick_Green(colorAct);

            cooldownBar.SetActive(true);
            colorActionCooldown.StartCooldown(5f, ColorActionCooldown.ColorState.green);

            // UI
            pressUI.gameObject.SetActive(false);
            waitUI.gameObject.SetActive(true);
        }

        if (possessing)
        {
            FollowPlayer(colorAct);
            Haunting(colorAct);
        }
    }

    private void Gimmick_Green(ColorAction colorAct)
    {
        possessing = true;

        if (ZoneEffect != null)
        {
            colorAct.StopCoroutine(ZoneEffect);
        }
        ZoneEffect = colorAct.StartCoroutine(ZoneActivate());

        int maxColliders = maxPossess;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(colorAct.transform.position, radius, hitColliders, colorAct.enemyMask);
        for (int i = 0; i < numColliders; i++)
        {
            GameObject enemy = hitColliders[i].transform.parent.gameObject;
            if (enemy.tag == "NormalGhost")
            {
                if (!enemy.GetComponent<EnemyBehaviour>().mazeGimmick && !enemy.GetComponent<EnemyBehaviour>().gimmickAction && !possessList.Contains(enemy) && !targetList.Contains(enemy))
                {
                    enemy.GetComponent<EnemyBehaviour>().Gimmick_Green(false, null);
                    possessList.Add(enemy);

                    // エフェクト
                    MonoBehaviour.Instantiate(colorActionObjects.possessAura, enemy.transform.position, enemy.transform.rotation, enemy.transform);
                }
            }
            else if (enemy.tag == "DonyoriGhost")
            {
                if (!enemy.GetComponent<DonyoriBehaviour>().mazeGimmick && !enemy.GetComponent<DonyoriBehaviour>().gimmickAction && !possessList.Contains(enemy) && !targetList.Contains(enemy))
                {
                    enemy.GetComponent<DonyoriBehaviour>().Gimmick_Green(false, null);
                    possessList.Add(enemy);

                    // エフェクト
                    MonoBehaviour.Instantiate(colorActionObjects.possessAura, enemy.transform.position, enemy.transform.rotation, enemy.transform);
                }
            }       
        }        
    }

    Coroutine ZoneEffect;
    IEnumerator ZoneActivate()
    {
        possessZone.SetActive(true);
        yield return new WaitForSeconds(1f);
        possessZone.SetActive(false);
    }

    private void FollowPlayer(ColorAction colorAct)
    {
        GameObject[] enemy = possessList.ToArray();
        for (int i = 0; i < enemy.Length; i++)
        {
            float distance = Vector3.Distance(enemy[i].transform.position, colorAct.transform.position);
            if (distance > 5f)
            {
                Vector3 offset = new Vector3(Random.Range(-2, 4), 0, Random.Range(-2, 4));
                if (enemy[i].tag == "NormalGhost")
                {
                    enemy[i].GetComponent<EnemyBehaviour>().agent.SetDestination(colorAct.transform.position + offset);
                }
                else if (enemy[i].tag == "DonyoriGhost")
                {
                    enemy[i].GetComponent<DonyoriBehaviour>().agent.SetDestination(colorAct.transform.position + offset);
                }
            } 
        }
    }

    private void Haunting(ColorAction colorAct)
    {
        int maxColliders = maxPossess + possessList.Count;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(colorAct.transform.position, radius * 2f, hitColliders, colorAct.enemyMask);

        for (int i = 0; i < numColliders; i++)
        {
            if (possessList.Count > 0)
            {
                GameObject enemy = hitColliders[i].transform.parent.gameObject;
                if (enemy.tag == "NormalGhost")
                {
                    if (!enemy.GetComponent<EnemyBehaviour>().gimmickAction && !targetList.Contains(enemy))
                    {
                        targetList.Add(enemy);
                        HauntTarget(colorAct, enemy);
                    }
                }
                else if (enemy.tag == "DonyoriGhost")
                {
                    if (!enemy.GetComponent<DonyoriBehaviour>().gimmickAction && !targetList.Contains(enemy))
                    {
                        targetList.Add(enemy);
                        HauntTarget(colorAct, enemy);
                    }
                }
            }
        }
    }

    private void HauntTarget(ColorAction colorAct, GameObject target)
    {
        GameObject[] haunter = possessList.ToArray();
        if (haunter[0].tag == "NormalGhost")
        {
            haunter[0].GetComponent<EnemyBehaviour>().Gimmick_Green(true, target);
        }
        else if (haunter[0].tag == "DonyoriGhost")
        {
            haunter[0].GetComponent<DonyoriBehaviour>().Gimmick_Green(true, target);
        }
        possessList.Remove(haunter[0]);
    }

    public void RemoveTarget(GameObject target)
    {
        targetList.Remove(target);
        
        // エフェクト
        GameObject effect = MonoBehaviour.Instantiate(colorActionObjects.colorHitEffect, target.transform.position, target.transform.rotation, target.transform);
        effect.GetComponent<UnityEngine.VFX.VisualEffect>().SetGradient("Gradient", colorActionCooldown.PickGradient(ColorActionCooldown.ColorState.green));
    }

    public override void DrawGizmosState(ColorAction colorAct)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(colorAct.transform.position, radius);
    }
}
