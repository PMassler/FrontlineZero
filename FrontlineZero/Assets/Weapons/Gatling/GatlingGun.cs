using UnityEngine;

public class GatlingGun : Weapon
{
    public ExplodingObject exObj;
    public ParticleSystem gatlingShootPS;

    public float rayStartDistance;

    public int VFXImpactIndex;
    public int VFXShotIndex;

    public float handRecoil;

    public int shotSoundIndex;
    public int hitSoundIndex;

    // check trigger value
    void Update()
    {
        if (triggerValue > 0.6f && Time.time > lastShotTime + coolDown)
        {
            Shoot();
        }
    }

    // shoot raycasts in front, if hit trigger explodingObject
    // bullets are visualized with particle system
    void Shoot()
    {
        // VR Haptics
        if (mechHandScript != null)
        {
            mechHandScript.ShootVibrate(1f, 1f, 0.1f);
        }
        // Hit Check
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.forward * rayStartDistance, transform.forward, out hit))
        {
            exObj.Explode(hit.point - transform.forward * 0.01f);
            // Hit SFX and VFX
            VFXManager.Instance.PlayVFX(VFXImpactIndex, hit.point, -transform.forward);
            SFXManager.Instance.PlaySFX(hitSoundIndex, hit.point);
        }
        lastShotTime = Time.time;
        AmmoCheck();
        // Recoil
        if (handRB != null)
        {
            handRB.AddForce(-transform.forward * handRecoil, ForceMode.Impulse);
        }
        // SFX and VFX
        SFXManager.Instance.PlaySFX(shotSoundIndex, transform.position);
        gatlingShootPS.Play();
        VFXManager.Instance.PlayVFX(VFXShotIndex, transform.position + transform.forward * rayStartDistance, transform.forward);
        animator.SetTrigger("Shoot");
    }
}