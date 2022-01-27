using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class GhostCatch : MonoBehaviour
{
    public List<GameObject> enemy = new List<GameObject>();
    public List<GameObject> bossEnemy = new List<GameObject>();
    public List<GameObject> caughtObj = new List<GameObject>();
    public List<Vector3> caughtObjPos = new List<Vector3>();
    [SerializeField] private DollSave dollSave;

    [SerializeField] private GameObject redDoll;
    [SerializeField] private GameObject blueDoll;
    [SerializeField] private GameObject yellowDoll;
    [SerializeField] private GameObject darkRedDoll;
    [SerializeField] private GameObject darkBlueDoll;
    [SerializeField] private GameObject darkYellowDoll;
    [SerializeField] private GameObject purpleDoll;
    [SerializeField] private GameObject greenDoll;
    [SerializeField] private GameObject orangeDoll;
    [SerializeField] private GameObject redDoll2;
    [SerializeField] private GameObject blueDoll2;
    [SerializeField] private GameObject yellowDoll2;
    [SerializeField] private GameObject darkRedDoll2;
    [SerializeField] private GameObject darkBlueDoll2;
    [SerializeField] private GameObject darkYellowDoll2;
    [SerializeField] private GameObject purpleDoll2;
    [SerializeField] private GameObject greenDoll2;
    [SerializeField] private GameObject orangeDoll2;
    [SerializeField] private GameObject bossDoll;
    [SerializeField] private Material ghostRedMaterial;
    [SerializeField] private Material ghostBlueMaterial;
    [SerializeField] private Material ghostYellowMaterial;
    [SerializeField] private Transform dollInstancePos;
    [SerializeField] private Transform dollInstancePos2;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform inhalePos;
    [SerializeField] Possess possessScript;
    public bool grab;
    public bool bossGrab;
    bool zoom;
    bool disclose;
    bool directionToggle;
    bool vibrate;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerController;
    [SerializeField] public GameObject presentBox;
    [SerializeField] private GameObject image;
    [SerializeField] private GameObject image2;
    [SerializeField] private GameObject lampMax;
    [SerializeField] private GameObject lamp3;
    [SerializeField] private GameObject lamp2;
    [SerializeField] private GameObject lamp1;
    [SerializeField] private GameObject lamp0;
    [SerializeField] private Transparent transparent;
    public GameObject doll = null;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject dollInstanceZoom1;
    [SerializeField] private GameObject dollInstanceZoom2;
    [SerializeField] private GameObject shootCamera;
    [SerializeField] private float dollRotateSpeed = 20.0f;
    [SerializeField] private float discloseHeight = 1.0f;
    [SerializeField] private float discloseSpeed = 1.0f;
    [SerializeField] private float inhaleSpeedMax = 5.0f;
    [SerializeField] private float inhaleAccelerate = 1.2f;
    [SerializeField] private float inhaleFirstSpeed = 1.0f;
    [SerializeField] private float inhaleTime = 1.0f;
    [SerializeField] private float vibrateTime = 1.0f;
    private float inhaleSpeed;
    private float notCatchTime;
    private float canGrabTime;
    private float maxGrabTime;
    private float dollInstanceTime;
    private float shootTime;
    [SerializeField] private float dollZoom2Time = 2.0f;
    [SerializeField] private float dollInstanceMaxTime = 10.0f;
    private float discloseTime;
    GameObject ear;
    [SerializeField] private float vibrateRange = 0.5f, vibrateSpeed = 10.0f;
    float initPosition, newPosition, minPosition, maxPosition;
    //[SerializeField] private int partition = 20;
    private List<float> time = new List<float>();

    AudioSource audioSource;
    public AudioClip grabSE;
    public AudioClip slowSE;
    public AudioClip showoffSE;
    public AudioClip attackedSE;
    public AudioClip dollVibrateSE;
    bool attacked;
    public enum Mode
    {
        CanGrab,
        Fusion,
        Instance,
        Carry,
        Shoot,
        CannotGrab,
        Attacked,
        End
    }
    public Mode mode;

    // アスビ用
    private GameStateManager gameStateManager;
    private ColorAction colorAction;

    void Start()
    {
        bossGrab = false;
        grab = false;
        zoom = false;
        disclose = false;
        directionToggle = false;
        vibrate = false;
        notCatchTime = 0.0f;
        canGrabTime = 3.0f;
        maxGrabTime = canGrabTime;
        dollInstanceTime = 0.0f;
        shootTime = 0.0f;
        discloseTime = 0.0f;
        lampMax.SetActive(false);
        lamp3.SetActive(false);
        lamp2.SetActive(false);
        lamp1.SetActive(false);
        lamp0.SetActive(false);
        audioSource = GameObject.Find("PlayerSound").GetComponent<AudioSource>();
        attacked = false;
        // アスビ用
        gameStateManager = GameObject.Find("GameState").GetComponent<GameStateManager>();
        colorAction = GameObject.Find("PlayerController").GetComponent<ColorAction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("BeforeBegin").GetComponent<BeforeBegin>().begin == true)
        {
            return;
        }
        switch (mode)
        {
            case Mode.CanGrab:
                GhostGrab();
                break;
            case Mode.Fusion:
                GhostFusion();
                break;
            case Mode.Instance:
                DollInstance();
                break;
            case Mode.Carry:
                DollCarry();
                break;
            case Mode.Shoot:
                DollShoot();
                break;
            case Mode.CannotGrab:
                CannotCatch();
                break;
            case Mode.Attacked:
                EnemyAttacked();
                break;
            case Mode.End:
                break;
        }
        if (mode != Mode.CanGrab || mode != Mode.Attacked)
        {
            CaughtObjStop();
        }
    }
    //  敵を掴む
    private void GhostGrab()
    {
        //ボス
        if ((Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton1)) && bossEnemy.Count > 0 && possessScript.possess == false && grab == false && bossGrab == false)
        {
            Sort(0, bossEnemy.Count - 1, ref bossEnemy);
            if (bossEnemy[0].tag == "BossEnemy" && bossEnemy[0].GetComponent<BossEnemy>().mode == BossEnemy.Mode.grabbed)
            {
                return;
            }
            else if ((bossEnemy[0].tag == "BossEarLeft" || bossEnemy[0].tag == "BossEarRight") && bossEnemy[0].transform.parent.parent.gameObject.GetComponent<BossEnemy>().mode == BossEnemy.Mode.grabbed)
            {
                return;
            }

            if (bossGrab == false)
            {
                bossGrab = true;
            }

            if (bossEnemy[0].tag == "BossEarLeft" || bossEnemy[0].tag == "BossEarRight")
            {
                bossEnemy[0].transform.parent.parent.gameObject.GetComponent<BossEar>().Selectear(bossEnemy[0]);
                audioSource.PlayOneShot(grabSE);
                ear = bossEnemy[0];
            }


            if (bossEnemy[0].tag == "BossEnemy" && !caughtObj.Contains(bossEnemy[0]))
            {
                if (bossEnemy[0].GetComponent<BossEnemy>().bossHP == 0)
                {
                    bossEnemy[0].transform.parent = playerController.transform;
                }
                audioSource.PlayOneShot(grabSE);
                caughtObj.Add(bossEnemy[0]);
            }
            else if ((bossEnemy[0].tag == "BossEarLeft" || bossEnemy[0].tag == "BossEarRight") && !caughtObj.Contains(bossEnemy[0].transform.parent.parent.gameObject))
            {

                caughtObj.Add(bossEnemy[0].transform.parent.parent.gameObject);
            }

            if (canGrabTime / maxGrabTime > 0)
            {
                GrabbingTime();
            }
        }
        else if ((Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton1)) && bossGrab == true && possessScript.possess == false && grab == false)
        {

            GrabbingTime();
        }
        else if (bossGrab == true && mode == Mode.CanGrab)
        {
            if (ear != null && ear.transform.parent.parent.gameObject.GetComponent<BossEar>().CanDetachEar() == false)
            {
                ear.transform.parent.parent.gameObject.GetComponent<BossEar>().UndoEar(ear);
                ear = null;
                CaughtObjMoveable(caughtObj[0]);
                bossGrab = false;
                caughtObj.Clear();
                lampMax.SetActive(false);
                lamp3.SetActive(false);
                lamp2.SetActive(false);
                lamp1.SetActive(false);
                lamp0.SetActive(false);
                canGrabTime = 3.0f;
                mode = Mode.CannotGrab;
            }
            else if (caughtObj[0].GetComponent<BossEnemy>().bossHP > 0)
            {
                if (ear != null)
                {
                    ear.transform.parent.parent.gameObject.GetComponent<BossEar>().UndoEar(ear);
                    ear.transform.parent.parent.gameObject.GetComponent<BossEar>().DetachEar();
                    ear = null;
                }
                caughtObj[0].GetComponent<BossEnemy>().bossHP -= 1;
                caughtObj[0].GetComponent<BossEnemy>().hpDown = true;
                CaughtObjMoveable(caughtObj[0]);
                bossGrab = false;
                caughtObj.Clear();
                lampMax.SetActive(false);
                lamp3.SetActive(false);
                lamp2.SetActive(false);
                lamp1.SetActive(false);
                lamp0.SetActive(false);
                canGrabTime = 3.0f;
                mode = Mode.CanGrab;
            }
            else
            {
                bossGrab = false;
                Vector3 startPos = transform.InverseTransformPoint(caughtObj[0].transform.GetChild(0).position);
                caughtObjPos.Add(startPos);
                time.Add(0.0f);

                lampMax.SetActive(false);
                lamp3.SetActive(false);
                lamp2.SetActive(false);
                lamp1.SetActive(false);
                lamp0.SetActive(false);
                audioSource.PlayOneShot(slowSE);
                mode = Mode.Fusion;
            }

        }
        //通常敵
        if ((Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton1)) && enemy.Count > 0 && possessScript.possess == false && bossGrab == false)
        {
            if (enemy.Count > 1)
            {
                Sort(0, enemy.Count - 1, ref enemy);
            }
            if (grab == false)
            {
                grab = true;
            }

            for (int i = 0; i < enemy.Count; i++)
            {
                if (!caughtObj.Contains(enemy[i]))
                {
                    if (enemy[i].tag == "NormalGhost")
                    {
                        enemy[i].GetComponent<EnemyBehaviour>().enabled = false;
                    }
                    else if (enemy[i].tag == "DonyoriGhost")
                    {
                        enemy[i].GetComponent<DonyoriBehaviour>().enabled = false;
                    }

                    enemy[i].transform.parent = playerController.transform;
                    caughtObj.Add(enemy[i]);
                    audioSource.PlayOneShot(grabSE);
                }
            }
            if (canGrabTime / maxGrabTime > 0)
            {
                GrabbingTime();
            }
        }
        else if ((Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton1)) && grab == true && possessScript.possess == false && bossGrab == false)
        {
            if (enemy.Count > 1)
            {
                Sort(0, enemy.Count - 1, ref enemy);
            }
            for (int i = 0; i < enemy.Count; i++)
            {
                if (!caughtObj.Contains(enemy[i]))
                {
                    if (enemy[i].tag == "NormalGhost")
                    {
                        enemy[i].GetComponent<EnemyBehaviour>().enabled = false;
                    }
                    else if (enemy[i].tag == "DonyoriGhost")
                    {
                        enemy[i].GetComponent<DonyoriBehaviour>().enabled = false;
                    }

                    enemy[i].transform.parent = playerController.transform;
                    caughtObj.Add(enemy[i]);
                    audioSource.PlayOneShot(grabSE);
                }
            }
            GrabbingTime();
        }
        else if (grab == true && mode == Mode.CanGrab)
        {
            grab = false;
            for (int i = 0; i < caughtObj.Count; i++)
            {
                //caughtObj[i].transform.parent = null;
                Vector3 startPos = transform.InverseTransformPoint(caughtObj[i].transform.GetChild(0).position);
                caughtObjPos.Add(startPos);
                time.Add(0.0f);
            }

            lampMax.SetActive(false);
            lamp3.SetActive(false);
            lamp2.SetActive(false);
            lamp1.SetActive(false);
            lamp0.SetActive(false);
            audioSource.PlayOneShot(slowSE);
            mode = Mode.Fusion;
        }
    }
    //  敵を箱に入れる
    private void GhostFusion()
    {
        for (int i = 0; i < caughtObj.Count; i++)
        {
            if (caughtObj[i].activeSelf == true)
            {
                Vector3 p1Pos = Vector3.zero;
                Vector3 p2Pos = Vector3.zero;
                BezierCoordinate(transform.TransformPoint(caughtObjPos[i]), ref p1Pos, ref p2Pos, presentBox.transform.position);
                SuckedIntoBox(transform.TransformPoint(caughtObjPos[i]), p1Pos, p2Pos, presentBox.transform.position, i);
            }
        }
        bool fusionComplete = false;
        for (int i = 0; i < time.Count; i++)
        {
            if (time[i] > 1.0f)
            {
                if (caughtObj[i].activeSelf == true)
                {
                    caughtObj[i].SetActive(false);
                }
                fusionComplete = true;
            }
            else
            {
                fusionComplete = false;
                break;
            }
        }
        if (fusionComplete == true)
        {
            mode = Mode.Instance;
        }

    }
    // 人形を作る
    private void DollInstance()
    {
        if (dollInstanceTime == 0.0f)
        {
            dollInstanceZoom1.SetActive(true);
            playerController.GetComponent<CharacterMovementScript>().enabled = false;
            playerController.transform.LookAt(new Vector3(mainCamera.transform.position.x, playerController.transform.position.y, mainCamera.transform.position.z));

            // アスビ用
            playerController.GetComponent<SeeThrough>().enabled = false;
        }
        else if (dollInstanceTime >= dollZoom2Time && zoom == false)
        {
            dollInstanceZoom2.SetActive(true);
            DollCombination(caughtObj);
            zoom = true;
        }
        else if (dollInstanceTime >= dollZoom2Time && zoom == true)
        {
            DollDisclose(caughtObj);
            doll.transform.Rotate(new Vector3(0, dollRotateSpeed * Time.deltaTime, 0));
        }
        dollInstanceTime += Time.deltaTime;
        if (dollInstanceTime < dollInstanceMaxTime)
        {
            return;
        }
        doll.transform.rotation = new Quaternion(0, 0, 0, 0);
        doll.transform.position = dollInstancePos2.position;
        playerController.GetComponent<CharacterMovementScript>().enabled = true;
        dollInstanceZoom1.SetActive(false);
        dollInstanceZoom2.SetActive(false);
        dollInstanceTime = 0.0f;
        discloseTime = 0.0f;
        zoom = false;
        disclose = false;
        mode = Mode.Carry;

        // アスビ用
        gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Maze);
    }

    // 人形を運ぶ
    private void DollCarry()
    {
        if (doll == null && !(Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton1)))
        {
            mode = Mode.CanGrab;

            // アスビ用
            gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Collect);
        }
    }
    //人形を箱に入れる
    private void DollShoot()
    {
        if (shootTime == 0.0f)
        {
            playerController.transform.position = new Vector3(shootPos.position.x, playerController.transform.position.y, shootPos.position.z);
            playerController.transform.LookAt(inhalePos);
            playerController.GetComponent<CharacterMovementScript>().enabled = false;
            mainCamera.SetActive(false);
            shootCamera.SetActive(true);
            dollSave.AnimStart();
        }
        shootTime += Time.deltaTime;
        if (shootTime > 0.0f && shootTime < vibrateTime)
        {
            DollVibrate();
        }
        else if (shootTime >= vibrateTime && shootTime < vibrateTime + inhaleTime)
        {
            vibrate = false;
            directionToggle = false;
            DollInhale();
        }
        else
        {
            if (dollSave.bossIn == false)
            {
                playerController.GetComponent<CharacterMovementScript>().enabled = true;
                shootTime = 0.0f;
                mainCamera.SetActive(true);
                shootCamera.SetActive(false);
                ReSetCatch();
                mode = Mode.CanGrab;

                // アスビ用
                gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Collect);
            }
            else
            {
                mode = Mode.End;
            }

        }

    }
    // 捕まえることができない
    private void CannotCatch()
    {
        notCatchTime += Time.deltaTime;
        if (notCatchTime > 2.0f && !(Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton1)))
        {
            ReSetCatch();
            notCatchTime = 0.0f;
            grab = false;
            mode = Mode.CanGrab;

            // アスビ用
            gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Collect);
        }
    }
    //攻撃された
    private void EnemyAttacked()
    {
        if (attacked == false)
        {
            audioSource.PlayOneShot(attackedSE);
            attacked = true;
        }
        if (doll != null)
        {
            doll.SetActive(false);
            doll = null;
        }
        for (int i = 0; i < caughtObj.Count; i++)
        {
            if (caughtObj[i].activeSelf == false)
            {
                caughtObj[i].SetActive(true);
                time[i] = 0.0f;

            }
            if (caughtObj[i].activeSelf == true)
            {
                Vector3 p1Pos = Vector3.zero;
                Vector3 p2Pos = Vector3.zero;
                BezierCoordinate(presentBox.transform.position, ref p1Pos, ref p2Pos, transform.TransformPoint(caughtObjPos[i]));
                SuckedIntoBox(presentBox.transform.position, p1Pos, p2Pos, transform.TransformPoint(caughtObjPos[i]), i);
            }
        }
        bool enemyFleeComplete = false;
        for (int i = 0; i < time.Count; i++)
        {
            if (time[i] > 1.0f)
            {
                enemyFleeComplete = true;
                CaughtObjMoveable(caughtObj[i]);
            }
            else
            {
                enemyFleeComplete = false;
                break;
            }
        }
        if (enemyFleeComplete == true)
        {
            ReSetCatch();
            attacked = false;
            mode = Mode.CannotGrab;
        }
    }
    //並べ替え
    private void Sort(int left, int right, ref List<GameObject> searchObject)
    {

        for (int i = searchObject.Count; i > 0; i--)
        {
            for (int j = 1; j < i; j++)
            {
                float value1 = (searchObject[j - 1].transform.position - player.transform.position).magnitude;
                float value2 = (searchObject[j].transform.position - player.transform.position).magnitude;

                if (value1 > value2)
                {
                    GameObject temp = searchObject[j - 1];
                    searchObject[j - 1] = searchObject[j];
                    searchObject[j] = temp;
                }
            }
        }

    }
    //掴んでいる間
    private void GrabbingTime()
    {
        canGrabTime -= Time.deltaTime;
        if (canGrabTime / maxGrabTime > 0)
        {
            CaughtObjStop();
            if (canGrabTime / maxGrabTime <= 0.03)
            {
                lamp1.SetActive(false);
                lamp0.SetActive(true);
            }
            else if (canGrabTime / maxGrabTime <= 0.25)
            {
                lamp2.SetActive(false);
                lamp1.SetActive(true);
            }
            else if (canGrabTime / maxGrabTime <= 0.5)
            {
                lamp3.SetActive(false);
                lamp2.SetActive(true);
            }
            else if (canGrabTime / maxGrabTime <= 0.75)
            {
                lampMax.SetActive(false);
                lamp3.SetActive(true);
            }
            else
            {
                lampMax.SetActive(true);
            }

        }
        else
        {

            lampMax.SetActive(false);
            lamp3.SetActive(false);
            lamp2.SetActive(false);
            lamp1.SetActive(false);
            lamp0.SetActive(false);
            bossGrab = false;
            grab = false;
            canGrabTime = 3.0f;
            for (int i = 0; i < caughtObj.Count; i++)
            {
                CaughtObjMoveable(caughtObj[i]);
            }
            caughtObj.Clear();
            enemy.Clear();
            bossEnemy.Clear();
            mode = Mode.CannotGrab;
            if (ear != null)
            {
                ear.transform.parent.parent.gameObject.GetComponent<BossEar>().UndoEar(ear);
                ear = null;
            }
            // アスビ用
            gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Collect);
        }
    }

    //お化けが入る軌道上の点
    private void BezierCoordinate(Vector3 startPos, ref Vector3 p1, ref Vector3 p2, Vector3 endPos)
    {
        p1 = new Vector3(0.0f, 6.0f, 0.0f);
        p2 = new Vector3(0.0f, 6.0f, 0.0f);
    }
    //お化けを入れる
    public void SuckedIntoBox(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int i)
    {
        float ax = 0.0f, ay = 0.0f, az = 0.0f;
        float bx = 0.0f, by = 0.0f, bz = 0.0f;
        float cx = 0.0f, cy = 0.0f, cz = 0.0f;
        Vector3 vec = GetPointAtTime(time[i], ref ax, ref ay, ref az, ref bx, ref by, ref bz, ref cx, ref cy, ref cz,
                                   ref p0, ref p1, ref p2, ref p3);
        caughtObj[i].transform.position = vec;
        time[i] += Time.deltaTime;
    }

    private Vector3 GetPointAtTime(float t, ref float ax, ref float ay, ref float az, ref float bx, ref float by, ref float bz, ref float cx, ref float cy, ref float cz,
                                   ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3)
    {
        CheckConstant(ref ax, ref ay, ref az, ref bx, ref by, ref bz, ref cx, ref cy, ref cz, ref p0, ref p1, ref p2, ref p3);
        float t2 = t * t;
        float t3 = t * t * t;
        float x = ax * t3 + bx * t2 + cx * t + p0.x;
        float y = ay * t3 + by * t2 + cy * t + p0.y;
        float z = az * t3 + bz * t2 + cz * t + p0.z;
        return new Vector3(x, y, z);
    }

    private void SetConstant(ref float ax, ref float ay, ref float az, ref float bx, ref float by, ref float bz, ref float cx, ref float cy, ref float cz, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        cx = 3.0f * ((p0.x + p1.x) - p0.x);
        bx = 3.0f * ((p3.x + p2.x) - (p0.x + p1.x)) - cx;
        ax = p3.x - p0.x - cx - bx;
        cy = 3.0f * ((p0.y + p1.y) - p0.y);
        by = 3.0f * ((p3.y + p2.y) - (p0.y + p1.y)) - cy;
        ay = p3.y - p0.y - cy - by;
        cz = 3.0f * ((p0.z + p1.z) - p0.z);
        bz = 3.0f * ((p3.z + p2.z) - (p0.z + p1.z)) - cz;
        az = p3.z - p0.z - cz - bz;

    }

    private void CheckConstant(ref float ax, ref float ay, ref float az, ref float bx, ref float by, ref float bz, ref float cx, ref float cy, ref float cz,
                               ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3)
    {
        SetConstant(ref ax, ref ay, ref az, ref bx, ref by, ref bz, ref cx, ref cy, ref cz, p0, p1, p2, p3);
    }
    //捕獲したもののの組み合わせによる人形の変更
    private void DollCombination(List<GameObject> ghost)
    {
        if (ghost.Count == 1)
        {
            //リス
            if (ghost[0].transform.GetChild(0).tag == "RedEnemyBody")
            {
                doll = Instantiate(redDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Red);
                doll.tag = "RedDoll";
            }
            else if (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody")
            {
                doll = Instantiate(blueDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Blue);
                doll.tag = "BlueDoll";
            }
            else if (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody")
            {
                doll = Instantiate(yellowDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Yellow);
                doll.tag = "YellowDoll";
            }
            //猫
            else if (ghost[0].transform.GetChild(0).tag == "RedEnemyBody2")
            {
                doll = Instantiate(redDoll2, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Red);
                doll.tag = "RedDoll2";
            }
            else if (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody2")
            {
                doll = Instantiate(blueDoll2, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Blue);
                doll.tag = "BlueDoll2";
            }
            else if (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody2")
            {
                doll = Instantiate(yellowDoll2, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Yellow);
                doll.tag = "YellowDoll2";
            }
            //ボス
            else if (ghost[0].transform.GetChild(0).tag == "BossEnemyBody")
            {
                doll = Instantiate(bossDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Yellow);
                doll.tag = "BossDoll";
            }

        }
        else if (ghost.Count >= 2)
        {
            //リス
            if ((ghost[0].transform.GetChild(0).tag == "RedEnemyBody" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody")
             || (ghost[0].transform.GetChild(0).tag == "RedEnemyBody" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody2"))
            {
                doll = Instantiate(darkRedDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_DarkRed);
                doll.tag = "RedDoll";
            }
            else if ((ghost[0].transform.GetChild(0).tag == "BlueEnemyBody" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody2"))
            {
                doll = Instantiate(darkBlueDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_DarkBlue);
                doll.tag = "BlueDoll";
            }
            else if ((ghost[0].transform.GetChild(0).tag == "YellowEnemyBody" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody2"))
            {
                doll = Instantiate(darkYellowDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_DarkYellow);
                doll.tag = "YellowDoll";
            }
            else if ((ghost[0].transform.GetChild(0).tag == "RedEnemyBody" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "RedEnemyBody" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody2"))
            {
                doll = Instantiate(purpleDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Purple);
                doll.tag = "PurpleDoll";
            }
            else if ((ghost[0].transform.GetChild(0).tag == "BlueEnemyBody" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody2"))
            {
                doll = Instantiate(greenDoll, dollInstancePos.position, Quaternion.identity); ;
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Green);
                doll.tag = "GreenDoll";
            }
            else if ((ghost[0].transform.GetChild(0).tag == "RedEnemyBody" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "RedEnemyBody" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody2"))
            {
                doll = Instantiate(orangeDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Orange);
                doll.tag = "OrangeDoll";
            }
            //猫
            else if ((ghost[0].transform.GetChild(0).tag == "RedEnemyBody2" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "RedEnemyBody2" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody"))
            {
                doll = Instantiate(darkRedDoll2, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_DarkRed);
                doll.tag = "RedDoll2";
            }
            else if ((ghost[0].transform.GetChild(0).tag == "BlueEnemyBody2" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody2" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody"))
            {
                doll = Instantiate(darkBlueDoll2, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_DarkBlue);
                doll.tag = "BlueDoll2";
            }
            else if ((ghost[0].transform.GetChild(0).tag == "YellowEnemyBody2" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody2" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody"))
            {
                doll = Instantiate(darkYellowDoll2, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_DarkYellow);
                doll.tag = "YellowDoll2";
            }
            else if ((ghost[0].transform.GetChild(0).tag == "RedEnemyBody2" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody2" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "RedEnemyBody2" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody2" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody"))
            {
                doll = Instantiate(purpleDoll2, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Purple);
                doll.tag = "PurpleDoll2";
            }
            else if ((ghost[0].transform.GetChild(0).tag == "BlueEnemyBody2" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody2" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody2" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody2" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody"))
            {
                doll = Instantiate(greenDoll2, dollInstancePos.position, Quaternion.identity); ;
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Green);
                doll.tag = "GreenDoll2";
            }
            else if ((ghost[0].transform.GetChild(0).tag == "RedEnemyBody2" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody2" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody2")
                  || (ghost[0].transform.GetChild(0).tag == "RedEnemyBody2" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody2" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody"))
            {
                doll = Instantiate(orangeDoll2, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Orange);
                doll.tag = "OrangeDoll2";
            }
        }
        doll.transform.parent = playerController.transform;
        doll.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        audioSource.PlayOneShot(showoffSE);
    }
    //人形の出現
    private void DollDisclose(List<GameObject> ghost)
    {
        if (discloseTime < 1.0f)
        {
            float phase = Time.time * 2 * Mathf.PI;
            discloseTime += (Time.deltaTime/* + Mathf.Sin(phase)*/) * discloseSpeed;
            doll.transform.position = new Vector3(dollInstancePos.position.x, dollInstancePos.position.y + (discloseTime * discloseHeight), dollInstancePos.position.z);
            if (doll.tag == "BossDoll")
            {
                doll.transform.localScale = new Vector3((1.6f + (ghost.Count - 1) * 0.3f) * discloseTime, (1.6f + (ghost.Count - 1) * 0.3f) * discloseTime, (1.6f + (ghost.Count - 1) * 0.3f) * discloseTime);
            }
            else
            {
                doll.transform.localScale = new Vector3((0.7f + (ghost.Count - 1) * 0.3f) * discloseTime, (0.7f + (ghost.Count - 1) * 0.3f) * discloseTime, (0.7f + (ghost.Count - 1) * 0.3f) * discloseTime);
            }
        }
        else if (disclose == false)
        {
            disclose = true;
            doll.transform.position = new Vector3(dollInstancePos.position.x, dollInstancePos.position.y + discloseHeight, dollInstancePos.position.z);
            if (doll.tag == "BossDoll")
            {
                doll.transform.localScale = new Vector3((1.6f + (ghost.Count - 1) * 0.3f) * discloseTime, (1.6f + (ghost.Count - 1) * 0.3f) * discloseTime, (1.6f + (ghost.Count - 1) * 0.3f) * discloseTime);
            }
            else
            {
                doll.transform.localScale = new Vector3((0.7f + (ghost.Count - 1) * 0.3f), (0.7f + (ghost.Count - 1) * 0.3f), (0.7f + (ghost.Count - 1) * 0.3f));
            }

        }
    }
    //人形の振動
    private void DollVibrate()
    {
        if (vibrate == false)
        {
            vibrate = true;
            initPosition = dollInstancePos2.position.y;
            newPosition = initPosition;
            minPosition = initPosition - vibrateRange;
            maxPosition = initPosition + vibrateRange;
            audioSource.PlayOneShot(dollVibrateSE);
        }
        Vibrate();

    }
    //縦揺れ
    private void Vibrate()
    {
        if (newPosition <= minPosition || maxPosition <= newPosition)
        {
            directionToggle = !directionToggle;
        }

        newPosition = directionToggle ? newPosition + (vibrateSpeed * Time.deltaTime) : newPosition - (vibrateSpeed * Time.deltaTime);
        newPosition = Mathf.Clamp(newPosition, minPosition, maxPosition);
        //doll.transform.localPosition = new Vector3(doll.transform.position.x, doll.transform.position.y+newPosition, doll.transform.position.z);
        doll.transform.position = new Vector3(dollInstancePos2.position.x, newPosition, dollInstancePos2.position.z);
    }
    //箱に入れる
    private void DollInhale()
    {
        if (doll != null)
        {
            float inhaleDis = Vector3.Distance(doll.transform.position, inhalePos.position);
            if (inhaleSpeed < inhaleSpeedMax)
            {
                inhaleSpeed += Time.deltaTime * inhaleAccelerate;
            }

            if (inhaleDis < 0.03f)
            {
                inhaleSpeed = 0.0f;
                dollSave.DollAdd(doll, caughtObj.Count);
                dollSave.slow = false;
                if (doll.tag == "BossDoll")
                {
                    dollSave.bossIn = true;
                }
                doll.SetActive(false);
                doll = null;
            }
            else
            {
                dollSave.SlowSound();
                doll.transform.position = Vector3.MoveTowards(doll.transform.position, inhalePos.position, Time.deltaTime * ((inhaleSpeed * inhaleSpeed) + inhaleFirstSpeed));
            }
        }
    }
    //捕まえたものを止める
    private void CaughtObjStop()
    {
        for (int i = 0; i < caughtObj.Count; i++)
        {
            if (caughtObj[i].tag == "NormalGhost" || caughtObj[i].tag == "DonyoriGhost")
            {
                if (caughtObj[i].GetComponent<NavMeshAgent>().isActiveAndEnabled)
                {
                    caughtObj[i].GetComponent<NavMeshAgent>().isStopped = true;
                }
            }
            else if (caughtObj[i].tag == "BossEnemy")
            {
                caughtObj[i].GetComponent<BossEnemy>().mode = BossEnemy.Mode.grabbed;
                caughtObj[i].GetComponent<BossEnemy>().reSet = false;
                if (caughtObj[i].GetComponent<NavMeshAgent>().isActiveAndEnabled)
                {
                    caughtObj[i].GetComponent<NavMeshAgent>().isStopped = true;
                }
            }
        }
    }
    //捕まえたものを動けるようにする
    public void CaughtObjMoveable(GameObject enemy)
    {
        if (enemy.tag == "NormalGhost" || enemy.tag == "DonyoriGhost")
        {
            if (enemy.tag == "NormalGhost")
            {
                enemy.GetComponent<EnemyBehaviour>().enabled = true;
                enemy.transform.parent = GameObject.Find("Enemies").transform;
            }
            else if (enemy.tag == "DonyoriGhost")
            {
                enemy.GetComponent<DonyoriBehaviour>().enabled = true;
                enemy.transform.parent = GameObject.Find("DonyoriEnemies").transform;
            }

            enemy.GetComponent<NavMeshAgent>().isStopped = false;
        }
        else if (enemy.tag == "BossEnemy")
        {
            enemy.GetComponent<BossEnemy>().reSet = false;
            enemy.GetComponent<NavMeshAgent>().isStopped = false;
            enemy.transform.parent = null;
        }
    }
    //リセットする
    public void ReSetCatch()
    {
        if (doll != null)
        {
            doll.SetActive(false);
            doll = null;
        }
        enemy.Clear();
        bossEnemy.Clear();
        caughtObj.Clear();
        caughtObjPos.Clear();
        time.Clear();
        canGrabTime = 3.0f;
    }
    //モード変更
    public void SetState(Mode temp)
    {
        mode = temp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NormalGhost" || other.tag == "DonyoriGhost")
        {
            Debug.Log("hit");
            if (!enemy.Contains(other.gameObject))
            {
                enemy.Add(other.gameObject);

            }
        }
        if (other.tag == "BossEnemy" || other.tag == "BossEarLeft" || other.tag == "BossEarRight")
        {
            Debug.Log("hit");
            if (!bossEnemy.Contains(other.gameObject))
            {
                bossEnemy.Add(other.gameObject);

            }
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "NormalGhost" || other.tag == "DonyoriGhost")
        {
            if (enemy.Contains(other.gameObject))
            {
                enemy.Remove(other.gameObject);

            }
        }
        if (other.tag == "BossEnemy" || other.tag == "BossEarLeft" || other.tag == "BossEarRight")
        {
            if (bossEnemy.Contains(other.gameObject))
            {
                bossEnemy.Remove(other.gameObject);

            }
        }
    }
}
