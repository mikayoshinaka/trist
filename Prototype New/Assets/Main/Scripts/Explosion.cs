using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class Explosion : MonoBehaviour
{
    [SerializeField] VisualEffect effect;
    public bool explode;
    // Start is called before the first frame update
    void Start()
    {
        explode = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //爆発表示　火球を消す
    private void OnCollisionEnter(Collision other)
    {
        if (explode==true) {
            Instantiate(effect, this.transform.position, Quaternion.identity);
            this.gameObject.SetActive(false);
        }
    }
}
