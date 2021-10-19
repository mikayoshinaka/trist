using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EnemiesManager : MonoBehaviour
{
    public GameObject[] enemies;
    public bool attacking;
    public float attackCooldown = 3f;
    public float worldStopTimer = 2f;
    bool attacked;
    float timerHalt;

    public GameObject playerController;
    public GameObject ghost;
    public GhostChange ghostChange;

    public float flashDuration = 3f;
    // Post Processing
    public PostProcessVolume postProcessVolume;
    LensDistortion lensDistortion;
    Coroutine Modify;

    private void Start()
    {
        attacking = false;
        attacked = false;
        playerController = GameObject.Find("PlayerController");
        ghost = playerController.transform.Find("Ghost").gameObject;
        ghostChange = ghost.GetComponent<GhostChange>();
        postProcessVolume = GameObject.Find("Post-Processing").GetComponent<PostProcessVolume>();
    }

    private void Update()
    {
        // とりつく時、驚かされた場合

        if (attacking && ghostChange.possess && !attacked)
        {
            GameObject.Find(ghostChange.possessObject.name).GetComponent<MonkeyDoll>().enabled = false;
            attacked = true;
            timerHalt = ghostChange.possessTime;
            //Debug.Log(timerHalt);
        }
        else if (attacked && !attacking && ghostChange.possess)
        {
            GameObject.Find(ghostChange.possessObject.name).GetComponent<MonkeyDoll>().enabled = true;
            attacked = false;
        }

        if (attacking && ghostChange.possess)
        {
            MonkeyDollTimerHalt();
        }
    }

    void MonkeyDollTimerHalt()
    {
        ghostChange.possessTime = timerHalt;
    }

    #region Post-Processing Effects

    public void ModifyPostProcess()
    {
        if (postProcessVolume.profile.TryGetSettings(out lensDistortion))
        {
            lensDistortion.intensity.value = 0;
            if (Modify != null)
            {
                StopCoroutine(Modify);
            }
            Modify = StartCoroutine(ModifyProgress());
        }
    }

    IEnumerator ModifyProgress()
    {
        float timer = 0f;
        float timeLimit = flashDuration;

        while (timer < timeLimit)
        {
            timer += Time.deltaTime;
            lensDistortion.intensity.value -= 100 / timeLimit * Time.deltaTime;
            yield return null;
        }

        ResetPostProcess();
    }

    
    public void ResetPostProcess()
    {
        if (postProcessVolume.profile.TryGetSettings(out lensDistortion))
        {
            if (Modify != null)
            {
                StopCoroutine(Modify);
            }
            lensDistortion.intensity.value = 0;
        }
    }

    #endregion
}
