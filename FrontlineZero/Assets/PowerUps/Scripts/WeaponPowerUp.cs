public class WeaponPowerUp : PowerUp
{
    public int weaponIndex;    
    public int VFXIndex;
    public int SFXIndex;

    public override void Consume(HandRB triggerHandRB)
    {
        VFXManager.Instance.PlayVFX(VFXIndex, triggerHandRB.transform.position, triggerHandRB.transform.forward);
        SFXManager.Instance.PlaySFX(SFXIndex, triggerHandRB.transform.position);
        triggerHandRB.PickUpWeapon(weaponIndex);
        if (!staysAfterConsume)
        {
            Destroy(gameObject);
        }
    }
}
