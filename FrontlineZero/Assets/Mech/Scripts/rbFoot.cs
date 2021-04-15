using UnityEngine;

public class rbFoot : MonoBehaviour
{
    public Rigidbody rb;
    public MechPhysics mechScript;
    public bool isGrounded;
    public rbFoot correspondingFoot;
    public FootState footState = FootState.Idle;
    [Range(-1,1)]
    public int leftOrRight;
    public PositionController posPID;
    public float targetDistance;   
    public float customGravity;
    public float maxLiftTime;
    float liftStartTime;
    public int liftUpSFXIndex;
    public int dropDownSFXIndex;
    public int stepSFXIndex;
    bool gotGroundedLastTick = false;
    bool[] lastFramesGroundedValues = new bool[3];
    int lastFrameGroundedCounter = 0;

    public enum FootState
    {
        Lifting, Dropping, Idle, OutOfRange
    }  

    private void FixedUpdate()
    {               
        switch (footState)
        {
            case FootState.Idle:
                Idle();
                break;
            case FootState.Lifting:
                LiftUp();
                break;
            case FootState.Dropping:
                DropDown();
                break;            
        }
    }

    // trigger step
    public void Step()
    {
        SFXManager.Instance.PlaySFX(liftUpSFXIndex, transform.position);
        liftStartTime = Time.time;
        footState = FootState.Lifting;
    }

    // calculate direction of step based on the other foots position
    public Vector3 GetNextStepDirection()
    {
        return Vector3.ProjectOnPlane((correspondingFoot.transform.position + correspondingFoot.transform.right * leftOrRight * mechScript.stepWidth - transform.position),Vector3.up);
    }

    // lifts foot 
    void LiftUp()
    {
        // stops if position is reached, or stuck
        if (Time.time > liftStartTime + maxLiftTime)
        {
            rb.velocity = Vector3.zero;
            footState = FootState.Idle;
            mechScript.isStepping = false;
        }
        
        Vector3 targetPos = correspondingFoot.transform.position + correspondingFoot.transform.right * leftOrRight * mechScript.stepWidth + Vector3.up * mechScript.stepHeight;       
        if (Vector3.Distance(transform.position, targetPos) > targetDistance)
        {
            rb.AddForce(posPID.getAcceleration(transform.position,rb.velocity,targetPos),ForceMode.Acceleration);            
        }
        else
        {
            SFXManager.Instance.PlaySFX(dropDownSFXIndex, transform.position);
            rb.velocity = Vector3.zero;            
            footState = FootState.Dropping;
        }
    }

    // drops foot down, until grounded
    void DropDown()
    {
        Vector3 stepForce = Vector3.up * -mechScript.stepDownwardForce + mechScript.stepDirection.normalized * mechScript.stepForwardForce;
        if (!isGrounded)
        {
            rb.AddForce(stepForce, ForceMode.VelocityChange);
        }
        else
        {
            SFXManager.Instance.PlaySFX(stepSFXIndex, transform.position);
            rb.velocity = Vector3.zero;
            footState = FootState.Idle;
            mechScript.isStepping = false;
        }
    }

    // normal state, normal gravity
    void Idle()
    {
        rb.AddForce(Vector3.up * -customGravity, ForceMode.Acceleration);
    }

    private void Update()
    {      
        // detection if grounded over multiple frames for higher security
        int counter = 0;
        for (int i = 0; i < lastFramesGroundedValues.Length; i++)
        {
            if (lastFramesGroundedValues[i])
            {
                counter++;
            }
        }

        if (counter >= 1)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        lastFramesGroundedValues[lastFrameGroundedCounter] = true;
        gotGroundedLastTick = true;
    }

    private void LateUpdate()
    {
        if (!gotGroundedLastTick)
        {
            lastFramesGroundedValues[lastFrameGroundedCounter] = false;
        }
        lastFrameGroundedCounter++;
        if(lastFrameGroundedCounter >= lastFramesGroundedValues.Length)
        {
            lastFrameGroundedCounter = 0;
        }
        gotGroundedLastTick = false;
    }  
}
