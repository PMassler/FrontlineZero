using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float despawnTime;
    public bool staysAfterConsume;
    float spawnTime;

    private void OnTriggerEnter(Collider other)
    {
        HandRB triggerHandRB = other.gameObject.GetComponentInParent<HandRB>();
        if (triggerHandRB != null)
        {
            Consume(triggerHandRB);
        }
    }

    // pick up powerup
    public virtual void Consume(HandRB triggerHandRB)
    {

    }

    public virtual void Despawn()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        // despawn after set time
        if(!staysAfterConsume && Time.time > spawnTime + despawnTime)
        {
            Despawn();
        }
    }
}
