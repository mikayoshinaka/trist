using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGSEOneShot : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip SGSE;
    public bool canSE;
    // Start is called before the first frame update
    void Start()
    {
        canSE = false;
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }
    void Update()
    {
        if (canSE)
        {
            audioSource.PlayOneShot(SGSE);
            canSE = false;
        }
    }
}
