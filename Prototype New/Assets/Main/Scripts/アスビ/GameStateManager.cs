using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameStateManager : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject cameras;
    public GameObject zoomOutCamera;    
    public GameObject zoomInCamera;
    private Vector3 zoomOutCameraOffset;
    private Vector3 zoomInCameraOffset;

    [Header("Lighting")]
    public GameObject lighting;
    public GameObject lightSource_Collect;
    public GameObject lightSource_Deliver;

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
        zoomOutCameraOffset = new Vector3(15f, 15f, 0f);
        zoomInCameraOffset = new Vector3(12f, 12f, 0f);

        lighting = GameObject.Find("Lighting");
        lightSource_Collect = lighting.transform.Find("LightSource_Collect").gameObject;
        lightSource_Deliver = lighting.transform.Find("LightSource_Deliver").gameObject;

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
        zoomOutCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = zoomOutCameraOffset;

        // ライティング
        if (lightSource_Deliver.activeInHierarchy)
        {
            lightSource_Deliver.SetActive(false);
        }
        lightSource_Collect.SetActive(true);

        ghostCatch.ReSetCatch();

        if (colorAction.CheckCurrentGimmick() != ColorAction.ColorGimmick.gimmick_Null)
        {
            colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Null);
        }

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
        zoomInCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = zoomInCameraOffset;

        // ライティング
        if (lightSource_Collect.activeInHierarchy)
        {
            lightSource_Collect.SetActive(false);
        }
        lightSource_Deliver.SetActive(true);

        enemiesManager.enemyMode = EnemiesManager.EnemyMode.Mode_Offensive;
    }

    #endregion
}
