using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DollSave : MonoBehaviour
{
    public Animator playerAnimator;
    [SerializeField] private GameObject catchArea;
    public List<GameObject> dolls = new List<GameObject>();
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject playerController;
    [SerializeField] private Transform clearSaveBoxPos;
    [SerializeField] private GameObject clearCamera;
    public GameObject clearCameraEndPos;
    public Text text;
    bool within;
    public bool bossIn;
    private float timer;
    [SerializeField] private float clearAnimationTime=2.0f;
    [SerializeField] private float clearMoveOnlyTime = 2.0f;
    private Animator anim;
    public Image clearFadeImage;
    public Image failedFadeImage;
    [SerializeField] float fadeSpeed = 1.0f;
    [SerializeField] float clearFadeCameraRotateSpeed = 2.0f;
    [SerializeField] float clearFadeCameraMoveSpeed = 1.0f;
    float failedRed, failedGreen, failedBlue, failedAlfa;
    float clearRed, clearGreen, clearBlue, clearAlfa;
    public bool isFadeOut = false;

    AudioSource audioSource;
    public AudioClip boxSlowSE;
    public GameObject boxSound;
    public bool slow;
    bool startClear;
    // Start is called before the first frame update
    void Start()
    {
        bossIn = false;
        within = false;
        //catchPoint = 0;
        timer = 0.0f;
        anim = GameObject.Find("ShootBox").GetComponent<Animator>();

        failedRed = failedFadeImage.color.r;
        failedGreen = failedFadeImage.color.g;
        failedBlue = failedFadeImage.color.b;
        failedAlfa = failedFadeImage.color.a;
        clearRed = clearFadeImage.color.r;
        clearGreen = clearFadeImage.color.g;
        clearBlue = clearFadeImage.color.b;
        clearAlfa = clearFadeImage.color.a;

        audioSource = boxSound.GetComponent<AudioSource>();
        slow = false;
        startClear = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton1)) && within == true && catchArea.GetComponent<GhostCatch>().mode == GhostCatch.Mode.Carry)
        {
            catchArea.GetComponent<GhostCatch>().mode = GhostCatch.Mode.Shoot;
            GameObject.Find("Enemies").GetComponent<EnemiesManager>().enemyMode = EnemiesManager.EnemyMode.Mode_Defensive;
        }
        if (bossIn && !(GameObject.Find("CatchArea").GetComponent<GhostCatch>().mode == GhostCatch.Mode.Shoot))
        {
        　　if(startClear==false)
            {
                startClear = true;
                playerController.transform.LookAt(new Vector3 (clearCamera.transform.position.x,playerController.transform.position.y ,clearCamera.transform.position.z));
                if (playerAnimator.GetBool("Moving")==true) {
                    playerAnimator.SetBool("Moving", false);
                }
                playerAnimator.SetBool("Clear", true);
            }
            timer += Time.deltaTime;
            if(timer>=clearAnimationTime)
            {
                if (playerAnimator.GetBool("Clear")==true) {
                    playerAnimator.SetBool("Clear", false);
                }
                Vector3 vector3 = playerController.transform.position - clearCamera.transform.position;
                Quaternion q = Quaternion.LookRotation(vector3);
                clearCamera.transform.rotation = Quaternion.Slerp(clearCamera.transform.rotation, q, Time.deltaTime*clearFadeCameraRotateSpeed);
                clearCamera.transform.position = Vector3.MoveTowards(clearCamera.transform.position, clearCameraEndPos.transform.position, clearFadeCameraMoveSpeed*Time.deltaTime);
                if(timer >= clearAnimationTime+clearMoveOnlyTime)
                {
                    WhiteOut();
                }
            }
            
        }
        else if (bossIn == false && GameObject.Find("Timer").GetComponent<GameTimer>().countdown <= 0)
        {
            BlackOut();
        }
    }
    //暗くする
    void BlackOut()
    {
        failedFadeImage.enabled = true;
        failedAlfa += Time.deltaTime * fadeSpeed;
        SetAlpha(failedFadeImage,failedRed,failedGreen,failedBlue,failedAlfa);
        if (failedAlfa >= 1)
        {
                SceneManager.LoadScene("GameOver");
        }

    }
    //明るくする
    void WhiteOut()
    {
        clearFadeImage.enabled = true;
        clearAlfa += Time.deltaTime * fadeSpeed;
        SetAlpha(clearFadeImage, clearRed, clearGreen, clearBlue, clearAlfa);
        if (clearAlfa >= 1)
        {  
                PlayerPrefs.SetInt("dollCount", dolls.Count);
                SceneManager.LoadScene("Result");
        }

    }
    void SetAlpha(Image fadeImage,float red, float green, float blue, float alfa)
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }
    //箱に人形追加
    public void DollAdd(GameObject doll, int count)
    {
        dolls.Add(doll);
        //catchPoint += count * 100 + (count - 1) * 60;
        //text.text = catchPoint + "Point";
        anim.SetBool("open", false);
    }
    public void AnimStart()
    {
        anim.SetBool("open", true);
    }
    public void SlowSound()
    {
        if (slow==false) {
            audioSource.PlayOneShot(boxSlowSE);
            slow = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBody")
        {
            within = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerBody")
        {
            within = false;
        }
    }
   
}
