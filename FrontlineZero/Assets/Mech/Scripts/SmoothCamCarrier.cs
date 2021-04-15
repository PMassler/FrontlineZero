using UnityEngine;

public class SmoothCamCarrier : MonoBehaviour
{    
    public Transform target;    
    public float followDistance;
    [Range(0f,1f)]
    public float followSpeed;
    public int viewDirection;

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPos;
            switch (viewDirection)
            {
                case 0:
                    targetPos = target.position - target.forward * followDistance;
                    break;
                case 1:
                    targetPos = target.position + target.right * followDistance;
                    break;
                case 2:
                    targetPos = target.position + target.forward * followDistance;
                    break;
                case 3:
                    targetPos = target.position - target.right * followDistance;
                    break;
                default:
                    targetPos = Vector3.zero;
                    break;
            }            
                transform.position = targetPos;
                transform.forward = target.position - transform.position;            
        }
    }

    public void SwitchViewDirection()
    {
        viewDirection++;
        if (viewDirection > 3)
        {
            viewDirection = 0;
        }
    }
}
