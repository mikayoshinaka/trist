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

    // Cooldown
    GameObject cooldownBar;
    ColorActionCooldown colorActionCooldown;

    public override void EnterState(ColorAction colorAct)
    {
        Debug.Log(this);

        radius = 5f;
        maxPossess = 5;
        possessing = false;

        GameObject gimmickObject = colorAct.transform.Find("GimmickObjects").gameObject;
        gimmickObject.SetActive(true);
        possessZone = gimmickObject.transform.Find("Green_Zone").gameObject;

        // Cooldown
        cooldownBar = GameObject.Find("Camera Canvas").transform.Find("GimmickCooldownBar").gameObject;
        colorActionCooldown = cooldownBar.GetComponent<ColorActionCooldown>();
    }

    public override void UpdateState(ColorAction colorAct)
    {
        if (!colorActionCooldown.cooldown && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton5)))
        {
            Gimmick_Green(colorAct);

            cooldownBar.SetActive(true);
            colorActionCooldown.StartCooldown(3f, ColorActionCooldown.ColorState.green);
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
        possessList.Clear();

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
            if (!enemy.GetComponent<EnemyBehaviour>().mazeGimmick && !enemy.GetComponent<EnemyBehaviour>().gimmickAction)
            {
                enemy.GetComponent<EnemyBehaviour>().Gimmick_Green(false, null);
                possessList.Add(enemy);
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
                enemy[i].GetComponent<EnemyBehaviour>().agent.SetDestination(colorAct.transform.position + offset);
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
            GameObject enemy = hitColliders[i].transform.parent.gameObject;
            if (!enemy.GetComponent<EnemyBehaviour>().gimmickAction)
            {
                HauntTarget(colorAct, enemy);
            }
        }
    }

    private void HauntTarget(ColorAction colorAct, GameObject target)
    {
        if (possessList.Count > 0)
        {
            GameObject[] haunter = possessList.ToArray();
            haunter[0].GetComponent<EnemyBehaviour>().Gimmick_Green(true, target);
            possessList.Remove(haunter[0]);
        }
    }

    public override void DrawGizmosState(ColorAction colorAct)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(colorAct.transform.position, radius);
    }
}
