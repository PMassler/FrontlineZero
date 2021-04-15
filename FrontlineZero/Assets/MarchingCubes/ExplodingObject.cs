using UnityEngine;

public class ExplodingObject : MonoBehaviour
{
    public float physicExplosionRadius;
    public float physicExplosionStrength;
    public float physicExplosionUpforce;
    public int unitDamage;

    public ExplosionSphere[] exSpheres;    

    // trigger all set ExplosionSpheres at gameObject
    public float Explode()
    {
        float percentage = 0;
        for (int i = 0; i < exSpheres.Length; i++)
        {
            try
            {
            percentage += MC_EditInterface.Instance.ModifySphere(transform.rotation*exSpheres[i].position+transform.position,exSpheres[i].radius,exSpheres[i].strength, exSpheres[i].smoothness);
            }
            catch
            {

            }
        }        
        PhysicsExplosion(transform.position);
        return percentage / exSpheres.Length;        
    }

    // trigger all set ExplosionSpheres at parameter pos
    public float Explode(Vector3 pos)
    {
        float percentage = 0;        
        for (int i = 0; i < exSpheres.Length; i++)
        {
            try
            {
                percentage += MC_EditInterface.Instance.ModifySphere(transform.rotation * exSpheres[i].position + pos, exSpheres[i].radius, exSpheres[i].strength, exSpheres[i].smoothness);
            }
            catch
            {

            }
        }        
        PhysicsExplosion(pos);
        return percentage / exSpheres.Length;
    }

    // trigger all set ExplosionSpheres at parameter pos with rotation rot
    public float Explode(Vector3 pos, Quaternion rot)
    {
        float percentage = 0;
        for (int i = 0; i < exSpheres.Length; i++)
        {
            try
            {
                percentage += MC_EditInterface.Instance.ModifySphere(rot * exSpheres[i].position + pos, exSpheres[i].radius, exSpheres[i].strength, exSpheres[i].smoothness);
            }
            catch
            {

            }
        }       
        PhysicsExplosion(pos);
        return percentage / exSpheres.Length;
    }



    // add force to hit rigidbodies, and modify health of hit units
    void PhysicsExplosion(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, physicExplosionRadius);
        foreach (Collider coll in colliders)
        {
            Rigidbody rb = coll.GetComponentInParent<Rigidbody>();
            Unit unit = coll.GetComponentInParent<Unit>();
            if (rb != null)
            {
                rb.AddExplosionForce(physicExplosionStrength, pos, physicExplosionRadius, physicExplosionUpforce, ForceMode.Impulse);
            }
            if (unit != null)
            {
                unit.ModifyHealth(unitDamage, this);
            }
        }
    }


    [System.Serializable]
    public class ExplosionSphere
    {
        public Vector3 position;
        public float radius;
        public float strength;
        [Range(0f,1f)]
        public float smoothness;
    }
}
