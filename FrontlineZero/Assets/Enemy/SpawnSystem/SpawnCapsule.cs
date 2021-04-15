using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCapsule : DestructableUnit
{
    public int enemyIndex;
    public float gravity;
    public Rigidbody rb;
    public ExplodingObject exObj;
    public int impactSFXIndex;
    bool hasSpawned;


    // Start is called before the first frame update
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
        SFXManager.Instance.PlaySFX(impactSFXIndex, transform.position);
        GameObject.Instantiate(SpawnManager.Instance.enemyPrefabs[enemyIndex], transform.position, Quaternion.identity);
        hasSpawned = true;
        base.Die();
    }
}
