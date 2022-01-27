using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class BossEnemy : MonoBehaviour
{
    //ボス自身
    Animator animator;
    [SerializeField] BossEar bossEar;
    [SerializeField] private float leftBossEarAngle = 25.0f;
    [SerializeField] private float rightBossEarAngle = 25.0f;
    Vector3 firstBossSize;
    [SerializeField] private int bossHPMax = 3;
    public int bossHP;
    public bool hpDown;
    private float bossSize = 1.0f;
    //攻撃開始
    [SerializeField] float changeTimerMax = 2.0f;
    public float changeTimer = 0.0f;
    [SerializeField] float hitPlayerDis = 3.0f;
    //火球
    [SerializeField] float radius = 10;
    [SerializeField] GameObject fireBall;
    [SerializeField] VisualEffect effect;
    [SerializeField] GameObject player;
    [SerializeField] int fireBallCount = 8;
    [SerializeField] float y;
    [SerializeField] float fireSpeed = 40.0f;
    [SerializeField] float fireRotateSpeed = 60.0f;
    private float rotateAngle;
    [SerializeField] float fireTimerMax = 1.0f;
    public float fireTimer = 0.0f;
    public List<GameObject> fireBalls = new List<GameObject>();
    int fireAttackCount;
    int fire;
    private float ballAjust = 1.8f;
    bool startFireInstance;
    bool fireInstance;
    Vector3 instantPlayerPos;
    //レーザー
    [SerializeField] GameObject laser;
    private LineRenderer lr;
    [SerializeField] float laserTimerMax = 4.0f;
    public float laserTimer = 0.0f;
    [SerializeField] float laserSpeed = 30.0f;
    [SerializeField] float laserMaxAngle = 30.0f;
    private float horizontalAngle;
    private float horizontalAngleLimit = 45.0f;
    bool laserStart;
    //リセット
    public bool reSet;
    private float grabbedDownTime = 0.0f;
    private float grabbedDownTimeMax = 1.0f;
    //移動
    public List<Vector3> sourcePos = new List<Vector3>();
    private float randomMoveTimer;
    [SerializeField] float randomMoveTimerMax = 1.0f;
    [SerializeField] float randomMoveRange = 3.0f;
    private int randomMoveCount;
    public NavMeshAgent agent;
    [SerializeField] float chaseDis = 5.0f;
    private Vector3 beforePlayerPos;
    private Vector3 playerAmountOfMovement = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 beforeBossPos;
    private Vector3 bossAmountOfMovement = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private Transform[] mig_Point;
    private int point = 0;

    AudioSource audioSource;
    public AudioClip instanceFireSE;
    public AudioClip beamSE;
    public GameObject bossSound;
    public enum Mode
    {
        fire,
        beam,
        change,
        down,
        grabbed,
        randomMove,
        chase,
        definePosMove
    }

    public Mode mode;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
        firstBossSize = new Vector3(this.transform.GetChild(0).localScale.x, this.transform.GetChild(0).localScale.y, this.transform.GetChild(0).localScale.z);
        reSet = false;
        bossHP = bossHPMax;
        hpDown = false;
        mode = Mode.definePosMove;
        rotateAngle = 0.0f;
        fire = 0;
        fireAttackCount = 0;
        startFireInstance = false;
        fireInstance = false;

        lr = laser.GetComponent<LineRenderer>();
        laserStart = false;
        horizontalAngle = laserMaxAngle;
        agent = GetComponent<NavMeshAgent>();

        beforePlayerPos = player.transform.position;
        beforeBossPos = this.transform.position;
        randomMoveCount = 0;

        audioSource = bossSound.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hpDown == true)
        {
            SizeDown();
        }
        switch (mode)
        {
            case Mode.fire:
                FireShoot();
                break;
            case Mode.beam:
                LaserShoot();
                break;
            case Mode.change:
                ModeChange();
                break;
            case Mode.down:
                BossDown();
                break;
            case Mode.grabbed:
                BossGrabbed();
                break;
            case Mode.randomMove:
                BossMoveRandom();
                break;
            case Mode.chase:
                BossMoveChase();
                break;
            case Mode.definePosMove:
                BossMoveSetPosition();
                break;
        }
        CalculateAmountOfMovement(ref playerAmountOfMovement, ref beforePlayerPos, player.transform.position);
        CalculateAmountOfMovement(ref bossAmountOfMovement, ref beforeBossPos, this.gameObject.transform.position);
    }
    //hp減少によるボス強化
    void BossPowerUp()
    {
        fireBallCount = 12;
        laserSpeed = 50.0f;
        fireSpeed = 50.0f;
        changeTimerMax = 2.0f;
        agent.speed = 5.0f;
    }
    //サイズ変更
    void SizeDown()
    {
        if (bossSize > 0.0f)
        {
            animator.SetBool("Attacked", true);
            bossSize -= Time.deltaTime /** 0.2f*/;
            this.transform.GetChild(0).localScale = new Vector3(firstBossSize.x / (bossHPMax + 1) * ((float)(bossHP + 1 / bossHPMax + 1) + ((float)(1 / bossHPMax + 1) * bossSize)), firstBossSize.y / (bossHPMax + 1) * ((float)(bossHP + 1 / bossHPMax + 1) + ((float)(1 / bossHPMax + 1) * bossSize)), firstBossSize.z / (bossHPMax + 1) * ((float)(bossHP + 1 / bossHPMax + 1) + ((float)(1 / bossHPMax + 1) * bossSize)));
        }
        else if (bossSize <= 0.0f)
        {
            this.transform.GetChild(0).localScale = new Vector3(firstBossSize.x / (bossHPMax + 1) * (float)(bossHP + 1 / bossHPMax + 1), firstBossSize.y / (bossHPMax + 1) * (float)(bossHP + 1 / bossHPMax + 1), firstBossSize.z / (bossHPMax + 1) * (float)(bossHP + 1 / bossHPMax + 1));
            animator.SetBool("Attacked", false);
            bossSize = 1.0f;
            hpDown = false;
        }
    }

    //火球攻撃
    void FireShoot()
    {
        transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
        if (startFireInstance == false)
        {
            startFireInstance = true;
            StartCoroutine(GenerateFire());
        }
        if (fireInstance && fireAttackCount < fireBalls.Count / 2)
        {
            fireTimer += Time.deltaTime;
            if (fireAttackCount > 0)
            {
                FireSet();
            }
            if (fireTimer > fireTimerMax)
            {
                animator.SetBool("Fire", true);
                fireTimer = 0.0f;
                fireAttackCount += 1;
                rotateAngle = 0.0f;
                instantPlayerPos = player.transform.position;
                Vector3 terminusDirection1 = FireTerminus(fireBalls[fireAttackCount - 1].transform.position, new Vector3(instantPlayerPos.x, instantPlayerPos.y + ballAjust, instantPlayerPos.z));
                Vector3 terminusDirection2 = FireTerminus(fireBalls[fireBalls.Count - fireAttackCount].transform.position, new Vector3(instantPlayerPos.x, instantPlayerPos.y + ballAjust, instantPlayerPos.z));
                StartCoroutine(AttackingFire(instantPlayerPos, fireBalls[fireAttackCount - 1], fireBalls[fireBalls.Count - fireAttackCount], terminusDirection1, terminusDirection2));
            }
        }
        else if (fireInstance && fireAttackCount >= fireBalls.Count / 2)
        {
            startFireInstance = false;
            fireInstance = false;
            fireBalls.Clear();
            fireAttackCount = 0;
            animator.SetBool("Fire", false);
            mode = Mode.change;
        }
    }

    //火球生成
    IEnumerator GenerateFire()
    {
        yield return new WaitForSeconds(0.5f);
        if (mode == Mode.fire)
        {
            audioSource.PlayOneShot(instanceFireSE);
            InstanceFire(fire);
            fire += 2;
            if (fire < fireBallCount)
            {
                StartCoroutine(GenerateFire());
            }
            else
            {
                fire = 0;
                fireInstance = true;
                instantPlayerPos = player.transform.position;
            }
        }
        else
        {
            yield break;
        }

    }
    void InstanceFire(int fire)
    {
        float angleDiff = (360.0f / (float)fireBallCount);
        for (int i = fire; i < fire + 2; i++)
        {
            if (fire >= fireBallCount)
            {
                break;
            }
            Vector3 firePos = new Vector3(transform.position.x, transform.position.y + y, transform.position.z);
            float angle = (90.0f - ((angleDiff * i + transform.localEulerAngles.y) + angleDiff / 2.0f)) * Mathf.Deg2Rad;
            firePos.x += radius * Mathf.Cos(angle);
            firePos.z += radius * Mathf.Sin(angle);
            fireBalls.Add(Instantiate(fireBall, firePos, Quaternion.identity));
            fireBalls[i].transform.parent = this.gameObject.transform;
        }
    }

    //火球移動
    void FireAttack(Vector3 playerPos, GameObject fireBall1, GameObject fireBall2, Vector3 dir1, Vector3 dir2)
    {

        fireBall1.tag = "FireBall";
        fireBall2.tag = "FireBall";
        fireBall1.transform.parent = null;
        fireBall2.transform.parent = null;
        if (fireBall1.activeSelf == true)
        {
            //fireBall1.transform.position = Vector3.MoveTowards(fireBall1.transform.position, new Vector3(playerPos.x,playerPos.y+ballAjust,playerPos.z), Time.deltaTime * fireSpeed);
            fireBall1.transform.position += dir1 * fireSpeed * Time.deltaTime;
            fireBall1.GetComponent<Explosion>().explode = true;
        }
        if (fireBall2.activeSelf == true)
        {
            //fireBall2.transform.position = Vector3.MoveTowards(fireBall2.transform.position, new Vector3(playerPos.x, playerPos.y + ballAjust, playerPos.z), Time.deltaTime * fireSpeed);
            fireBall2.transform.position += dir2 * fireSpeed * Time.deltaTime;
            fireBall2.GetComponent<Explosion>().explode = true;
        }

        float dis1 = Vector3.Distance(fireBall1.transform.position, new Vector3(playerPos.x, playerPos.y + ballAjust, playerPos.z));
        float dis2 = Vector3.Distance(fireBall2.transform.position, new Vector3(playerPos.x, playerPos.y + ballAjust, playerPos.z));

        if (dis1 < 1.0f && fireBall1.activeSelf == true)
        {
            Instantiate(effect, fireBall1.transform.position, Quaternion.identity);
            fireBall1.SetActive(false);
        }
        if (dis2 < 1.0f && fireBall2.activeSelf == true)
        {
            Instantiate(effect, fireBall2.transform.position, Quaternion.identity);
            fireBall2.SetActive(false);
        }
    }
    //火球の向かう方向
    Vector3 FireTerminus(Vector3 startPos, Vector3 endPos)
    {
        Vector3 heading = endPos - startPos;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        return direction;
    }
    //火球攻撃　移動
    IEnumerator AttackingFire(Vector3 playerPos, GameObject fireBall1, GameObject fireBall2, Vector3 dir1, Vector3 dir2)
    {
        if (reSet == false)
        {
            while (fireBall1.activeSelf == true || fireBall2.activeSelf == true)
            {
                FireAttack(playerPos, fireBall1, fireBall2, dir1, dir2);
                if (reSet == true)
                {
                    Destroy(fireBall1);
                    Destroy(fireBall2);
                    yield break;
                }
                yield return null;
            }
            while (mode == Mode.fire)
            {
                yield return null;
            }
            Destroy(fireBall1);
            Destroy(fireBall2);
        }
        else
        {
            yield break;
        }

    }
    //火球の出現場所
    void FireSet()
    {
        float angleDiff = 360.0f / (float)fireBallCount;
        rotateAngle += Time.deltaTime;
        if (rotateAngle < angleDiff)
        {
            rotateAngle += fireRotateSpeed * Time.deltaTime;
            for (int i = fireAttackCount; i < fireBalls.Count - fireAttackCount; i++)
            {
                Quaternion angleAxis;
                if (i < fireBalls.Count / 2)
                {
                    angleAxis = Quaternion.AngleAxis(-fireRotateSpeed * Time.deltaTime, Vector3.up);

                }
                else
                {
                    angleAxis = Quaternion.AngleAxis(fireRotateSpeed * Time.deltaTime, Vector3.up);

                }
                Vector3 firePos = fireBalls[i].transform.position;
                firePos -= new Vector3(transform.position.x, y, transform.position.z);
                firePos = angleAxis * firePos;
                firePos += new Vector3(transform.position.x, y, transform.position.z);
                fireBalls[i].transform.position = firePos;
            }
        }

    }
    //レーザーを打つ
    void LaserShoot()
    {
        if (laserStart == false)
        {
            laserStart = true;
            transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
            animator.SetBool("Beam", true);
            audioSource.clip = beamSE;
            audioSource.Play();
        }
        laserTimer += Time.deltaTime;
        LaserMigration();
        if (laserTimer > laserTimerMax)
        {
            laserStart = false;
            lr.positionCount = 0;
            lr.positionCount = 2;
            laserTimer = 0.0f;
            mode = Mode.change;
            horizontalAngle = laserMaxAngle;
            animator.SetBool("Beam", false);
            audioSource.Stop();
            if (laserSpeed < 0)
            {
                laserSpeed *= -1;
            }
        }
    }
    //レーザーの移動
    void LaserMigration()
    {
        LaserRotation();
        lr.SetPosition(0, laser.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(laser.transform.position, laser.transform.forward, out hit))
        {
            if (hit.collider.tag == "PlayerBody")
            {
                hit.collider.GetComponent<ParalysisPlayer>().paralysis = true;
                lr.SetPosition(1, hit.point + laser.transform.forward * 1);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("UpStages") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Stages"))
            {
                lr.SetPosition(1, hit.point);
            }
            else
            {
                lr.SetPosition(1, laser.transform.forward * 500);
            }

        }
        else
        {
            lr.SetPosition(1, laser.transform.forward * 500);
        }
    }
    //レーザーの回転
    void LaserRotation()
    {
        horizontalAngle += Time.deltaTime * -laserSpeed;
        laser.transform.localRotation = Quaternion.Euler(7.0f, horizontalAngle, 0.0f);
        if (horizontalAngle > horizontalAngleLimit)
        {
            horizontalAngle = horizontalAngleLimit;
            laserSpeed *= -1;
        }
        else if (horizontalAngle < -horizontalAngleLimit)
        {
            horizontalAngle = -horizontalAngleLimit;
            laserSpeed *= -1;
        }
    }
    //モード変更
    void ModeChange()
    {
        bool hitPlayer = InArea(hitPlayerDis);
        if (changeTimer < changeTimerMax)
        {
            changeTimer += Time.deltaTime;
        }
        else if (changeTimer > changeTimerMax)
        {
            if (hitPlayer)
            {
                int modeChange;
                modeChange = Random.Range(0, 2);
                if (modeChange == 0)
                {
                    mode = Mode.fire;
                }
                else if (modeChange == 1)
                {
                    mode = Mode.beam;
                }
                if (GetComponent<NavMeshAgent>().isActiveAndEnabled == true)
                {
                    GetComponent<NavMeshAgent>().isStopped = true;
                }
                changeTimer = 0.0f;
            }
            else
            {
                changeTimer = 0.0f;
                GetComponent<NavMeshAgent>().isStopped = false;
                mode = Mode.randomMove;
            }
        }
    }
    //エリア内にプレイヤーがいるかどうか
    bool InArea(float hitDis)
    {
        float dis = Vector3.Distance(player.transform.position, this.transform.position);
        if (dis < hitDis)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //ボスが倒れたとき
    void BossDown()
    {
        agent.agentTypeID = 0;
    }
    //掴まれた
    void BossGrabbed()
    {
        if (bossHP > 0 && player.transform.GetChild(0).GetChild(3).GetComponent<GhostCatch>().mode == GhostCatch.Mode.CanGrab)
        {

            if (bossEar.earNum == 1)
            {
                transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y - leftBossEarAngle, 0);
            }
            else if (bossEar.earNum == 2)
            {
                transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y + rightBossEarAngle, 0);
            }
            else
            {
                this.gameObject.transform.LookAt(new Vector3(this.gameObject.transform.position.x + (this.transform.position.x - player.transform.position.x), this.gameObject.transform.position.y, this.gameObject.transform.position.z + (this.transform.position.z - player.transform.position.z)));
            }
        }

        ResetMode();

        if (bossHP <= 0)
        {
            mode = Mode.down;
            return;
        }
        grabbedDownTime += Time.deltaTime;
        if (grabbedDownTime >= grabbedDownTimeMax)
        {
            if (bossHP == 1)
            {
                BossPowerUp();
            }
            grabbedDownTime = 0.0f;
            mode = Mode.change;
        }


    }
    //すべて元に戻す
    void ResetMode()
    {
        animator.SetBool("Fire", false);
        animator.SetBool("Beam", false);
        animator.SetBool("Escape", false);
        animator.SetBool("Attacked", false);
        animator.SetBool("Walk", false);
        rotateAngle = 0.0f;
        fire = 0;
        fireAttackCount = 0;
        startFireInstance = false;
        fireInstance = false;
        laserStart = false;
        audioSource.Stop();
        changeTimer = 0.0f;
        fireTimer = 0.0f;
        laserTimer = 0.0f;
        horizontalAngle = laserMaxAngle;
        lr.positionCount = 0;
        lr.positionCount = 2;
        if (laserSpeed < 0)
        {
            laserSpeed *= -1;
        }
        for (int i = 0; i < fireBalls.Count; i++)
        {
            Destroy(fireBalls[i]);
        }
        fireBalls.Clear();
    }
    //ランダム移動
    void BossMoveRandom()
    {
        animator.SetBool("Walk", true);
        randomMoveTimer += Time.deltaTime;
        if (randomMoveTimer > randomMoveTimerMax)
        {
            randomMoveTimer = 0.0f;
            randomMoveCount++;
            float angleDiff = (360.0f / 8.0f);

            for (int i = 0; i < 8; i++)
            {
                RaycastHit hit;
                Vector3 rayPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                float angle = (90.0f - ((angleDiff * i + transform.localEulerAngles.y) + angleDiff / 2.0f)) * Mathf.Deg2Rad;
                rayPos.x += 10f * Mathf.Cos(angle);
                //rayPos.y -= 3.0f;
                rayPos.z += 10f * Mathf.Sin(angle);
                Vector3 overhead = new Vector3(this.transform.position.x, this.transform.position.y + 10.0f, this.transform.position.z);
                Vector3 heading = (rayPos - overhead).normalized;
                if (Physics.Raycast(overhead, heading * 15.0f, out hit))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("UpStages"))
                    {
                        sourcePos.Add(new Vector3(hit.point.x, hit.point.y, hit.point.z));
                    }

                }

                Debug.DrawRay(overhead, heading * 15, Color.red, 5, false);
            }
            if (sourcePos.Count > 0)
            {
                int pos = Random.Range(0, sourcePos.Count - 1);
                agent.SetDestination(sourcePos[pos]);
            }
            else
            {
                float posX = Random.Range(-randomMoveRange, randomMoveRange);
                float posZ = Random.Range(-randomMoveRange, randomMoveRange);
                Vector3 pos = this.transform.position;
                pos.x += posX;
                pos.z += posZ;

                agent.SetDestination(pos);

            }
            sourcePos.Clear();
        }
        if (randomMoveCount >= 3)
        {
            randomMoveCount = 0;
            randomMoveTimer = 0.0f;
            mode = Mode.definePosMove;
        }
        bool approachPlayer = InArea(chaseDis);
        if (approachPlayer)
        {
            randomMoveCount = 0;
            randomMoveTimer = 0.0f;
            mode = Mode.chase;
        }
    }
    //追いかける
    void BossMoveChase()
    {
        animator.SetBool("Walk", true);
        Vector3 bossMovePos = lookAhead();
        agent.SetDestination(bossMovePos);
        bool approachPlayer = InArea(chaseDis);
        if (approachPlayer == false)
        {
            mode = Mode.randomMove;
        }
        bool hitPlayer = InArea(hitPlayerDis);
        if (hitPlayer)
        {
            int modeChange;
            modeChange = Random.Range(0, 2);
            if (modeChange == 0)
            {
                animator.SetBool("Walk", false);
                mode = Mode.fire;
            }
            else if (modeChange == 1)
            {
                animator.SetBool("Walk", false);
                mode = Mode.beam;
            }
            if (GetComponent<NavMeshAgent>().isActiveAndEnabled == true)
            {
                GetComponent<NavMeshAgent>().isStopped = true;
            }
        }
    }
    //決まった場所に移動
    void BossMoveSetPosition()
    {
        animator.SetBool("Walk", true);
        Vector3 pos = mig_Point[point].position;
        if (Vector3.Distance(agent.transform.position, pos) < 2.0f)
        {
            point = (point < mig_Point.Length - 1) ? point + 1 : 0;
            mode = Mode.randomMove;

        }
        agent.SetDestination(pos);
        bool approachPlayer = InArea(chaseDis);
        if (approachPlayer)
        {
            randomMoveTimer = 0.0f;
            mode = Mode.chase;
        }
    }
    //先読みで向かう場所
    Vector3 lookAhead()
    {
        Vector3 Vr, Sr;
        float Tc;
        Vr = playerAmountOfMovement.normalized - bossAmountOfMovement.normalized;
        Sr = player.transform.position - this.transform.position;
        Tc = Mathf.Abs(Mathf.Sqrt(Sr.x * Sr.x + Sr.y * Sr.y + Sr.z * Sr.z)) / Mathf.Abs(Mathf.Sqrt(Vr.x * Vr.x + Vr.y * Vr.y + Vr.z * Vr.z));
        return player.transform.position + playerAmountOfMovement.normalized * Tc;
    }
    //移動量計算
    void CalculateAmountOfMovement(ref Vector3 amountOfMovement, ref Vector3 beforePos, Vector3 moveObj)
    {

        amountOfMovement = moveObj - beforePos;
        beforePos = player.transform.position;
    }

}
