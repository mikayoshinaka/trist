using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundScript : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip decideSE;
    public AudioClip selectSE;


    //二つ以上にならないように
    private void Awake()
    {
        int numMusicPlayers = FindObjectsOfType<MenuSoundScript>().Length;
        if(numMusicPlayers>1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        DontDestroyOnLoad(this);
    }

    public void Decide()
    {
        audioSource.PlayOneShot(decideSE);
    }
    public void Select()
    {
        audioSource.PlayOneShot(selectSE);
    }
}
