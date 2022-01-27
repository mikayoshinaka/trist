using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCameraCanLookBox : MonoBehaviour
{
    GhostCatch ghostCatch;
    public List<GameObject> furniture = new List<GameObject>();
    [SerializeField] DollSave dollSave;
    // Start is called before the first frame update
    void Start()
    {
        ghostCatch = GameObject.Find("CatchArea").GetComponent<GhostCatch>();
    }

    //人形を入れる際カメラに邪魔になる壁や物を消し、終わったら元に戻す
    // Update is called once per frame
    void Update()
    {
        if (ghostCatch.mode == GhostCatch.Mode.Shoot)
        {
            for (int i = 0; i < furniture.Count; i++)
            {
                if (furniture[i].activeSelf == true)
                {
                    furniture[i].SetActive(false);
                }
            }
        }
        else if(dollSave.bossIn==true)
        {
            return;
        }
        else
        {
            if (furniture.Count > 0)
            {
                for (int i = 0; i < furniture.Count; i++)
                {
                    if (furniture[i].activeSelf == false)
                    {
                        furniture[i].SetActive(true);
                        furniture.Remove(furniture[i]);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Box" || other.tag == "Chair" || other.tag == "Desk" || other.tag == "Wall"))
        {
            Debug.Log("hit");
            if (!furniture.Contains(other.gameObject))
            {
                furniture.Add(other.gameObject);

            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if ((other.tag == "Box" || other.tag == "Chair" || other.tag == "Desk" || other.tag == "Wall"))
        {
            Debug.Log("hit");
            if (furniture.Contains(other.gameObject))
            {
                if (other.gameObject.activeSelf == false)
                {
                    other.gameObject.SetActive(true);
                }
                furniture.Remove(other.gameObject);
            }
        }

    }
}
