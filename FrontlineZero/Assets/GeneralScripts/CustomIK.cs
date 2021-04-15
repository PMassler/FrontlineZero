using UnityEngine;

public class CustomIK : MonoBehaviour
{
    public int affectedBones;
    public Transform targetObj;
    public Transform poleObj; 

    public float toleranceDistance;
    public int maxIterations;
    [Range(0, 1)]
    public float originalPoseInfluence = 1f;  

    IKBone[] bones;
    Quaternion targetStartRotation;
    Transform rootObj;    

    private void Start()
    {
        InitializeIK();
    }

    void InitializeIK()
    {
        bones = new IKBone[affectedBones + 1];

        // Set Root Object
        rootObj = transform;
        for (var i = 0; i <= affectedBones; i++)
        {
            if (rootObj == null)
            {
                Debug.LogError("Affected Bonechain set to long!");
                return;
            }               
            rootObj = rootObj.parent;
        }

        // Set original target rotation     
        targetStartRotation = GetRotRootSpace(targetObj);

        // Set Data
        Transform currentBoneTransform = transform;
        for (int i = bones.Length-1; i >= 0; i--)
        {
            bones[i] = new IKBone();
            bones[i].realBone = currentBoneTransform;
            bones[i].startRotation = GetRotRootSpace(currentBoneTransform);

            if(i == bones.Length - 1)
            {
                bones[i].startDirection = GetPosRootSpace(targetObj) - GetPosRootSpace(currentBoneTransform);
            }
            else
            {
                bones[i].startDirection = GetPosRootSpace(bones[i+1].realBone) - GetPosRootSpace(currentBoneTransform);
                bones[i].boneLength = bones[i].startDirection.magnitude;
            }          
            currentBoneTransform = currentBoneTransform.parent;
        }
    }

    private void LateUpdate()
    {
        SolveIK();
    }

    void SolveIK()
    {
        if(targetObj == null)
        {
            return;
        }

        // Update bone and target values
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].position = GetPosRootSpace(bones[i].realBone);
        }
        Vector3 targetPosition = GetPosRootSpace(targetObj);
        Quaternion targetRotation = GetRotRootSpace(targetObj);

        // Original Pose Influence setback
        for (int i = 0; i < bones.Length - 1; i++)
        {
            bones[i + 1].position = Vector3.Lerp(bones[i + 1].position, bones[i].position + bones[i].startDirection, originalPoseInfluence);
        }

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            // end to root
            for (int i = bones.Length - 1; i > 0; i--)
            {
                if (i == bones.Length - 1)
                {
                    // Endeffector to target
                    bones[i].position = targetPosition;
                }
                else
                {
                    // Move bone towards previous bone
                    bones[i].position = bones[i + 1].position + (bones[i].position - bones[i + 1].position).normalized * bones[i].boneLength;
                }
            }
            // root to end - starting at first bone after root
            for (int i = 1; i < bones.Length; i++)
            {
                // Set bones position back to root
                bones[i].position = bones[i - 1].position + (bones[i].position - bones[i - 1].position).normalized * bones[i - 1].boneLength;
            }

            // Check if close enough, else iterate again
            if ((bones[bones.Length - 1].position - targetPosition).magnitude < toleranceDistance)
            {
                break;
            }
        }

        // Pole influence        
        if (poleObj != null)
        {
            Vector3 poleObjPosition = GetPosRootSpace(poleObj);
            for (int i = 1; i < bones.Length - 1; i++)
            {
                // Create plane perpendicular to vector from inward to outward bone, at inward bone position
                Plane plane = new Plane(bones[i + 1].position - bones[i - 1].position, bones[i - 1].position);
                // Calculate angle between projected bone position and pole position on plane relative to inward bone position
                Vector3 projectedPolePosition = plane.ClosestPointOnPlane(poleObjPosition);
                Vector3 projectedBonePosition = plane.ClosestPointOnPlane(bones[i].position);
                float angle = Vector3.SignedAngle(projectedBonePosition - bones[i - 1].position, projectedPolePosition - bones[i - 1].position, plane.normal);
                // Rotate bone position around calculated angle - it now is lined up with the pole object
                bones[i].position = Quaternion.AngleAxis(angle, plane.normal) * (bones[i].position - bones[i - 1].position) + bones[i - 1].position;
            }
        }              

        // Set calculated position and rotation of bone transform
        for (int i = 0; i < bones.Length; i++)
        {
            if (i == bones.Length - 1)
            {
                // Set Endeffector rotation to target rotation
                SetRotRootSpace(bones[i].realBone, Quaternion.Inverse(targetRotation) * targetStartRotation * Quaternion.Inverse(bones[i].startRotation));
            }
            else
            {
                SetRotRootSpace(bones[i].realBone, Quaternion.FromToRotation(bones[i].startDirection, bones[i + 1].position - bones[i].position) * Quaternion.Inverse(bones[i].startRotation));
                SetPosRootSpace(bones[i].realBone, bones[i].position);
            }
        }
    }

    // Space conversions
    private Vector3 GetPosRootSpace(Transform cTransform)
    {
        return Quaternion.Inverse(rootObj.rotation) * (cTransform.position - rootObj.position);
    }

    private void SetPosRootSpace(Transform cTransform, Vector3 position)
    {
        cTransform.position = rootObj.rotation * position + rootObj.position;
    }

    private Quaternion GetRotRootSpace(Transform cTransform)
    {
        return Quaternion.Inverse(cTransform.rotation) * rootObj.rotation;
    }

    private void SetRotRootSpace(Transform cTransform, Quaternion rotation)
    {
        cTransform.rotation = rootObj.rotation * rotation;
    }
    
    public class IKBone
    {
        public float boneLength;
        public Transform realBone;
        public Vector3 position;
        public Vector3 startDirection;
        public Quaternion startRotation;       
    }
}
