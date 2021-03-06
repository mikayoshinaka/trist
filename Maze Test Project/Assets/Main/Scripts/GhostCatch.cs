using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class GhostCatch : MonoBehaviour
{
    public List<GameObject> enemy = new List<GameObject>();
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
    [SerializeField] private Material ghostRedMaterial;
    [SerializeField] private Material ghostBlueMaterial;
    [SerializeField] private Material ghostYellowMaterial;
    [SerializeField] private Transform dollInstancePos;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform inhalePos;
    [SerializeField] Possess possessScript;
    public bool grab;
    bool zoom;
    bool disclose;
    bool directionToggle;
    bool vibrate;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerController;
    [SerializeField] public GameObject presentBox;
    [SerializeField] private GameObject image;
    [SerializeField] private GameObject image2;
    [SerializeField] private Transparent transparent;
    public GameObject doll = null;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject dollInstanceZoom1;
    [SerializeField] private GameObject dollInstanceZoom2;
    [SerializeField] private GameObject shootCamera;
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

    [SerializeField] private float vibrateRange = 0.5f, vibrateSpeed = 10.0f;
    float initPosition,newPosition,minPosition ,maxPosition;
    //[SerializeField] private int partition = 20;
    private List<float> time = new List<float>();
    public enum Mode
    {
        CanGrab,
        Fusion,
        Instance,
        Carry,
        Shoot,
        CannotGrab,
        Attacked
    }
    public Mode mode;

    // アスビ用
    private GameStateManager gameStateManager;
    private ColorAction colorAction;

    void Start()
    {
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
        image.SetActive(false);
        image2.SetActive(false);

        // アスビ用
        gameStateManager = GameObject.Find("GameState").GetComponent<GameStateManager>();
        colorAction = GameObject.Find("PlayerController").GetComponent<ColorAction>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (mode)
        {
            case Mode.CanGrab:
                transparent.catchEnemy = false;
                GhostGrab();
                break;
            case Mode.Fusion:
                GhostFusion();
                break;
            case Mode.Instance:
                DollInstance();
                break;
            case Mode.Carry:
                transparent.catchEnemy = true;
                DollCarry();
                break;
            case Mode.Shoot:
                DollShoot();
                break;
            case Mode.CannotGrab:
                CannotCatch();
                break;
            case Mode.Attacked:
                transparent.catchEnemy = false;
                EnemyAttacked();
                break;
        }
        if (mode != Mode.CanGrab|| mode != Mode.Attacked)
        {
            CaughtObjStop();
        }
    }
    //  敵を掴む
    private void GhostGrab()
    {
        if ((Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton0)) && enemy.Count > 0&&possessScript.possess==false)
        {
            if (enemy.Count > 1)
            {
                Sort(0, enemy.Count - 1, ref enemy);
            }
            if (grab == false)
            {
                grab = true;
            }
            if (image.activeSelf == false)
            {
                image.SetActive(true);
                image2.SetActive(true);
            }
            for (int i = 0; i < enemy.Count; i++)
            {
                if (!caughtObj.Contains(enemy[i]))
                {
                    enemy[i].GetComponent<EnemyBehaviour>().enabled = false;
                    enemy[i].transform.parent = player.transform;
                    caughtObj.Add(enemy[i]);
                }
            }
            if (canGrabTime / maxGrabTime > 0)
            {
                GrabbingTime();
            }
        }
        else if ((Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton0)) && grab == true && possessScript.possess == false)
        {
            if (enemy.Count > 1)
            {
                Sort(0, enemy.Count - 1, ref enemy);
            }
            for (int i = 0; i < enemy.Count; i++)
            {
                if (!caughtObj.Contains(enemy[i]))
                {
                    enemy[i].GetComponent<EnemyBehaviour>().enabled = false;
                    enemy[i].transform.parent = player.transform;
                    caughtObj.Add(enemy[i]);
                }
            }
            GrabbingTime();
        }
        else if (grab == true && mode == Mode.CanGrab)
        {
            grab = false;
            for (int i = 0; i < caughtObj.Count; i++)
            {
                caughtObj[i].transform.parent = null;
                Vector3 startPos = transform.InverseTransformPoint(caughtObj[i].transform.GetChild(0).position);
                caughtObjPos.Add(startPos);
                time.Add(0.0f);
            }
            image.SetActive(false);
            image2.SetActive(false);
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
        }
        dollInstanceTime += Time.deltaTime;
        if (dollInstanceTime < dollInstanceMaxTime)
        {
            return;
        }
        playerController.GetComponent<CharacterMovementScript>().enabled = true;
        dollInstanceZoom1.SetActive(false);
        dollInstanceZoom2.SetActive(false);
        dollInstanceTime = 0.0f;
        discloseTime = 0.0f;
        zoom = false;
        disclose = false;
        mode = Mode.Carry;
        // アスビ用
        gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Deliver);
    }

    // 人形を運ぶ
    private void DollCarry()
    {
        if (doll == null && !(Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton0)))
        {
            mode = Mode.CanGrab;

            // アスビ用
            gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Collect);
        }
    }
    //人形を箱に入れる
    private void DollShoot()
    {
        if (shootTime==0.0f) {
            playerController.transform.position = new Vector3(shootPos.position.x, playerController.transform.position.y, shootPos.position.z);
            playerController.transform.LookAt(inhalePos);
            playerController.GetComponent<CharacterMovementScript>().enabled = false;
            mainCamera.SetActive(false);
            shootCamera.SetActive(true);
        }
        shootTime += Time.deltaTime;
        if(shootTime>0.0f&&shootTime<vibrateTime)
        {
            DollVibrate();
        }
        else if(shootTime>=vibrateTime&&shootTime<vibrateTime+inhaleTime)
        {
            vibrate = false;
            directionToggle = false;
            DollInhale();
        }
        else
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

    }
    // 捕まえることができない
    private void CannotCatch()
    {
        notCatchTime += Time.deltaTime;
        if (notCatchTime > 2.0f && !(Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton0)))
        {
            ReSetCatch();
            notCatchTime = 0.0f;
            grab = false;
            mode = Mode.CanGrab;

            // アスビ用
            gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Collect);
        }
    }
    private void EnemyAttacked()
    {
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
            mode = Mode.CannotGrab;
        }
    }

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


        //int i, j, step;
        //GameObject pv;
        //if (right - left >= 1)
        //{
        //    pv = searchObject[left];
        //    i = left;
        //    j = right;
        //    step = 1;
        //    while (i < j)
        //    {
        //        if (step == 1)
        //        {
        //            if ((searchObject[j].transform.position - player.transform.position).magnitude < (pv.transform.position - player.transform.position).magnitude)
        //            {
        //                searchObject[i++] = searchObject[j];
        //                step = 2;
        //            }
        //            else
        //            {
        //                j--;
        //            }
        //        }
        //        else
        //        {
        //            if ((searchObject[i].transform.position - player.transform.position).magnitude >= (pv.transform.position - player.transform.position).magnitude)
        //            {
        //                searchObject[j--] = searchObject[i];
        //                step = 1;
        //            }
        //            else
        //            {
        //                i++;
        //            }
        //        }
        //        searchObject[j] = pv;
        //        Sort(left, j - 1, ref searchObject);
        //        Sort(j + 1, right, ref searchObject);
        //    }
        //}

    }
    private void GrabbingTime()
    {
        canGrabTime -= Time.deltaTime;
        if (canGrabTime / maxGrabTime > 0)
        {
            CaughtObjStop();
            image.GetComponent<Image>().fillAmount = canGrabTime / maxGrabTime;
        }
        else
        {
            image.GetComponent<Image>().fillAmount = 0;
            image.SetActive(false);
            image2.SetActive(false);
            canGrabTime = 3.0f;
            for (int i = 0; i < caughtObj.Count; i++)
            {
                CaughtObjMoveable(caughtObj[i]);
            }
            caughtObj.Clear();
            enemy.Clear();
            mode = Mode.CannotGrab;

            // アスビ用
            gameStateManager.ChangeGameState(GameStateManager.GameState.gameState_Collect);
        }
    }

    private void BezierCoordinate(Vector3 startPos, ref Vector3 p1, ref Vector3 p2, Vector3 endPos)
    {
        //p1 = new Vector3((startPos.x + ((startPos.x + endPos.x) / 2.0f)) / 2.0f, 6.0f, (startPos.z + ((startPos.z + endPos.z) / 2.0f)) / 2.0f);
        //p2 = new Vector3((endPos.x + ((startPos.x + endPos.x) / 2.0f)) / 2.0f, 6.0f, (endPos.z + ((startPos.z + endPos.z) / 2.0f)) / 2.0f);
        //p1 = new Vector3(((startPos.x + endPos.x) / 2.0f), 6.0f, ((startPos.z + endPos.z) / 2.0f));
        //p2 = new Vector3(((startPos.x + endPos.x) / 2.0f), 6.0f, ((startPos.z + endPos.z) / 2.0f));
        //p1 = new Vector3((endPos.x + ((startPos.x + endPos.x) / 2.0f)) / 2.0f, 4.0f, (endPos.z + ((startPos.z + endPos.z) / 2.0f)) / 2.0f);
        //p1 = new Vector3(player.transform.position.x, 10.0f, player.transform.position.z);
        //p2 = new Vector3(player.transform.position.x, 10.0f, player.transform.position.z);
        p1 = new Vector3(0.0f, 6.0f, 0.0f);
        p2 = new Vector3(0.0f, 6.0f, 0.0f);
    }

    public void SuckedIntoBox(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int i)
    {

        Vector3 b0 = Vector3.zero;
        Vector3 b1 = Vector3.zero;
        Vector3 b2 = Vector3.zero;
        Vector3 b3 = Vector3.zero;
        float ax = 0.0f, ay = 0.0f, az = 0.0f;
        float bx = 0.0f, by = 0.0f, bz = 0.0f;
        float cx = 0.0f, cy = 0.0f, cz = 0.0f;
        Vector3 vec = GetPointAtTime(time[i], ref ax, ref ay, ref az, ref bx, ref by, ref bz, ref cx, ref cy, ref cz,
                                   ref p0, ref p1, ref p2, ref p3, ref b0, ref b1, ref b2, ref b3);
        caughtObj[i].transform.position = vec;
        time[i] += Time.deltaTime;
    }

    private Vector3 GetPointAtTime(float t, ref float ax, ref float ay, ref float az, ref float bx, ref float by, ref float bz, ref float cx, ref float cy, ref float cz,
                                   ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector3 b0, ref Vector3 b1, ref Vector3 b2, ref Vector3 b3)
    {
        CheckConstant(ref ax, ref ay, ref az, ref bx, ref by, ref bz, ref cx, ref cy, ref cz, ref p0, ref p1, ref p2, ref p3, ref b0, ref b1, ref b2, ref b3);
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
                               ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector3 b0, ref Vector3 b1, ref Vector3 b2, ref Vector3 b3)
    {
        if (p0 != b0 || p1 != b1 || p2 != b2 || p3 != b3)
        {
            SetConstant(ref ax, ref ay, ref az, ref bx, ref by, ref bz, ref cx, ref cy, ref cz, p0, p1, p2, p3);
            b0 = p0;
            b1 = p1;
            b2 = p2;
            b3 = p3;
        }
    }
    private void DollCombination(List<GameObject> ghost)
    {
        if (ghost.Count == 1)
        {
            if (ghost[0].transform.GetChild(0).tag == "RedEnemyBody")
            {
                doll = Instantiate(redDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Red);
            }
            else if (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody")
            {
                doll = Instantiate(blueDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Blue);
            }
            else if (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody")
            {
                doll = Instantiate(yellowDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Yellow);
            }
        }
        else if (ghost.Count >= 2)
        {
            if (ghost[0].transform.GetChild(0).tag == "RedEnemyBody" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody")
            {
                doll = Instantiate(darkRedDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_DarkRed);
            }
            else if (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody")
            {
                doll = Instantiate(darkBlueDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_DarkBlue);
            }
            else if (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody")
            {
                doll = Instantiate(darkYellowDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_DarkYellow);
            }
            else if ((ghost[0].transform.GetChild(0).tag == "RedEnemyBody" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "BlueEnemyBody" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody"))
            {
                doll = Instantiate(purpleDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Purple);
            }
            else if ((ghost[0].transform.GetChild(0).tag == "BlueEnemyBody" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody" && ghost[1].transform.GetChild(0).tag == "BlueEnemyBody"))
            {
                doll = Instantiate(greenDoll, dollInstancePos.position, Quaternion.identity); ;
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Green);
            }
            else if ((ghost[0].transform.GetChild(0).tag == "RedEnemyBody" && ghost[1].transform.GetChild(0).tag == "YellowEnemyBody")
                  || (ghost[0].transform.GetChild(0).tag == "YellowEnemyBody" && ghost[1].transform.GetChild(0).tag == "RedEnemyBody"))
            {
                doll = Instantiate(orangeDoll, dollInstancePos.position, Quaternion.identity);
                colorAction.ChooseColorAction(ColorAction.ColorGimmick.gimmick_Orange);
            }
        }
        doll.transform.parent = playerController.transform;
        doll.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void DollDisclose(List<GameObject> ghost)
    {
        if (discloseTime < 1.0f)
        {
            float phase = Time.time * 2 * Mathf.PI;
            discloseTime += (Time.deltaTime/* + Mathf.Sin(phase)*/) * discloseSpeed;
            doll.transform.position = new Vector3(dollInstancePos.position.x, dollInstancePos.position.y + (discloseTime * discloseHeight), dollInstancePos.position.z);
            doll.transform.localScale = new Vector3((1.0f + (ghost.Count - 1) * 0.3f) * discloseTime, (1.0f + (ghost.Count - 1) * 0.3f) * discloseTime, (1.0f + (ghost.Count - 1) * 0.3f) * discloseTime);
        }
        else if (disclose == false)
        {
            disclose = true;
            doll.transform.position = new Vector3(dollInstancePos.position.x, dollInstancePos.position.y + discloseHeight, dollInstancePos.position.z);
            doll.transform.localScale = new Vector3((1.0f + (ghost.Count - 1) * 0.3f), (1.0f + (ghost.Count - 1) * 0.3f), (1.0f + (ghost.Count - 1) * 0.3f));
        }
    }
    private void DollVibrate()
    {
        if (vibrate == false)
        {
            vibrate = true;
            initPosition = doll.transform.position.y;
            newPosition = initPosition;
            minPosition = initPosition - vibrateRange;
            maxPosition = initPosition + vibrateRange;
        }
        Vibrate();

    }
    private void Vibrate()
    {
        if (newPosition<=minPosition||maxPosition<=newPosition)
        {
            directionToggle = !directionToggle;
        }

        newPosition = directionToggle ? newPosition + (vibrateSpeed * Time.deltaTime) : newPosition - (vibrateSpeed * Time.deltaTime);
        newPosition = Mathf.Clamp(newPosition, minPosition, maxPosition);
        //doll.transform.localPosition = new Vector3(doll.transform.position.x, doll.transform.position.y+newPosition, doll.transform.position.z);
        doll.transform.localPosition = new Vector3(0, newPosition,0);
    }
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
                dollSave.dolls.Add(doll);
                doll.SetActive(false);
                doll = null;
            }
            else
            {
                doll.transform.position = Vector3.MoveTowards(doll.transform.position, inhalePos.position, Time.deltaTime * ((inhaleSpeed * inhaleSpeed) + inhaleFirstSpeed));
            }
        }
    }
    private void CaughtObjStop()
    {
        for (int i = 0; i < caughtObj.Count; i++)
        {
            if (caughtObj[i].GetComponent<NavMeshAgent>().isActiveAndEnabled)
            {
                caughtObj[i].GetComponent<NavMeshAgent>().isStopped = true;
            }
        }
    }
    public void CaughtObjMoveable(GameObject enemy)
    {
        enemy.GetComponent<EnemyBehaviour>().enabled = true;
        enemy.GetComponent<NavMeshAgent>().isStopped = false;
        enemy.transform.parent = null;
    }

    public void ReSetCatch()
    {
        if (doll != null)
        {
            doll.SetActive(false);
            doll = null;
        }
        enemy.Clear();
        caughtObj.Clear();
        caughtObjPos.Clear();
        time.Clear();
        canGrabTime = 3.0f;
    }
    public void SetState(Mode temp)
    {
        mode = temp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("hit");
            if (!enemy.Contains(other.gameObject))
            {
                enemy.Add(other.gameObject);

            }
        }


    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (enemy.Contains(other.gameObject))
            {
                enemy.Remove(other.gameObject);

            }
        }
    }

}
