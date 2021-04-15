using UnityEngine;

public class CustomInteractiveHands : MonoBehaviour
{
    VRInputObject cInputObject;
    public Transform targetTransform;


    // triggers action when hand entes inputObject
    private void OnTriggerEnter(Collider other)
    {
        VRInputObject inputObject;
        inputObject = other.GetComponentInParent<VRInputObject>();
        if(inputObject != null)
        {
            inputObject.OnHandEnter(this);
            cInputObject = inputObject;
        }
    }

    // triggers action when hand leaves input object
    private void OnTriggerExit(Collider other)
    {
        if (cInputObject != null)
        {
            cInputObject.OnHandExit();
            cInputObject = null;
        }
    }
}
