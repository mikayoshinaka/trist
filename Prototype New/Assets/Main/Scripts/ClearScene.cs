using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearScene : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform deskTransform;
    [SerializeField] private Transform bookTransform;
    [SerializeField] private Transform chairTransform;
    [SerializeField] private Transform enemiesTransform;
    public List<GameObject> desk = new List<GameObject>();
    public List<GameObject> book = new List<GameObject>();
    public List<GameObject> chair = new List<GameObject>();
    public List<Vector3> startDeskPos = new List<Vector3>();
    public List<Vector3> startBookPos = new List<Vector3>();
    public List<Vector3> startChairPos = new List<Vector3>();
    public List<Vector3> startDeskForward = new List<Vector3>();
    public List<Vector3> startBookForward = new List<Vector3>();
    public List<Vector3> startChairForward = new List<Vector3>();
    public List<GameObject> enemy = new List<GameObject>();
    public List<GameObject> flewBook = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        CountFurniture();
        CountEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CountFurniture()
    {
        for (int i = 0;i< deskTransform.childCount;i++)
        {
            desk.Add(deskTransform.GetChild(i).gameObject);
            startDeskPos.Add(targetTransform.InverseTransformPoint(deskTransform.GetChild(i).position));
            startDeskForward.Add(targetTransform.InverseTransformDirection(deskTransform.GetChild(i).forward));
        }
        for(int i = 0; i < bookTransform.childCount; i++)
        {
            book.Add(bookTransform.GetChild(i).gameObject);
            startBookPos.Add(targetTransform.InverseTransformPoint(bookTransform.GetChild(i).position));
            startBookForward.Add(targetTransform.InverseTransformDirection(bookTransform.GetChild(i).forward));
        }
        for(int i = 0; i < chairTransform.childCount; i++)
        {
            chair.Add(chairTransform.GetChild(i).gameObject);
            startChairPos.Add(targetTransform.InverseTransformPoint(chairTransform.GetChild(i).position));
            startChairForward.Add(targetTransform.InverseTransformDirection(chairTransform.GetChild(i).forward));
        }
    }
    void CountEnemy()
    {
        for (int i = 0; i < enemiesTransform.childCount; i++)
        {
            enemy.Add(enemiesTransform.GetChild(i).gameObject);
        }
    }

    void ResetFurniturePos()
    {
        for(int i = 0; i < desk.Count; i++)
        {
            if(desk[i].activeSelf==false)
            {
                desk[i].SetActive(true);
            }
            desk[i].transform.position = targetTransform.TransformPoint(startDeskPos[i]);
            desk[i].transform.forward = targetTransform.TransformDirection(startDeskForward[i]);
        }
        for (int i = 0; i < book.Count; i++)
        {
            if (book[i].activeSelf == false)
            {
                book[i].SetActive(true);
            }
            book[i].transform.position = targetTransform.TransformPoint(startBookPos[i]);
            book[i].transform.forward = targetTransform.TransformDirection(startBookForward[i]);
        }
        for (int i = 0; i < chair.Count; i++)
        {
            if (chair[i].activeSelf == false)
            {
                chair[i].SetActive(true);
            }
            chair[i].transform.position = targetTransform.TransformPoint(startChairPos[i]);
            chair[i].transform.forward = targetTransform.TransformDirection(startChairForward[i]);
        }
    }
    void EnemyCannnotSee()
    {
        for (int i = 0; i < enemy.Count; i++)
        {
            if(enemy[i].activeSelf==true)
            {
                enemy[i].SetActive(false);

            }
        }
    }

    void FlewBookCannotSee()
    {
        for (int i = 0; i < flewBook.Count; i++)
        {
            if (flewBook[i].activeSelf == true)
            {
                flewBook[i].SetActive(false);

            }
        }
    }

    public void ClearScenePos()
    {
        ResetFurniturePos();
        EnemyCannnotSee();
        FlewBookCannotSee();
    }
}
