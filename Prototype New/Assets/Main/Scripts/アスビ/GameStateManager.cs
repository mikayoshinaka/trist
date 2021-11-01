using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject cameras;
    public GameObject zoomOutCamera;
    public GameObject zoomInCamera;

    [Header("Lighting")]
    public GameObject lighting;
    public GameObject lightSource_1;
    public GameObject lightSource_2;

    [Header("Player")]
    public GameObject playerController;
    public ColorAction colorAction;
    public GhostCatch ghostCatch;

    [Header("Enemies")]
    public EnemiesManager enemiesManager;

    public enum GameState
    {
        gameState_Collect,
        gameState_Deliver
    }
    [Header("Game State")]
    public GameState gameState;

    private void Start()
    {
        cameras = GameObject.Find("Cameras");
        zoomOutCamera = cameras.transform.Find("ZoomOutCamera").gameObject;
        zoomInCamera = cameras.transform.Find("ZoomInCamera").gameObject;

        lighting = GameObject.Find("Lighting");
        lightSource_1 = lighting.transform.Find("LightSource_1").gameObject;
        lightSource_2 = lighting.transform.Find("LightSource_2").gameObject;

        playerController = GameObject.Find("PlayerController");
        colorAction = playerController.GetComponent<ColorAction>();
        ghostCatch = playerController.transform.Find("PlayerBody").Find("CatchArea").GetComponent<GhostCatch>();

        enemiesManager = GameObject.Find("Enemies").GetComponent<EnemiesManager>();

        StartState_Collecting();
    }

    #region GameState

    public void ChangeGameState(GameState newState)
    {
        if (newState == GameState.gameState_Collect)
        {
            StartState_Collecting();
        }
        else if (newState == GameState.gameState_Deliver)
        {
            StartState_Delivering();
        }
    }

    void StartState_Collecting()
    {
        gameState = GameState.gameState_Collect;

        // カメラ
        if (zoomInCamera.activeInHierarchy)
        {
            zoomInCamera.SetActive(false);
        }
        zoomOutCamera.SetActive(true);

        // ライティング
        if (lightSource_2.activeInHierarchy)
        {
            lightSource_2.SetActive(false);
        }
        lightSource_1.SetActive(true);

        ghostCatch.ReSetCatch();
        colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Null);

        enemiesManager.enemyMode = EnemiesManager.EnemyMode.Mode_Defensive;
    }

    void StartState_Delivering()
    {
        gameState = GameState.gameState_Deliver;

        // カメラ
        if (zoomOutCamera.activeInHierarchy)
        {
            zoomOutCamera.SetActive(false);
        }
        zoomInCamera.SetActive(true);

        // ライティング
        if (lightSource_1.activeInHierarchy)
        {
            lightSource_1.SetActive(false);
        }
        lightSource_2.SetActive(true);

        enemiesManager.enemyMode = EnemiesManager.EnemyMode.Mode_Offensive;
    }

    #endregion
}
