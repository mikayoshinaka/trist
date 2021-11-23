using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BossEnemy : MonoBehaviour
{
    [SerializeField] float hitPlayerDis=3.0f;

    [SerializeField] float changeTimerMax = 1.0f;
    public float changeTimer = 0.0f;

    [SerializeField] float radius = 10;
    [SerializeField] GameObject fireBall;
    [SerializeField] GameObject player;
    [SerializeField] int fireBallCount = 8;
    [SerializeField] float y;
    [SerializeField] float fireSpeed=40.0f;
    [SerializeField] float fireRotateSpeed=60.0f;
    private float rotateAngle;
    [SerializeField] float fireTimerMax = 1.0f;
    public float fireTimer=0.0f;
    public List<GameObject> fireBalls = new List<GameObject>();
    int fireAttackCount;
    int fire;
    bool startFireInstance;
    bool fireInstance;
    Vector3 instantPlayerPos;

    [SerializeField] GameObject laser;
    private LineRenderer lr;
    [SerializeField] float laserTimerMax = 2.0f;
    public float laserTimer = 0.0f;
    [SerializeField] float laserSpeed=2.0f;
    [SerializeField] float laserMaxAngle = 30.0f;
    public enum Mode
    {
       fire,
       beam,
       earthquake,
       change
    }
    public Mode mode;
    // Start is called before the first frame update
    void Start()
    {
        mode = Mode.change;
        rotateAngle = 0.0f;
        fire = 0;
        fireAttackCount = 0;
        startFireInstance = false;
        fireInstance = false;

        lr = laser.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
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
        }
    }

    void FireShoot()
    {
        transform.LookAt(new Vector3(player.transform.position.x,this.transform.position.y,player.transform.position.z));
        if (startFireInstance == false)
        {
            startFireInstance = true;
            StartCoroutine("GenerateFire");
        }
        if(fireInstance&&fireAttackCount<fireBalls.Count/2)
        {
            fireTimer += Time.deltaTime;
            FireSet();
            if (fireTimer>fireTimerMax)
            {
                fireTimer = 0.0f;
                fireAttackCount += 1;
                rotateAngle = 0.0f;
                instantPlayerPos = player.transform.position;
                StartCoroutine(AttackingFire(instantPlayerPos, fireBalls[fireAttackCount - 1], fireBalls[fireBalls.Count - fireAttackCount]));
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
        for (int i = fire; i < fire+2; i++)
        {
            if(fire >= fireBallCount)
            {
                break;
            }
            Vector3 firePos = new Vector3(transform.position.x, transform.position.y+y,transform.position.z);
            float angle = (90.0f - ((angleDiff * i+transform.localEulerAngles.y) +angleDiff/2.0f)) * Mathf.Deg2Rad;
            firePos.x += radius * Mathf.Cos(angle);
            firePos.z += radius * Mathf.Sin(angle);
            fireBalls.Add(Instantiate(fireBall, firePos, Quaternion.identity));
            fireBalls[i].transform.parent = this.gameObject.transform;
        }
    }

    IEnumerator GenerateFire()
    {
        yield return new WaitForSeconds(0.5f);
        InstanceFire(fire);
        fire += 2;
        if (fire < fireBallCount)
        {
            StartCoroutine("GenerateFire");
        }
        else
        {
            fire = 0;
            fireInstance = true;
            fireAttackCount += 1;
            instantPlayerPos = player.transform.position;
            StartCoroutine(AttackingFire(instantPlayerPos, fireBalls[fireAttackCount - 1], fireBalls[fireBalls.Count - fireAttackCount]));
        }

    }
    void FireAttack(Vector3 playerPos, GameObject fireBall1, GameObject fireBall2)
    {
        fireBall1.tag = "FireBall";
        fireBall2.tag = "FireBall";
        fireBall1.transform.position = Vector3.MoveTowards(fireBall1.transform.position, playerPos, Time.deltaTime * fireSpeed);
        fireBall2.transform.position = Vector3.MoveTowards(fireBall2.transform.position, playerPos, Time.deltaTime * fireSpeed);
        float dis1 = Vector3.Distance(fireBall1.transform.position, playerPos);
        float dis2 = Vector3.Distance(fireBall2.transform.position, playerPos);
        if (dis1<1.0f)
        {
            fireBall1.SetActive(false);

        }
        if(dis2<1.0f)
        {
            fireBall2.SetActive(false);
        }
    }
    IEnumerator AttackingFire(Vector3 playerPos,GameObject fireBall1, GameObject fireBall2)
    {
        while(fireBall1.activeSelf==true||fireBall2.activeSelf==true)
        {
            FireAttack(playerPos,fireBall1,fireBall2);
            yield return null;
        }
        while(mode==Mode.fire)
        {
            yield return null;
        }
        Destroy(fireBall1);
        Destroy(fireBall2);
    }
    void FireSet()
    {
        float angleDiff = 360.0f / (float)fireBallCount;
        rotateAngle += Time.deltaTime;
        if(rotateAngle<angleDiff)
        {
            rotateAngle += fireRotateSpeed*Time.deltaTime;
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

    void LaserShoot()
    {
        transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
        laserTimer += Time.deltaTime;
        LaserMigration();
        if (laserTimer>laserTimerMax)
        {
            lr.positionCount = 0;
            lr.positionCount = 2;
            laserTimer = 0.0f;
            mode = Mode.change;
        }
    }

    void LaserMigration()
    {
        lr.SetPosition(0, laser.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(laser.transform.position, laser.transform.forward, out hit))
        {
            if(hit.collider.tag=="PlayerBody")
            {
                hit.collider.GetComponent<ParalysisPlayer>().paralysis = true;
            }
            lr.SetPosition(1, hit.point);
        }
        else
        {
            lr.SetPosition(1, laser.transform.forward * 5000);
        }
    }

    void ModeChange()
    {
        bool hitPlayer= InArea(); ;
        if (changeTimer <= changeTimerMax) {
            changeTimer += Time.deltaTime;
        }
        else if (changeTimer > changeTimerMax) {
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
                
                   GetComponent<NavMeshAgent>().isStopped = false;
                   GetComponent<EnemyBehaviour>().enabled = true;
                
            }
        }
    }

    bool InArea()
    {
        float dis = Vector3.Distance(player.transform.position,this.transform.position);
        if (dis<hitPlayerDis) {
                return true;  
        }
        else {
            return false;
        }
    }
}
