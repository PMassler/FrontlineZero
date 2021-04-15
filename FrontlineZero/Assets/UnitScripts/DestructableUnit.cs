using UnityEngine;

public class DestructableUnit : Unit
{
    public float rbMass;
    public float partLivingTime;
    public int partExplosionVFXIndex;
    public int partExplosionSFXIndex;  

    // add rigidbodys to each limb and add explosion force
    public void Destruct(Vector3 velocity)
    {
        for (int i = 0; i < damageMatMesh.Length; i++)
        {
            Transform part = damageMatMesh[i].transform;
            part.parent = null;

            Rigidbody partRB = part.GetComponent<Rigidbody>();            
            if (partRB == null)
            {
                    partRB = part.gameObject.AddComponent<Rigidbody>();
                partRB.mass = rbMass;
            }
            partRB.velocity = velocity;

            DestructionPart dPart = part.gameObject.AddComponent<DestructionPart>();
            dPart.livingTime = partLivingTime * Random.Range(0.8f,1.2f);
            dPart.VFXIndex = partExplosionVFXIndex;
            dPart.SFXIndex = partExplosionSFXIndex;
            //part.gameObject.AddComponent<MeshCollider>().convex = true;
            part.gameObject.AddComponent<BoxCollider>();          
        }
    }      
}
