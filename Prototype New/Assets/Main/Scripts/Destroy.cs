using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class Destroy : MonoBehaviour
{
    VisualEffect effect;
    float destroyTime = 4.0f;
    float timer;
    AudioSource audioSource;
    public AudioClip hitFireSE;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        effect = this.gameObject.GetComponent<VisualEffect>();
        effect.SendEvent("OnPlay");
        audioSource = this.gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(hitFireSE);
    }
    //火球の爆発を始め、爆発を消す
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>destroyTime)
        {
            effect.SendEvent("StopPlay");
            Destroy(this.gameObject);
        }
    }
}
