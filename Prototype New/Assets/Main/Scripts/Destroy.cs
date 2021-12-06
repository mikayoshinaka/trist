using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class Destroy : MonoBehaviour
{
    VisualEffect effect;
    float destroyTime = 4.0f;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        effect = this.gameObject.GetComponent<VisualEffect>();
        effect.SendEvent("OnPlay");
    }

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
