using UnityEngine;

public class PositionController : MonoBehaviour
{   
    public float maxVel;
    public float maxAcc;
    public float gainVel;
    public float gainAcc;

    // returns force needed for the rigidbody to reach target position
    public Vector3 getAcceleration(Vector3 pos, Vector3 vel, Vector3 targetPos)
    {
        Vector3 dist = targetPos - pos;
        Vector3 tgtVel = Vector3.ClampMagnitude(gainVel * dist, maxVel);
        Vector3 error = tgtVel - vel;
        Vector3 retAcc = Vector3.ClampMagnitude(gainAcc * error, maxAcc);
        return retAcc;
    }
}
