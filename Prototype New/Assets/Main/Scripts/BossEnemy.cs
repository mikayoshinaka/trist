using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class BossEnemy : MonoBehaviour
{
    [SerializeField] private int bossHPMax = 3;
    public int bossHP;
    public bool hpDown;
    private float bossSize = 1.0f;
    [SerializeField] float hitPlayerDis = 3.0f;

    [SerializeField] float changeTimerMax = 2.0f;
    public float changeTimer = 0.0f;

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

    [SerializeField] GameObject laser;
    private LineRenderer lr;
    [SerializeField] float laserTimerMax = 4.0f;
    public float laserTimer = 0.0f;
    [SerializeField] float laserSpeed = 30.0f;
    [SerializeField] float laserMaxAngle = 30.0f;
    private float horizontalAngle = 0.0f;
    private float horizontalAngleLimit = 45.0f;
    bool laserStart;
    public Vector3 sourcePos;
    Vector3 targetDir;
    public NavMeshAgent agent;

    public bool reSet;
    private float grabbedDownTime = 0.0f;
    private float grabbedDownTimeMax = 3.0f;
    public enum Mode
    {
        fire,
        beam,
        earthquake,
        change,
        down,
        grabbed
    }
    public enum Migration
    {
        chase,
        ambush
    }
    public Mode mode;
    public Migration migration;
    // Start is called before the first frame update
    void Start()
    {
        reSet = false;
        bossHP = bossHPMax;
        hpDown = false;

        mode = Mode.change;
        rotateAngle = 0.0f;
        fire = 0;
        fireAttackCount = 0;
        startFireInstance = false;
        fireInstance = false;

        lr = laser.GetComponent<LineRenderer>();
        laserStart = false;
        targetDir = Vector3.Slerp(transform.forward, transform.right, 0.5f);
        agent = GetComponent<NavMeshAgent>();
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
        }
        LookFor();
    }
    //サイズ変更
    void SizeDown()
    {
        if (bossSize > 0.0f)
        {
            bossSize -= Time.deltaTime * 0.2f;
            this.transform.GetChild(0).localScale = new Vector3((float)(bossHP + 1 / bossHPMax + 1) + ((float)(1 / bossHPMax + 1) * bossSize), (float)(bossHP + 1 / bossHPMax + 1) + ((float)(1 / bossHPMax + 1) * bossSize), (float)(bossHP + 1 / bossHPMax + 1) + ((float)(1 / bossHPMax + 1) * bossSize));
        }
        else if (bossSize <= 0.0f)
        {
            this.transform.GetChild(0).localScale = new Vector3((float)(bossHP + 1 / bossHPMax + 1), (float)(bossHP + 1 / bossHPMax + 1), (float)(bossHP + 1 / bossHPMax + 1));
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
            FireSet();
            if (fireTimer > fireTimerMax)
            {
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
            mode = Mode.change;
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

    IEnumerator GenerateFire()
    {
        yield return new WaitForSeconds(0.5f);
        if (mode == Mode.fire)
        {
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
                fireAttackCount += 1;
                instantPlayerPos = player.transform.position;
                Vector3 terminusDirection1 = FireTerminus(fireBalls[fireAttackCount - 1].transform.position, new Vector3(instantPlayerPos.x, instantPlayerPos.y + ballAjust, instantPlayerPos.z));
                Vector3 terminusDirection2 = FireTerminus(fireBalls[fireBalls.Count - fireAttackCount].transform.position, new Vector3(instantPlayerPos.x, instantPlayerPos.y + ballAjust, instantPlayerPos.z));
                StartCoroutine(AttackingFire(instantPlayerPos, fireBalls[fireAttackCount - 1], fireBalls[fireBalls.Count - fireAttackCount], terminusDirection1, terminusDirection2));
            }
        }
        else
        {
            yield break;
        }

    }
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
    Vector3 FireTerminus(Vector3 startPos, Vector3 endPos)
    {
        Vector3 heading = endPos - startPos;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        return direction;
    }
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
        if (laserStart==false) {
            laserStart = true;
            transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
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
            horizontalAngle = 0.0f;
            if (laserSpeed < 0)
            {
                laserSpeed *= -1;
            }
        }
    }

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
                lr.SetPosition(1, laser.transform.forward * 500);
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
    void LaserRotation()
    {
        horizontalAngle += Time.deltaTime * laserSpeed;
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
        bool hitPlayer = InArea();
        if (changeTimer <= changeTimerMax)
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
                    GetComponent<EnemyBehaviour>().enabled = false;
                }
                changeTimer = 0.0f;
            }
            else
            {
                changeTimer = 0.0f;
                GetComponent<NavMeshAgent>().isStopped = false;
                GetComponent<EnemyBehaviour>().enabled = true;

            }
        }
    }
    //エリア内にプレイヤーがいるかどうか
    bool InArea()
    {
        float dis = Vector3.Distance(player.transform.position, this.transform.position);
        if (dis < hitPlayerDis)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void BossDown()
    {

    }
    //掴まれた
    void BossGrabbed()
    {
        if (reSet == true)
        {
            ResetMode();
        }
        else
        {
            if (bossHP <= 0)
            {
                mode = Mode.down;
                return;
            }
            ResetMode();
            grabbedDownTime += Time.deltaTime;
            if (grabbedDownTime >= grabbedDownTimeMax)
            {
                grabbedDownTime = 0.0f;
                mode = Mode.change;
            }
        }

    }
    void ResetMode()
    {
        rotateAngle = 0.0f;
        fire = 0;
        fireAttackCount = 0;
        startFireInstance = false;
        fireInstance = false;
        laserStart = false;

        changeTimer = 0.0f;
        fireTimer = 0.0f;
        laserTimer = 0.0f;
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
    //boss移動用　プレイヤーの方向とy座標は分かっている 試し　作成中
    void LookFor()
    {
        float angleDiff = (360.0f / 8.0f);
        if (Input.GetKeyDown(KeyCode.E))
        {
            for (int i = 0; i < 8; i++)
            {
                NavMeshHit hit;
                Vector3 rayPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                float angle = (90.0f - ((angleDiff * i + transform.localEulerAngles.y) + angleDiff / 2.0f)) * Mathf.Deg2Rad;
                rayPos.x += 10f * Mathf.Cos(angle);
                //rayPos.y -= 3.0f;
                rayPos.z += 10f * Mathf.Sin(angle);
                Vector3 overhead = new Vector3(this.transform.position.x, this.transform.position.y + 10.0f, this.transform.position.z);
                Vector3 heading = (rayPos - overhead).normalized;
                if (NavMesh.Raycast(overhead, heading/**3.0f*/, out hit, NavMesh.AllAreas))
                {
                    sourcePos = hit.position;
                    Instantiate(fireBall, sourcePos, fireBall.transform.rotation);
                }

                Debug.DrawRay(overhead, heading * 3, Color.red, 5, false);
            }
        }
    }
}
