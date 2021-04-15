using UnityEngine;

public class MechHydraulics : MonoBehaviour
{
    public Transform target;
    public Transform pipe;
    
    // positions hydraulic pipe (does jitter alot --> not used)
    void Update()
    {
        if (target != null)
        {
            Vector3 dir = transform.position - target.position;

            pipe.position = (transform.position + target.position) / 2;
            pipe.localScale = new Vector3(1, dir.magnitude, 1);

            pipe.transform.up = -dir;
            transform.up = -dir;
            target.up = dir;
        }
    }
}
