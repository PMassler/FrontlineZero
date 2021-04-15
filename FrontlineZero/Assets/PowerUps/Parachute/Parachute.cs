using UnityEngine;

public class Parachute : MonoBehaviour
{
    public Rigidbody target;

    public float unfoldHeight;
    public float disconnectHeight;
    public float despawnHeight;
    public float drag;

    public Vector3 windDirection;
    public float windSpeed;

    public float ropeLength;

    public Animator animator;

    bool unfolded;
    bool disconnected;

    float chuterDrag;

    public LineRenderer[] ropeMounts;

    public float ropeMountOffset;

    void Start()
    {
        for (int i = 0; i < ropeMounts.Length; i++)
        {
            ropeMounts[i].positionCount = 2;
        }
    }

    // check movement status, to play parachute animations on time
    void Update()
    {
        if (target != null)
        {
            Vector3 targetVector = target.position - transform.position;

            if (!disconnected && targetVector.magnitude > ropeLength)
            {
                transform.position = target.position - targetVector.normalized * ropeLength;
                transform.up = -targetVector;
            }

            if (!unfolded && target.position.y < unfoldHeight && target.velocity.y < 0 && transform.position.y > target.position.y + ropeLength / 2)
            {
                unfolded = true;
                chuterDrag = target.drag;
                target.drag = drag;
                animator.SetBool("unfolded", true);
            }
            else if (unfolded && !disconnected && target.position.y < disconnectHeight)
            {
                disconnected = true;
                target.drag = chuterDrag;
                transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(windDirection, Vector3.up));
                animator.SetBool("disconnected", true);
            }

            if (!disconnected)
            {
                for (int i = 0; i < ropeMounts.Length; i++)
                {
                    ropeMounts[i].SetPosition(0, ropeMounts[i].transform.position);
                    ropeMounts[i].SetPosition(1, target.position + Vector3.up * ropeMountOffset);
                }
            }
            else
            {
                for (int i = 0; i < ropeMounts.Length; i++)
                {
                    ropeMounts[i].SetPosition(0, ropeMounts[i].transform.position);
                    ropeMounts[i].SetPosition(1, ropeMounts[i].transform.position + (target.position - ropeMounts[i].transform.position).normalized * ropeLength);
                }

                transform.position += windDirection.normalized * windSpeed;
                windSpeed += 0.03f;
                if (transform.position.y > despawnHeight)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {            
            for (int i = 0; i < ropeMounts.Length; i++)
            {
                ropeMounts[i].SetPosition(0, ropeMounts[i].transform.position);
                ropeMounts[i].SetPosition(1, ropeMounts[i].transform.position + (windDirection - ropeMounts[i].transform.position).normalized * ropeLength);
            }

            transform.position += windDirection.normalized * windSpeed;
            if (transform.position.y > despawnHeight)
            {
                Destroy(gameObject);
            }
        }

    }
}
