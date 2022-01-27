using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioSource audioSource;
    //二つ以上にならないように
    private void Awake()
    {
        int numMusicPlayers = FindObjectsOfType<BGM>().Length;
        if (numMusicPlayers > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TitleSelectBGM()
    {
        audioSource.clip = clips[0];
        audioSource.Play();
    }
    public void Stage1BGM()
    {
        audioSource.clip = clips[1];
        audioSource.Play();
    }
    public void Stage2BGM()
    {
        audioSource.clip = clips[2];
        audioSource.Play();
    }
    public void ClearResultBGM()
    {
        audioSource.clip = clips[3];
        audioSource.Play();
    }
    public void GameOverDramaBGM()
    {
        audioSource.clip = clips[4];
        audioSource.Play();
    }
    public void GameOverBGM()
    {
        audioSource.clip = clips[5];
        audioSource.Play();

    }
    public void HowTo()
    {
        audioSource.Stop();
    }
}
