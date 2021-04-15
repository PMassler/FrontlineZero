using UnityEngine;


public class MechGrabber : Weapon
{
    bool isTryingGrab;
    bool isGrabbing;
    public int dirtGrabParticelIndex;
    public int dirtGrabSFXIndex;
    public float grabRange;
    public LayerMask terrainMask;

    public GameObject matterBallPrefab;

    public ExplodingObject exObj;

    MechGrabbable currentGrabbedObject;
    FixedJoint cFixedJoint;

    public HandRB handRBScript;

    public int grabSFXIndex;
    public int unGrabSFXIndex;

    private void Update()
    {
        if(!isTryingGrab && triggerValue > 0.5f)
        {
            isTryingGrab = true;
            animator.SetBool("isGrabbing", true);
            TryGrab();
        }

        if(isTryingGrab && triggerValue<= 0.5f)
        {
            isTryingGrab = false;
            animator.SetBool("isGrabbing", false);
            TryGrabStop();
        }
    }

    // check if there is an object or terrain to be grabbed
    void TryGrab()
    {
        SFXManager.Instance.PlaySFX(grabSFXIndex, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange))
        {
            MechGrabbable objToGrab = hit.transform.GetComponentInParent<MechGrabbable>();
           
            if (objToGrab != null)
            {
                GrabBegin(objToGrab);
                isGrabbing = true;
            }
            else if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange, terrainMask))
            {
                exObj.Explode(hit.point);
                MechGrabbable matterObj = Instantiate(matterBallPrefab, transform.position + transform.forward * hit.distance, Quaternion.identity).GetComponent<MechGrabbable>();
                ParticleManager.Instance.PlayParticleSystem(dirtGrabParticelIndex, transform.position + transform.forward * hit.distance, -transform.forward);
                SFXManager.Instance.PlaySFX(dirtGrabSFXIndex, transform.position + transform.forward * hit.distance);
                GrabBegin(matterObj);
                isGrabbing = true;
            }

        }
        else if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange, terrainMask))
        {         
            exObj.Explode(hit.point);
            MechGrabbable matterRB = Instantiate(matterBallPrefab, transform.position + transform.forward * hit.distance, Quaternion.identity).GetComponent<MechGrabbable>();
            ParticleManager.Instance.PlayParticleSystem(dirtGrabParticelIndex, transform.position + transform.forward * hit.distance, -transform.forward);
            SFXManager.Instance.PlaySFX(dirtGrabSFXIndex, transform.position + transform.forward * hit.distance);
            GrabBegin(matterRB);
            isGrabbing = true;

        }
        
    }

    // if something is grabbed, release
    void TryGrabStop()
    {
        SFXManager.Instance.PlaySFX(unGrabSFXIndex, transform.position);
        if (isGrabbing)
        {            
            isGrabbing = false;
            GrabRelease();
        }
    }


    // conntect grabbed object with fixedJoint to hand rigidbody
    void GrabBegin(MechGrabbable grabbedObj)
    {
        mechHandScript.ShootVibrate(1f,1f,0.1f);
        currentGrabbedObject = grabbedObj;
        
        cFixedJoint = handRB.gameObject.AddComponent<FixedJoint>();
        cFixedJoint.connectedBody = grabbedObj.rb;
    }
    
    // destroy fixedJoint
    void GrabRelease()
    {
        mechHandScript.ShootVibrate(1f, 1f, 0.1f);
        Destroy(cFixedJoint);
        cFixedJoint = null;
        if(currentGrabbedObject != null)
        {
            currentGrabbedObject.rb.angularVelocity = handRBScript.GetAngularVelocity();
            currentGrabbedObject = null;
        }
    }	
}

