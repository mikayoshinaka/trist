using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class TutorialTextManager : MonoBehaviour
{
    // 仮スクリプト

    public GameObject textPanel;
    public GameObject textOutput;
    public Text[] textLists;

    [Header("チュートリアル用")]
    private bool intro;
    public GameObject player;
    public GameObject ghost;
    public GhostChange ghostChange;
    public GameObject[] enemy;
    public GameObject doll;

    private void Start()
    {
        textOutput.GetComponent<Text>().text = textLists[0].text;
        StartCoroutine(IntroStart());

        intro = false;
        player.GetComponent<CharacterMovementScript>().enabled = false;
        ghostChange = ghost.GetComponent<GhostChange>();

        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].GetComponent<NavMeshAgent>().speed = 0f;
        }
    }

    IEnumerator IntroStart()
    {
        yield return new WaitForSeconds(0.01f);
        ghost.GetComponent<GhostChange>().enabled = false;

        yield return new WaitForSeconds(3f);
        textOutput.GetComponent<Text>().text = textLists[1].text;
        ghost.GetComponent<GhostChange>().enabled = true;
        intro = true;
        tutorialOver = false;
    }

    private void Update()
    {
        // Intro
        if (intro)
        {
            if (ghostChange.possess)
            {
                ghostChange.possessTime = 0f;
            }

            if (Input.GetKeyDown(KeyCode.JoystickButton1) && !ghostChange.possess)
            {
                textOutput.GetComponent<Text>().text = textLists[2].text;
                StartCoroutine(LatePossess());
            }
            if (doll != null)
            {
                if (doll.GetComponent<MonkeyDoll>().enabled)
                {
                    doll.GetComponent<MonkeyDoll>().enabled = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.JoystickButton3) && ghostChange.possess)
            {
                StartCoroutine(IntroEnd());
            }
        }
    }

    IEnumerator LatePossess()
    {
        yield return new WaitForSeconds(0.5f);
        doll = ghostChange.possessObject;
    }

    IEnumerator IntroEnd()
    {
        yield return new WaitForSeconds(3f);
        intro = false;
        doll.GetComponent<MonkeyDoll>().enabled = true;
        player.GetComponent<CharacterMovementScript>().enabled = false;
        textOutput.GetComponent<Text>().text = textLists[3].text;

        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].GetComponent<NavMeshAgent>().speed = 3.5f;
        }

        StartCoroutine(TutorialOver());
    }

    IEnumerator TutorialOver()
    {
        yield return new WaitForSeconds(4f);
        if (!tutorialOver)
        {
            textOutput.GetComponent<Text>().text = textLists[4].text;
        }

        yield return new WaitForSeconds(4f);
        if (!tutorialOver)
        {
            textPanel.SetActive(false);
        }
    }

    private bool tutorialOver;
    // DoorViewに呼ぶ
    public void TutorialGoal()
    {
        tutorialOver = true;
        textPanel.SetActive(true);
        textOutput.GetComponent<Text>().text = textLists[5].text;
    }
}
