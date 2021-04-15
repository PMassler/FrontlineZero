using UnityEngine;

public class FlameThrower : Weapon
{
    public float shootingStrength;
    public GameObject bulletPrefab;
    public ParticleSystem[] muzzlePS;
    public int shotSoundIndex;
    public float fireSpawnDistance = 2f;


    // check trigger value
    void Update()
    {
        if(triggerValue > 0.1f && Time.time > lastShotTime+coolDown)
        {
            Shoot();
        }                          
    }

    // spawn fireparticles in front and add force
    void Shoot()
    {
        if (mechHandScript != null)
        {
            mechHandScript.ShootVibrate(1f, 0.5f, 0.1f);
        }
        Rigidbody bullet = Instantiate(bulletPrefab, transform.position + transform.forward * fireSpawnDistance, Quaternion.identity).GetComponent<Rigidbody>();
        bullet.transform.up = transform.forward;

        for (int i = 0; i < muzzlePS.Length; i++)
        {
            muzzlePS[i].Play();
        }

        bullet.AddForce(GetShootingVector() * shootingStrength * triggerValue, ForceMode.Impulse);
        lastShotTime = Time.time;
        SFXManager.Instance.PlaySFX(shotSoundIndex, transform.position);

        animator.SetTrigger("Shoot");

        AmmoCheck();
    }
}