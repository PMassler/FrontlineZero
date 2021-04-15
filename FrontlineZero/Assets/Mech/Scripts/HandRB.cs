using UnityEngine;

public class HandRB : MonoBehaviour
{
    public Transform target;
    public PositionController posPID;
    public Rigidbody rb;
    public WeaponSystem weaponSystem;
    public int leftOrRight;    
    public OVRGrabber ovrGrabber;


    public void PickUpWeapon(int weaponIndex)
    {
        weaponSystem.PickUpWeapon(weaponIndex, leftOrRight);
    }


    void Update()
    {       
        rb.MoveRotation(target.rotation);       
        rb.AddForce( posPID.getAcceleration(transform.position,rb.velocity,target.position), ForceMode.Acceleration);
    }      

    public Vector3 GetAngularVelocity()
    {     
        return ovrGrabber.GetAngularVelocity();
    }

    // vr haptics
    public void ShootVibrate(float frequency, float amplitude, float duration)
    {
        weaponSystem.WeaponShootVibrate(frequency, amplitude, duration, leftOrRight);
    }
}
