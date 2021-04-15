using UnityEngine;

public class AIRocketLauncher : AIWeapon
{
    public float distanceHeightMultiplier;

    public override void Shoot()
    {
        if (target != null)
        {
            // Aim above Target, so Rocket flies in an arch
            Vector3 aimTarget;
            aimTarget = (transform.position + target.position) / 2 + Vector3.up * distanceHeightMultiplier * Mathf.Pow(Vector3.Distance(transform.position, target.position), 2f); 
            transform.up = aimTarget - (transform.position + GetRandomOffset() * aimRandomness);
            weapon.triggerValue = 1f;
        }
    }
}
