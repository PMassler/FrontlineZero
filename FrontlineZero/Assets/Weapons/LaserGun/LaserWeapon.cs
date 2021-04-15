using UnityEngine;

public class LaserWeapon : Weapon
{
    public ExplodingObject exObj;
    public ParticleSystem laserPS;

    public float rayStartDistance;

    public int particleManagerIndex;

    public int shotSoundIndex;
    public int hitSoundIndex;

    public float handRecoil;

    public LineRenderer laserLR;
    bool isShooting = false;


    void Start()
    {
        laserLR.positionCount = 2;
    }

    // check trigger value
    void Update()
    {
        if (triggerValue > 0.6f)
        {
            if (Time.time > lastShotTime + coolDown)
            {
                Shoot();
                if (!isShooting)
                {
                    isShooting = true;
                    laserLR.enabled = true;
                }
            }
        }
        else
        {
            if (isShooting)
            {
                isShooting = false;
                laserLR.enabled = false;
            }
        }
    }

    // shoot raycasts in front, if hit trigger explodingObject
    // laser is visualized with line renderer
    void Shoot()
    {        
        laserLR.SetPosition(0, laserLR.transform.position);

        if (mechHandScript != null)
        {
            mechHandScript.ShootVibrate(1f, 1f, 0.1f);
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.forward * rayStartDistance, transform.forward, out hit))
        {
            exObj.Explode(hit.point - transform.forward * 0.01f);
            ParticleManager.Instance.PlayParticleSystem(particleManagerIndex, hit.point, -transform.forward);
            SFXManager.Instance.PlaySFX(hitSoundIndex, hit.point);

            laserLR.SetPosition(1, hit.point);
        }
        else
        {
            laserLR.SetPosition(1, laserLR.transform.position+transform.forward*200f);
        }

        if (handRB != null)
        {
            handRB.AddForce(-transform.forward * handRecoil, ForceMode.Impulse);
        }
        SFXManager.Instance.PlaySFX(shotSoundIndex, transform.position);
        lastShotTime = Time.time;
        //laserPS.Play();

        animator.SetTrigger("Shoot");
        AmmoCheck();
    }
}
