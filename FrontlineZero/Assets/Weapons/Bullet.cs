using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ExplodingObject explodingObject;
    public int VFXIndex;
    public int terrainLayerIndex;
    public bool onlyExplodeOnTerrain;
    public int SFXIndex;

   
    // Explode on collison
    private void OnCollisionEnter(Collision collision)
    {
        if (onlyExplodeOnTerrain)
        {
            if(collision.gameObject.layer == terrainLayerIndex)
            {
                explodingObject.Explode();
                VFXManager.Instance.PlayVFX(VFXIndex, transform.position, Vector3.up);
                SFXManager.Instance.PlaySFX(SFXIndex, transform.position);
                Destroy(gameObject);
            }
        }
        else
        {
            explodingObject.Explode();
            VFXManager.Instance.PlayVFX(VFXIndex, transform.position, -transform.up);
            SFXManager.Instance.PlaySFX(SFXIndex, transform.position);
            Destroy(gameObject);
        }        
    }
}
