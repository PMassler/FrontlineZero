using UnityEngine;

public class MechPhysics : MonoBehaviour
{       
    public Rigidbody rb;
    public Transform COM;
    public rbFoot[] rbFeet;
    public float targetHeight;
    public PositionController posPID;
    public RotationController rotPID;    
    // Feet 
    public float stepUpwardForce;
    public float stepForwardForce;
    public float stepDownwardForce;
    public float turnAngle;
    public float turnAngleForStep;
    public float maxFeetDistance;
    public float stepWidth;
    public float stepHeight;
    public float feetGravity;
    public float jumpForce;

    Vector3 feetCenter;
    [HideInInspector]
    public Vector3 feetForwardVector;
    Vector3 feetToFeetVector;
    public bool isStepping;

    public float maxPitch;   
    float pitchValue;

    [Range(0f, 1f)]
    public float rotStabelization;
    [Range(0f, 10f)]
    public float posSmoothnes;
    [Range(0f, 10f)]
    public float rotSmoothnes;
    [Range(0f, 1f)]
    public float footRotSpeed;
    [HideInInspector]
    public Vector3 stepDirection;
    [HideInInspector]
    public float pitchInput;
    [HideInInspector]
    public float rotInput;
    [HideInInspector]
    public float horizontalInput;
    [HideInInspector]
    public float verticalInput;
    public HandRB leftHand;
    public HandRB rightHand;
    float startTime;

    void Update()
    {               
        if(Time.time > startTime + 0.5f)
        {
            InputProcessing();           
        }
    }

    void InputProcessing()
    {
        // rotate feet from rotation intput
        pitchValue = pitchInput * maxPitch;
        for (int i = 0; i < rbFeet.Length; i++)
        {
            rbFeet[i].rb.rotation *= Quaternion.Euler(Vector3.up * turnAngle * rotInput);
        }

        // Movement
        float movementVertical = verticalInput;
        float movementHorizontal = horizontalInput;
        stepDirection = (movementVertical * feetForwardVector + -movementHorizontal * Vector3.Cross(feetForwardVector, Vector3.up)).normalized;
        if (!isStepping)
        {
            // trigger step on player input
            if (stepDirection != Vector3.zero)
            {
                int footToStep = GetFootToStep(stepDirection);
                Step(footToStep);
            }            

            // trigger step from rotation
            Vector3 feetVec = Vector3.ProjectOnPlane(rbFeet[0].transform.position - rbFeet[1].transform.position, Vector3.up).normalized;
            float angle = Vector3.Angle(feetForwardVector, feetVec);

            if (angle < turnAngleForStep || angle > 180 - turnAngleForStep && !isStepping)
            {
                int footToStep = GetFootToStep(feetForwardVector);
                Step(footToStep);
            }

            // trigger step from do high of a distance between feet
            float feetDistance = (rbFeet[0].transform.position - rbFeet[1].transform.position).magnitude;
            if (feetDistance > maxFeetDistance && !isStepping)
            {
                Step(GetHighestFoot());
            }
        }
    }

    public void Jump()
    {
        for (int i = 0; i < rbFeet.Length; i++)
        {
            rbFeet[i].footState = rbFoot.FootState.Idle;
            rbFeet[i].rb.AddForce(Vector3.up * jumpForce);
        }
    }

    

    
    void Step(int footID)
    {
        // Check if is able to step
        if(footID == 0 && rbFeet[1].footState != rbFoot.FootState.Lifting || footID == 1 && rbFeet[0].footState != rbFoot.FootState.Lifting)
        {
            rbFeet[footID].Step();
            isStepping = true;
        }            
    }

    // returns index of highest foot
    int GetHighestFoot()
    {      
        float height = -Mathf.Infinity;
        int footToStep = 0;
        for (int i = 0; i < rbFeet.Length; i++)
        {
            if (rbFeet[i].transform.position.y > height)
            {
                height = rbFeet[i].transform.position.y;
                footToStep = i;
            }
        }
        return footToStep;
    }

    // returns index of foot on which a step should be triggered, based on movement direction
    int GetFootToStep(Vector3 stepDirection)
    {        
        float distance = -Mathf.Infinity;
        int footToStep = 0;
        for (int i = 0; i < rbFeet.Length; i++)
        {
            if (Vector3.Dot(stepDirection, rbFeet[i].GetNextStepDirection()) > distance)
            {
                distance = Vector3.Dot(stepDirection, rbFeet[i].GetNextStepDirection());
                footToStep = i;
            }
        }
        return footToStep;
    }

    private void FixedUpdate()
    {
        CalcFeetForwardVector();
        StabilizeAboveFeet();
    }


    void CalcFeetForwardVector()
    {   
        feetForwardVector = Vector3.zero;
        for (int i = 0; i < rbFeet.Length; i++)
        {            
            feetForwardVector += rbFeet[i].transform.forward;
        }       
        feetForwardVector = feetForwardVector.normalized;
        feetToFeetVector = rbFeet[0].transform.position - rbFeet[1].transform.position;
    }


    void StabilizeAboveFeet()
    {
        // Calculate Position
        Vector3 stablePoint = Vector3.zero;
        for (int i = 0; i < rbFeet.Length; i++)
        {
            stablePoint += rbFeet[i].transform.position;
        }
        stablePoint /= rbFeet.Length;
        stablePoint += new Vector3(0, targetHeight, 0);    

        // Move to Position
        rb.AddForce(posPID.getAcceleration(COM.position,rb.velocity,stablePoint), ForceMode.Acceleration);

        // Calculate Rotation
        Vector3 viewDirection = Vector3.zero;
        for (int i = 0; i < rbFeet.Length; i++)
        {
            viewDirection += rbFeet[i].transform.forward;
        }
        viewDirection = Quaternion.AngleAxis(pitchValue,Vector3.Cross(feetForwardVector,Vector3.up)) * Vector3.ProjectOnPlane(viewDirection, Vector3.up);       
        Vector3 upDirection = Vector3.Lerp((Vector3.Cross(feetToFeetVector, viewDirection)), Vector3.up, rotStabelization);       
        Vector3 torque = rotPID.getAngularAcceleration(transform.rotation, rb.angularVelocity, Quaternion.LookRotation(viewDirection, upDirection));

        // Rotate to Rotation
        rb.AddTorque(torque, ForceMode.Acceleration);            
    }
    
    // resets position of hands and feet
    public void UnStuck()
    {
        leftHand.transform.position = transform.position - transform.right * 4;
        rightHand.transform.position = transform.position + transform.right * 4;

        rbFeet[0].transform.position = transform.position - transform.right * stepWidth / 2;
        rbFeet[1].transform.position = transform.position + transform.right * stepWidth / 2;
    }

    private void Start()
    {
        startTime = Time.time;
    }
}
