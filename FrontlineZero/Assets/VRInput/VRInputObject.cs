using UnityEngine;

public class VRInputObject : MonoBehaviour
{
    [HideInInspector]
    public CustomInteractiveHands cHand;

    public virtual void OnHandEnter(CustomInteractiveHands hand)
    {
        cHand = hand;
    }
    public virtual void OnHandExit()
    {
        cHand = null;
    }
}
