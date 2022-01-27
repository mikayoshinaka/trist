using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSoundScript : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip bookAttackSE;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BookOneShot()
    {
        audioSource.PlayOneShot(bookAttackSE);
    }
}
