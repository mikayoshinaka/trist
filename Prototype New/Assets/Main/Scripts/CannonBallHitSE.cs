using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallHitSE : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip BallHitSE;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }
    public void HitBall()
    {
        audioSource.PlayOneShot(BallHitSE);
    }
}
