using UnityEngine;

public class DestructionPart : MonoBehaviour
{
    public float livingTime;
    public int VFXIndex;
    public int SFXIndex;

    private void Start()
    {
        Invoke("Destruct", livingTime);
    }

    void Destruct()
    {
        SFXManager.Instance.PlaySFX(SFXIndex, transform.position);
        VFXManager.Instance.PlayVFX(VFXIndex, transform.position, Vector3.up);
        Destroy(gameObject);
    }    
}
