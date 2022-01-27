using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
    public Image fadeImage;
    [SerializeField] float fadeSpeed = 1.0f;
    float red, green, blue, alfa;
    bool isFadeOut = false;
    private GameObject BGM;

    [SerializeField] GameObject[] enemy;
    [SerializeField] GameObject player;
    [SerializeField] GameObject firstPlayerPos;
    [SerializeField] GameObject secondPlayerPos;
    [SerializeField] GameObject[] enemyApproachPos;
    [SerializeField] GameObject[] enemyAttackPos;
    [SerializeField] GameObject sweat;
    [SerializeField] GameObject floor1;
    [SerializeField] GameObject floor2;
    [SerializeField] GameObject endUI;
    //敵
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sidleSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float sweatTime;
    [SerializeField] private float lookTime;
    [SerializeField] private float chaseTime;
    [SerializeField] private Animator[] enemyAnimator;
    //プレイヤー
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float playerRotateSpeed;
    [SerializeField] private float playerFidgetRotateSpeed;
    [SerializeField] private Animator playerAnimator;
    private float timer;
    private int rnd = 0;
    public enum Mode
    {
        approach,
        sweat,
        attack,
        look,
        chase,
        end
    }
    public Mode mode;
    // Start is called before the first frame update
    void Start()
    {
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;
        BGM = GameObject.Find("BGM").transform.gameObject;
        BGM.GetComponent<BGM>().GameOverDramaBGM();
        BGM.GetComponent<AudioSource>().loop = false;
        mode = Mode.approach;
        if (PlayerPrefs.GetInt("SceneNumber") == 1)
        {
            floor1.SetActive(true);
            floor2.SetActive(false);
        }
        else if(PlayerPrefs.GetInt("SceneNumber") == 2)
        {
            floor2.SetActive(true);
            floor1.SetActive(false);
        }
        for (int i=0;i<enemyAnimator.Length;i++) {
            if (!enemyAnimator[i].GetBool("Running"))
            {
                enemyAnimator[i].SetBool("Running", true);
            }
        }
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (mode)
        {
            case Mode.approach:
                ApproachScene();
                break;
            case Mode.sweat:
                SweatScene();
                break;
            case Mode.attack:
                AttackScene();
                break;
            case Mode.look:
                LookScene();
                break;
            case Mode.chase:
                ChaseScene();
                break;
            case Mode.end:
                EndScene();
                break;
        }

    }

    void ApproachScene()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            Vector3 vector3 = firstPlayerPos.transform.position - enemy[i].transform.position;
            Quaternion q = Quaternion.LookRotation(vector3);
            enemy[i].transform.rotation = Quaternion.Slerp(enemy[i].transform.rotation, q, Time.deltaTime * rotateSpeed);
            enemy[i].transform.position = Vector3.MoveTowards(enemy[i].transform.position, enemyApproachPos[i].transform.position, moveSpeed * Time.deltaTime);
        }
        PlayerFidget();
        bool approached = false;
        for (int i = 0; i < enemy.Length; i++)
        {
            float dis = (enemyApproachPos[i].transform.position - enemy[i].transform.position).magnitude;
            if (dis < 0.1f)
            {
                if (enemyAnimator[i].GetBool("Running"))
                {
                    enemyAnimator[i].SetBool("Running", false);
                }
                approached = true;
            }
            else
            {
                approached = false;
                break;
            }
        }
        if (approached == true)
        {
            mode = Mode.sweat;
            sweat.SetActive(true);
        }
    }
    void SweatScene()
    {
        timer += Time.deltaTime;
        for (int i = 0; i < enemy.Length; i++)
        {
            Vector3 vector3 = firstPlayerPos.transform.position - enemy[i].transform.position;
            Quaternion q = Quaternion.LookRotation(vector3);
            enemy[i].transform.rotation = Quaternion.Slerp(enemy[i].transform.rotation, q, Time.deltaTime * rotateSpeed);
            enemy[i].transform.position = Vector3.MoveTowards(enemy[i].transform.position, firstPlayerPos.transform.position, sidleSpeed * Time.deltaTime);
        }
        PlayerFidget();
        if (timer > sweatTime)
        {
            mode = Mode.attack;
            timer = 0.0f;
            for (int i = 0; i < enemyAnimator.Length; i++)
            {
                if (!enemyAnimator[i].GetBool("Running"))
                {
                    enemyAnimator[i].SetBool("Running", true);
                }
            }
        }
    }
    void AttackScene()
    {
        for (int i = 0; i < enemy.Length; i++)
        {

            Vector3 vector3 = firstPlayerPos.transform.position - enemy[i].transform.position;
            Quaternion q = Quaternion.LookRotation(vector3);
            enemy[i].transform.rotation = Quaternion.Slerp(enemy[i].transform.rotation, q, Time.deltaTime * rotateSpeed);
            enemy[i].transform.position = Vector3.MoveTowards(enemy[i].transform.position, enemyAttackPos[i].transform.position, attackSpeed * Time.deltaTime);
        }
        PlayerRunAway();
        bool attacked = false;
        for (int i = 0; i < enemy.Length; i++)
        {
            float dis = (enemyAttackPos[i].transform.position - enemy[i].transform.position).magnitude;
            if (dis < 0.1f)
            {
                attacked = true;
                if (enemyAnimator[i].GetBool("Running"))
                {
                    enemyAnimator[i].SetBool("Running", false);
                }
            }
            else
            {
                attacked = false;
                break;
            }
        }
        if (attacked == true)
        {
            mode = Mode.look;
        }
    }
    void LookScene()
    {
        timer += Time.deltaTime;
        for (int i = 0; i < enemy.Length; i++)
        {
            Vector3 vector3 = secondPlayerPos.transform.position - enemy[i].transform.position;
            Quaternion q = Quaternion.LookRotation(vector3);
            enemy[i].transform.rotation = Quaternion.Slerp(enemy[i].transform.rotation, q, Time.deltaTime * lookSpeed);
        }
        PlayerRunAway();
        if (timer > lookTime)
        {
            for (int i = 0; i < enemyAnimator.Length; i++)
            {
                if (!enemyAnimator[i].GetBool("Running"))
                {
                    enemyAnimator[i].SetBool("Running", true);
                }
            }
            mode = Mode.chase;
            timer = 0.0f;
        }
    }
    void ChaseScene()
    {
        timer += Time.deltaTime;
        for (int i = 0; i < enemy.Length; i++)
        {
            Vector3 vector3 = secondPlayerPos.transform.position - enemy[i].transform.position;
            Quaternion q = Quaternion.LookRotation(vector3);
            enemy[i].transform.rotation = Quaternion.Slerp(enemy[i].transform.rotation, q, Time.deltaTime * rotateSpeed);
            enemy[i].transform.position = Vector3.MoveTowards(enemy[i].transform.position, secondPlayerPos.transform.position, moveSpeed * Time.deltaTime);
        }
        PlayerRunAway();
        if (timer > chaseTime)
        {
            mode = Mode.end;
            timer = 0.0f;
        }
    }
    void EndScene()
    {
        if (isFadeOut == false)
        {
            StartFadeOut();
        }
    }
    void StartFadeOut()
    {
        fadeImage.enabled = true;
        alfa += Time.deltaTime * fadeSpeed;
        SetAlpha();
        if (alfa >= 1)
        {
            BGM.GetComponent<AudioSource>().loop = true;
            BGM.GetComponent<BGM>().GameOverBGM();
            isFadeOut = true;
            endUI.SetActive(true);
        }

    }
    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }
    //逃げる
    void PlayerRunAway()
    {
        if (!playerAnimator.GetBool("Moving"))
        {
            playerAnimator.SetBool("Moving", true);
        }
        Vector3 vector3 = secondPlayerPos.transform.position - player.transform.position;
        Quaternion q = Quaternion.LookRotation(vector3);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, q, Time.deltaTime * playerRotateSpeed);
        player.transform.position = Vector3.MoveTowards(player.transform.position, secondPlayerPos.transform.position, playerMoveSpeed * Time.deltaTime);
    }
    //狼狽
    void PlayerFidget()
    {

        Vector3 vector3 = enemy[rnd].transform.position - player.transform.position;
        Quaternion q = Quaternion.LookRotation(vector3);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, q, Time.deltaTime * playerFidgetRotateSpeed);
        if (Mathf.DeltaAngle(Mathf.Round(player.transform.eulerAngles.x), enemy[rnd].transform.eulerAngles.x) <= 5 &&
           Mathf.DeltaAngle(Mathf.Round(player.transform.eulerAngles.y), enemy[rnd].transform.eulerAngles.y) <= 5 &&
           Mathf.DeltaAngle(Mathf.Round(player.transform.eulerAngles.z), enemy[rnd].transform.eulerAngles.z) <= 5)
        {
            if (rnd <= 2)
            {
                rnd += 2;
            }
            else
            {
                rnd = rnd - 3;
            }
        }
    }

}
