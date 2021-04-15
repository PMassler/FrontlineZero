using UnityEngine;

public class FlameParticle : MonoBehaviour
{
    public int damage;
    public float tickCooldown;
    public float damageUpRange;
    public Rigidbody rb;
    public Collider coll;
    Unit stickedUnit;
    float lastTickTime;
    bool isSticking;
    public float livingTime;
    public AudioSource fireAS;
    public static int counter;

    void Start()
    {
        Invoke("Despawn", livingTime);

        // only every sixth fire particle plays fire sound for perfomance increase
        if(counter == 0)
        {
            fireAS.Play();
        }
        else if(counter >= 6)
        {
            counter = -1;
        }
        counter++;
    }

    void Despawn()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        // every few seconds do a tick
        if(Time.time > lastTickTime + tickCooldown)
        {
            Tick();
        }
    }

    // each tick damages unity above and unit the fire sticks to
    void Tick()
    {        
        lastTickTime = Time.time;
        if(stickedUnit != null)
        {
            stickedUnit.ModifyHealth(damage);
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position,Vector3.up,out hit, damageUpRange))
        {
            Unit unit = hit.collider.GetComponentInParent<Unit>();
            if (unit != null)
            {

                unit.ModifyHealth(damage);
            }
        }
    }

    // stick to colliders
    private void OnCollisionEnter(Collision collision)
    {
        if (!isSticking)
        {
            transform.parent = collision.transform;
            //rb.isKinematic = true;
            Destroy(rb);
            isSticking = true;
            coll.enabled = false;

            Unit unit = collision.collider.GetComponentInParent<Unit>();
            if (unit != null)
            {
                stickedUnit = unit;
            }
        }       
    }
}
