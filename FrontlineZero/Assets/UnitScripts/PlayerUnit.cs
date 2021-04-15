using UnityEngine;

public class PlayerUnit : DestructableUnit
{
    public Rigidbody rb;
    public Transform ejectorSeat;
    public float ejectForce;
    public Vector3 ejectDirection;
    public Parachute ejectParachute;
    public Transform healthTorus;

    // on death eject seat with VRCameraRig
    void EjectSeat()
    {
        Rigidbody ejectorSeatRB = ejectorSeat.gameObject.AddComponent<Rigidbody>();
        ejectorSeatRB.mass = rbMass;
        //ejectorSeatRB.gameObject.AddComponent<MeshCollider>().convex = true;
        ejectorSeatRB.transform.parent = null;
        ejectorSeatRB.AddForce(transform.rotation*ejectDirection * ejectForce, ForceMode.Impulse);

        ejectParachute.transform.parent = null;
        ejectParachute.target = ejectorSeatRB;
        ejectParachute.gameObject.SetActive(true);
        
    }

    // in addition to changing healt´hvalue, also change healthbar in the cockpit
    public override void ModifyHealth(int value)
    {
        float rotAngle = ((float)value / (float)maxHealth)*180f;
        healthTorus.Rotate(Vector3.forward, -rotAngle, Space.Self);
        base.ModifyHealth(value);
    }

    // damageSource parameter prevents taking damgage more than one time if multiple colliders are hit by explosion
    public override void ModifyHealth(int value, MonoBehaviour damageSource)
    {
        if (canTakeDamage)
        {
            if (damageSource != lastDamageSource)
            {
                currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
                float rotAngle = ((float)value / (float)maxHealth) * 180f;
                healthTorus.Rotate(Vector3.forward, -rotAngle, Space.Self);
                for (int i = 0; i < damageMatMesh.Length; i++)
                {
                    damageMatMesh[i].material.SetFloat("DamageTaken", 1f - (float)currentHealth / (float)maxHealth);
                }
                if (currentHealth == 0)
                {
                    Die();
                }
                lastDamageSource = damageSource;
            }
        }
    }

    public override void Die()
    {
        EjectSeat();
        Destruct(rb.velocity);
        RoundManager.Instance.EndRoundIn(10f);
        base.Die();
    }
}
