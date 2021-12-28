using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEar : MonoBehaviour
{
    [SerializeField] private GameObject playerController;
    [SerializeField] private GameObject leftEar;
    [SerializeField] private GameObject rightEar;
    [SerializeField] private GameObject leftEarjoint;
    [SerializeField] private GameObject rightEarjoint;
    [SerializeField] private GameObject leftEarArea;
    [SerializeField] private GameObject rightEarArea;
    [SerializeField] private GameObject flyleftEar;
    [SerializeField] private GameObject flyrightEar;
    Vector3 leftEarjointPos;
    Vector3 rightEarjointPos;
    bool pull;
    int earNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        pull = false;
        StartEarPos();
    }

    // Update is called once per frame
    void Update()
    {
        if(pull==true)
        {
            PullEar();
        }
    }
    //������������
    public void PullEar()
    {
        if(earNum==1)
        {
            leftEarjoint.transform.position = playerController.transform.position;
        }
        else if(earNum==2)
        {
            rightEarjoint.transform.position = playerController.transform.position;
        }
    }
    //�������鎨��I��
    public void Selectear(GameObject ear)
    {
        if(ear.tag == "BossEarLeft")
        {
            earNum = 1;
        }
        else if(ear.tag == "BossEarRight")
        {
            earNum = 2;
        }
        pull = true;
    }
    //����߂�
    public void UndoEar(GameObject ear)
    {
        if (ear.tag == "BossEarLeft")
        {
            leftEarjoint.transform.position = this.transform.TransformPoint(leftEarjointPos);
        }
        else if (ear.tag == "BossEarRight")
        {
            rightEarjoint.transform.position = this.transform.TransformPoint(rightEarjointPos);
        }
        pull = false;

    }
    //����؂藣��
    public void DetachEar()
    {
        if(earNum == 1)
        {
            leftEar.SetActive(false);
            leftEarArea.SetActive(false);
            flyleftEar.SetActive(true);
            flyleftEar.transform.parent = null;
            flyleftEar.GetComponent<Rigidbody>().isKinematic = false;
        }
        else if(earNum == 2)
        {
            rightEar.SetActive(false);
            rightEarArea.SetActive(false);
            flyrightEar.SetActive(true);
            flyrightEar.transform.parent = null;
            flyrightEar.GetComponent<Rigidbody>().isKinematic = false;

        }
        if(leftEar.activeSelf==false&&rightEar.activeSelf==false)
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
    }
    //���̍ŏ��̏ꏊ
    void StartEarPos()
    {
        leftEarjointPos = this.transform.InverseTransformPoint(leftEarjoint.transform.position);
        rightEarjointPos = this.transform.InverseTransformPoint(rightEarjoint.transform.position);
    }

   
}
