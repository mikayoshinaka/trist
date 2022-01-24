using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameStateManager : MonoBehaviour
{
    public enum Stage
    {
        stage1,
        stage2
    }
    [Header("Stage")]
    public Stage stage;

    [Header("Cameras")]
    [SerializeField] private GameObject cameras;
    [SerializeField] private GameObject zoomOutCamera;
    [SerializeField] private GameObject zoomMazeCamera;
    [SerializeField] private GameObject zoomInCamera;
    [SerializeField] private Vector3 zoomOutCameraOffset;
    [SerializeField] private Vector3 zoomInCameraOffset;

    [Header("Lighting")]
    [SerializeField] private GameObject lighting;
    [SerializeField] private GameObject lightSource_Collect;
    [SerializeField] private GameObject lightSource_Deliver;

    [Header("Player")]
    [SerializeField] private GameObject playerController;
    [SerializeField] private ColorAction colorAction;
    [SerializeField] private GhostCatch ghostCatch;
    [SerializeField] private SeeThrough seeThrough;
   
    [Header("Enemies")]
    [SerializeField] private EnemiesManager enemiesManager;
    [SerializeField] private DonyoriManager donyoriManager;

    [Header("Maze")]
    [SerializeField] private CollectBoxPost collectBoxPost;
    [SerializeField] private MazeAssignment mazeAssignment;
    [SerializeField] private float mazeTimer = 4f;
    [SerializeField] private GameTimer gameTimer;

    public enum GameState
    {
        gameState_Collect,
        gameState_Maze,
        gameState_Deliver
    }
    [Header("Game State")]
    public GameState gameState;

    private void Start()
    {
        cameras = GameObject.Find("Cameras");
        zoomOutCamera = cameras.transform.Find("ZoomOutCamera").gameObject;
        zoomMazeCamera = cameras.transform.Find("ZoomMazeCamera").gameObject;
        zoomInCamera = cameras.transform.Find("ZoomInCamera").gameObject;
        //zoomOutCameraOffset = new Vector3(15f, 15f, 0f);
        //zoomInCameraOffset = new Vector3(12.5f, 12.5f, 0f);

        lighting = GameObject.Find("Lighting");
        lightSource_Collect = lighting.transform.Find("LightSource_Collect").gameObject;
        lightSource_Deliver = lighting.transform.Find("LightSource_Deliver").gameObject;

        playerController = GameObject.Find("PlayerController");
        colorAction = playerController.GetComponent<ColorAction>();
        ghostCatch = playerController.transform.Find("PlayerBody").Find("CatchArea").GetComponent<GhostCatch>();
        seeThrough = playerController.GetComponent<SeeThrough>();
       
        enemiesManager = GameObject.Find("Enemies").GetComponent<EnemiesManager>();
        if (stage == Stage.stage2)
        {
            donyoriManager = GameObject.Find("DonyoriEnemies").GetComponent<DonyoriManager>();
        }

        collectBoxPost = GameObject.Find("Maze").transform.Find("SaveBox").GetComponent<CollectBoxPost>();
        mazeAssignment = GameObject.Find("Maze").GetComponent<MazeAssignment>();
        gameTimer = GameObject.Find("Timer").GetComponent<GameTimer>();

        StartState_Collecting();
    }

    #region GameState

    public void ChangeGameState(GameState newState)
    {
        if (newState == GameState.gameState_Collect)
        {
            StartState_Collecting();
        }
        else if (newState == GameState.gameState_Maze)
        {
            StartState_Maze();
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
        if (zoomInCamera.activeInHierarchy || zoomMazeCamera.activeInHierarchy)
        {
            zoomInCamera.SetActive(false);
            zoomMazeCamera.SetActive(false);
        }
        zoomOutCamera.SetActive(true);
        zoomOutCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = zoomOutCameraOffset;
        seeThrough.enabled = true;
        
        // ライティング
        if (lightSource_Deliver.activeInHierarchy)
        {
            lightSource_Deliver.SetActive(false);
        }
        lightSource_Collect.SetActive(true);

        //ghostCatch.ReSetCatch();

        if (colorAction.CheckCurrentGimmick() != ColorAction.ColorGimmick.gimmick_Null)
        {
            colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Null);
        }

        // 敵
        enemiesManager.RespawnEnemy();
        enemiesManager.SetMode(EnemiesManager.EnemyMode.Mode_Defensive);
        enemiesManager.ClearGimmick();

        if (stage == Stage.stage2)
        {
            donyoriManager.RespawnEnemy();
            donyoriManager.SetMode(DonyoriManager.EnemyMode.Mode_Defensive);
            donyoriManager.ClearGimmick();
        }

        // 迷路
        mazeAssignment.FurnitureActive();
        mazeAssignment.MazeNavmesh(false);
        gameTimer.timerPlay = true;
    }


    void StartState_Maze()
    {
        gameState = GameState.gameState_Maze;

        // カメラ
        if (zoomOutCamera.activeInHierarchy || zoomInCamera.activeInHierarchy)
        {
            zoomOutCamera.SetActive(false);
            zoomInCamera.SetActive(false);
        }
        zoomMazeCamera.SetActive(true);
        seeThrough.enabled = false;

        // 迷路
        collectBoxPost.SwitchBox();
        mazeAssignment.MazeAssign();

        if (MazeProgress != null)
        {
            StopCoroutine(MazeProgress);
        }
        MazeProgress = StartCoroutine(StartMaze());
    }
    Coroutine MazeProgress;
    IEnumerator StartMaze()
    {
        playerController.GetComponent<CharacterMovementScript>().enabled = false;
        collectBoxPost.HideBox(true);
        gameTimer.timerPlay = false;

        yield return new WaitForSeconds(mazeTimer);
        
        playerController.GetComponent<CharacterMovementScript>().enabled = true;
        collectBoxPost.HideBox(false);
        gameTimer.timerPlay = true;

        mazeAssignment.MazeNavmesh(true);
        ChangeGameState(GameState.gameState_Deliver);
    }


    void StartState_Delivering()
    {
        gameState = GameState.gameState_Deliver;

        // カメラ
        if (zoomOutCamera.activeInHierarchy || zoomMazeCamera.activeInHierarchy)
        {
            zoomOutCamera.SetActive(false);
            zoomMazeCamera.SetActive(false);
        }
        zoomInCamera.SetActive(true);
        zoomInCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = zoomInCameraOffset;
        seeThrough.enabled = true;
        
        // ライティング
        if (lightSource_Collect.activeInHierarchy)
        {
            lightSource_Collect.SetActive(false);
        }
        lightSource_Deliver.SetActive(true);

        // 敵
        enemiesManager.SetMode(EnemiesManager.EnemyMode.Mode_Offensive);

        if (stage == Stage.stage2)
        {
            donyoriManager.SetMode(DonyoriManager.EnemyMode.Mode_Offensive);
        }
    }

    #endregion
}
