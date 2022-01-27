using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSEOneShot : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip VFXSE;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(VFXSE);
    }
}
