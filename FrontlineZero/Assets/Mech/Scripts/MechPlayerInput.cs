using UnityEngine;

public class MechPlayerInput : MonoBehaviour
{
    public MechPhysics mechScript;
    public bool isVR;
    public HandsController leftController;
    public HandsController rightController;

    void Update()
    {
        // Rotation        
        float rotInput = 0;
        if (isVR && rightController.isGrabbed)
        {
            rotInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
            mechScript.pitchInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
        }
        else
        {
            if (Input.GetKey(KeyCode.Q))
            {
                rotInput -= 1;
            }
            if (Input.GetKey(KeyCode.E))
            {
                rotInput = 1;
            }
        }
        mechScript.rotInput = rotInput;

        // Movement
        if (isVR)
        {
            if (leftController.isGrabbed)
            {
                mechScript.verticalInput = Input.GetAxis("Vertical");
                mechScript.horizontalInput = Input.GetAxis("Horizontal");
            }
            else
            {
                mechScript.verticalInput = 0f;
                mechScript.horizontalInput = 0f;
            }
        }
        else
        {
            mechScript.verticalInput = Input.GetAxis("Vertical");
            mechScript.horizontalInput = Input.GetAxis("Horizontal");
        }      

        // Jump
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mechScript.Jump();
        }
        */
    }
}
