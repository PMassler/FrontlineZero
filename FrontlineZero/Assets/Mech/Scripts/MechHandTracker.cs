using UnityEngine;

public class MechHandTracker : MonoBehaviour
{
    public Transform leftPlayerShoulder;
    public Transform rightPlayerShoulder;
    public Transform leftPlayerTarget;
    public Transform rightPlayerTarget;

    public Transform leftMechShoulder;
    public Transform rightMechShoulder;
    public Transform leftMechTarget;
    public Transform rightMechTarget;

    public float scale;
    public float maxRange;
   

    void Update()
    {
        //Transfer controller movement to the mechs hands
        leftMechTarget.position = leftMechShoulder.position + Vector3.ClampMagnitude((leftPlayerTarget.position-leftPlayerShoulder.position) * scale,maxRange);
        rightMechTarget.position = rightMechShoulder.position + Vector3.ClampMagnitude((rightPlayerTarget.position-rightPlayerShoulder.position) * scale,maxRange);

        leftMechTarget.rotation = leftPlayerTarget.rotation;
        rightMechTarget.rotation = rightPlayerTarget.rotation;
    }
}
