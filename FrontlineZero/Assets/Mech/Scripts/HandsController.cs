using UnityEngine;

public class HandsController : MonoBehaviour
{
    public OVRGrabbable grabbable;
    public Transform dockingTransform;
    public float returnSpeed;
    public float dockDistance;
    [HideInInspector]
    public bool isGrabbed = false;
    bool isDocked = true;
    public Animator activeArmatureAnimator;
    
    void Update()
    {
        if(!isGrabbed && !isDocked)
        {
            GoToDock();
        }
    }

    void GoToDock()
    {
        if(dockingTransform != null)
        {
            Vector3 dockVec = dockingTransform.position - transform.position;
            if (dockVec.magnitude > dockDistance)
            {
                transform.position += dockVec.normalized * returnSpeed;
            }
            else
            {
                transform.rotation = dockingTransform.rotation;
                isDocked = true;
                activeArmatureAnimator.SetBool("IsOut", true);
            }
        }        
    }

    public void Grab()
    {
        isGrabbed = true;
        isDocked = false;
        activeArmatureAnimator.SetBool("IsOut", false);
    }

    public void Release()
    {
        isGrabbed = false;
    }
}
