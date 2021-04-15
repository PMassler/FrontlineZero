using UnityEngine;

public class RotationController : MonoBehaviour
{
    public float maxVel;
    public float maxAcc;
    public float gainVel;
    public float gainAcc;

    // returns torque needed for the rigidbody to reach target rotation
    public Vector3 getAngularAcceleration(Quaternion rot, Vector3 angularVel, Quaternion targetRot)
    {               
        float angleDist;
        Vector3 axis;
        Quaternion dist = targetRot * Quaternion.Inverse(rot);
        dist.ToAngleAxis(out angleDist, out axis);
        if(angleDist >= 180)
        {
            angleDist = -360 + angleDist;
        }
        axis.Normalize();      
        float tgtVelAngle = Mathf.Clamp(gainVel * angleDist, -maxVel, maxVel);
        Vector3 velError = tgtVelAngle * axis - angularVel * Mathf.Rad2Deg;
        Vector3 tgtAcc = Vector3.ClampMagnitude(gainAcc * velError, maxAcc);

        if (tgtAcc.magnitude > 0.0001f)
        {
            return tgtAcc;
        }
        else
        {
            return Vector3.zero;
        }
    }
}