using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MechGrabbable : MonoBehaviour
{
    public float damageMultiplier;
    public float ownDamageMultiplier;
    public float damageThreshhold;
    public Unit myUnit;
    public Rigidbody rb;

    public int hitSFXIndex;


    private void OnCollisionEnter(Collision collision)
    {
        //float impactValue = collision.relativeVelocity.magnitude;
        //float impactValue = rb.GetPointVelocity(collision.contacts[0].point).magnitude;

        float myImpactDamage = collision.relativeVelocity.magnitude;
        float hitImpactDamage = rb.GetPointVelocity(collision.contacts[0].point).magnitude;

        Unit collisionUnit = collision.gameObject.GetComponentInParent<Unit>();
        if (collisionUnit != null)
        {                       
            if (myImpactDamage > damageThreshhold)
            {
                float newMyImpactValue = myImpactDamage - damageThreshhold;
                float newHitImpactValue = hitImpactDamage - damageThreshhold;
                SFXManager.Instance.PlaySFX(hitSFXIndex,collision.GetContact(0).point);
                collisionUnit.ModifyHealth(-(int)(newHitImpactValue * damageMultiplier));
                if (myUnit != null)
                {
                    myUnit.ModifyHealth(-(int)(newMyImpactValue * ownDamageMultiplier));
                }
            }
        }
        else
        {            
            if (myImpactDamage > damageThreshhold && myUnit != null)
            {                
                float newMyImpactValue = myImpactDamage - damageThreshhold;
                SFXManager.Instance.PlaySFX(hitSFXIndex, collision.GetContact(0).point);
                myUnit.ModifyHealth(-(int)(newMyImpactValue * ownDamageMultiplier));
            }
        }
    }

}
