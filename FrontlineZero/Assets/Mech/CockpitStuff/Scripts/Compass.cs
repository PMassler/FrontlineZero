using UnityEngine;

public class Compass : MonoBehaviour
{
    Quaternion targetRot;
    private void Start()
    {
        targetRot = transform.rotation*Quaternion.Inverse(Quaternion.identity);
    }

    void Update()
    {
        transform.rotation = targetRot;        
    }
}
