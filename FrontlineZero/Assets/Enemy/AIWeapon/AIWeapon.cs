using UnityEngine;

public class AIWeapon : MonoBehaviour
{
    public bool isActive;
    public Transform target;
    public Weapon weapon;
    public float maxRange;
    public float aimRandomness;
    public float lastSeenPlayer;
    public float maxAngle;
    public Transform dirTransform;

    void Start()
    {
        if (Player.Instance != null)
        {
            target = Player.Instance.transform;
        }
    }

    private void Update()
    {
        if (isActive && CheckLineOfSight())
        {
            Shoot();
        }
        else
        {
            weapon.triggerValue = 0f;
        }
    }

    public bool CheckLineOfSight()
    {
        if (target != null)
        {
            if (dirTransform != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, target.position - transform.position, out hit, maxRange) && Vector3.Angle(dirTransform.up, (hit.point - transform.position)) < maxAngle && Vector3.Distance(hit.point,target.position)<10f)
                {
                    lastSeenPlayer = Time.time;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, target.position - transform.position, out hit, maxRange) && Vector3.Distance(hit.point, target.position) < 10f)
                {
                    lastSeenPlayer = Time.time;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }
    }

    // Random offset for imperfect shooting
    public Vector3 GetRandomOffset()
    {
        Vector3 targetOffset = new Vector3(Mathf.PerlinNoise(Time.time + 10, Time.time), Mathf.PerlinNoise(Time.time + 100, Time.time - 30), Mathf.PerlinNoise(Time.time + 60, Time.time - 200));
        return targetOffset;
    }

    public virtual void Shoot()
    {

    }
}