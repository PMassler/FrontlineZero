using UnityEngine;

public class Drill : Weapon
{
    public ExplodingObject exObj;

    public int particleManagerIndex;
    public int sfxManagerDrillIndex;
    public int sfxManagerDigIndex;

    // check trigger value
    void Update()
    {
        if(triggerValue > 0.6f && Time.time > lastShotTime+coolDown)
        {
            DoDrill();
        }                         
    }

    // trigger explodingObject at own position
    void DoDrill()
    {
        if (exObj.Explode() > 0.2f)
        {
            SFXManager.Instance.PlaySFX(sfxManagerDigIndex, transform.position);
            ParticleManager.Instance.PlayParticleSystem(particleManagerIndex, transform.position, transform.forward);
        }
        lastShotTime = Time.time;
        SFXManager.Instance.PlaySFX(sfxManagerDrillIndex, transform.position);
        animator.SetTrigger("Shoot");
        AmmoCheck();
    }
}