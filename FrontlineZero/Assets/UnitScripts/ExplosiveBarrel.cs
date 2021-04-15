public class ExplosiveBarrel : Unit
{
    public ExplodingObject exObj;

    public int VFXIndex;
    public int SFXIndex;

    // die with big explosion
    public override void Die()
    {
        canTakeDamage = false;
        VFXManager.Instance.PlayVFX(VFXIndex, transform.position, transform.up);
        SFXManager.Instance.PlaySFX(SFXIndex, transform.position);
        exObj.Explode();
        Destroy(gameObject);
    }
}
