using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAttack : MonoBehaviour
{
    private float time;
    private float timeMax=3.0f;
    [SerializeField] private BookShelf bookShelfCamera;
    private GameObject[] ChildObj;
    bool attack;
   // Start is called before the first frame update
   void Start()
    {
        attack = false;
        time = 0.0f;
        ChildObj = new GameObject[this.gameObject.transform.childCount];
        for(int i=0;i< this.gameObject.transform.childCount;i++)
        {
            ChildObj[i] = this.gameObject.transform.GetChild(i).gameObject;
        }
    }

    //攻撃できる本に変える
    // Update is called once per frame
    void Update()
    {
        if(bookShelfCamera.fly==true&& time < timeMax)
        {
            time += Time.deltaTime;
        }
        else if(time>=timeMax&& attack==false)
        {
            attack = true;
            for (int i = 0; i < ChildObj.Length; i++)
            {
                if (ChildObj[i].tag == "AttackBook") {
                    ChildObj[i].tag = "Book";
                }
            }
        }
    }
}
