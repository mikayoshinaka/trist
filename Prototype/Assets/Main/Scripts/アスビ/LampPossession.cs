using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampPossession : MonoBehaviour
{
    GameObject lampLight;
    GameObject lampCamera;
    public GhostChange ghost;
    public bool posessing;

    private void Start()
    {
        lampLight = transform.Find("LampLight").gameObject;
        lampCamera = transform.Find("Camera").gameObject;
        ghost = GameObject.Find("Ghost").GetComponent<GhostChange>();
        posessing = false;
    }

    private void Update()
    {
        PossessionTransition();
    }

    void PossessionTransition()
    {
        // ENTER
        if (!posessing && ghost.possessObject != null && ghost.possess && !ghost.canPossess)
        {
            if (ghost.possessObject.name == this.gameObject.name)
            {
                posessing = true;
                lampLight.transform.SetParent(lampCamera.transform);
            }
        }

        // LEAVE
        if (posessing && ghost.possessObject != null && ghost.leave)
        {
            if (ghost.possessObject.name == this.gameObject.name)
            {
                posessing = false;
                lampLight.transform.SetParent(this.transform);
            }
        }
    }
}
