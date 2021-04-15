using UnityEngine;

public class FireBarrel : Unit
{
    public ExplodingObject exObj;
    public GameObject fireParticle;
    public float particleCount;
    public float fireSpawnRadius;
    [Range(0f,1f)]
    public float fireSpread;

    public int VFXIndex;
    public int SFXIndex;

   // on death spawn fire particles
    public override void Die()
    {
        canTakeDamage = false;
        VFXManager.Instance.PlayVFX(VFXIndex, transform.position, transform.up);
        SFXManager.Instance.PlaySFX(SFXIndex, transform.position);
        SpawnFire();
        exObj.Explode();
        Destroy(gameObject);
    }

    // spawn fireparticles and add force
    void SpawnFire()
    {
        for (int i = 0; i < particleCount; i++)
        {
            Instantiate(fireParticle,transform.position+ (Vector3.right * Random.Range(-fireSpread, fireSpread) + Vector3.forward * Random.Range(-fireSpread, fireSpread))* fireSpawnRadius, Quaternion.identity);
        }
    }

}
