using UnityEngine;

public class SupplyDrop : DestructableUnit
{
    public int powerUpIndex;
    public float gravity;
    public Rigidbody rb;
    public ExplodingObject exObj;

    public int impactSFXIndex;
    public int particleImpactIndex;
    bool hasSpawned;

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasSpawned)
        {
            Impact();
        }
    }

    void Impact()
    {
        Destruct(rb.velocity);
        exObj.Explode();
        hasSpawned = true;
        SFXManager.Instance.PlaySFX(impactSFXIndex, transform.position);

        GameObject.Instantiate(SupplyManager.Instance.powerUpPrefab[powerUpIndex], transform.position, Quaternion.identity);
        base.Die();
    }
}
