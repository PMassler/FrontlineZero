using UnityEngine;

public class MatterLauncher : Weapon
{
    public ExplodingObject exObj;
    public ParticleSystem laserPS;

    public float rayStartDistance;

    public int particleManagerIndex;

    public int shotSoundIndex;
    public int hitSoundIndex;

    public float handRecoil;

    public GameObject matterBall;

    // check trigger value
    void Update()
    {
        if (triggerValue > 0.6f && Time.time > lastShotTime + coolDown)
        {
            Shoot();
        }
    }

    // shoot raycasts in front, if hit trigger explodingObject
    // matterlaser is visualized with particle system
    void Shoot()
    {
        if (mechHandScript != null)
        {
            mechHandScript.ShootVibrate(1f, 0.5f, 0.1f);
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position + GetShootingVector() * rayStartDistance, transform.forward, out hit))
        {
            if (hit.collider.tag == "MCTerrain")
            {
                exObj.Explode(hit.point - transform.forward * 0.01f);
                ParticleManager.Instance.PlayParticleSystem(particleManagerIndex, hit.point, -transform.forward);
                SFXManager.Instance.PlaySFX(hitSoundIndex, hit.point);
            }
            else
            {
                GameObject.Instantiate(matterBall, hit.point, Quaternion.identity);
            }
        }

        if (handRB != null)
        {
            handRB.AddForce(-transform.forward * handRecoil, ForceMode.Impulse);
        }
        SFXManager.Instance.PlaySFX(shotSoundIndex, transform.position);
        lastShotTime = Time.time;
        laserPS.Play();

        animator.SetTrigger("Shoot");
        AmmoCheck();
    }
}