public class AIGatlingGun : AIWeapon
{
    public override void Shoot()
    {
        transform.up = target.position - (transform.position + GetRandomOffset() * aimRandomness);
        weapon.triggerValue = 1f;
    }
}
