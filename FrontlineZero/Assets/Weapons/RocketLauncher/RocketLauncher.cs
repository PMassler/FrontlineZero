using UnityEngine;

public class RocketLauncher : Weapon
{
    public float shootingStrength;
    public GameObject bulletPrefab;

    public float handRecoil;

    public int shotSoundIndex;

    public float rocketSpawnDistance = 2f;

    // check trigger value
    void Update()
    {
        if(triggerValue > 0.6f && Time.time > lastShotTime+coolDown)
        {
            Shoot();
        }                         
    }
    
    // instantiate rocket prefab in front and add force
    void Shoot()
    {
        if (mechHandScript != null)
        {
            mechHandScript.ShootVibrate(0.5f, 1f, 0.1f);
        }
        Rigidbody bullet = Instantiate(bulletPrefab, transform.position + transform.forward * rocketSpawnDistance, Quaternion.identity).GetComponent<Rigidbody>();
        bullet.transform.up = transform.forward;
        if(handRB != null)
        {
            handRB.AddForce(-transform.forward * handRecoil, ForceMode.Impulse);
        }
        bullet.AddForce(transform.forward * shootingStrength,ForceMode.Impulse);
        lastShotTime = Time.time;
        SFXManager.Instance.PlaySFX(shotSoundIndex, transform.position);

        animator.SetTrigger("Shoot");

        AmmoCheck();
    }
}