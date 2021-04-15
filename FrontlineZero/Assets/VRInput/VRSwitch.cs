using UnityEngine;
using UnityEngine.Events;

public class VRSwitch : VRInputObject
{
    public Animator switchAnimator;
    public UnityEvent<bool> OnPress;   
    bool isOn;    

    public override void OnHandEnter(CustomInteractiveHands hand)
    {
        base.OnHandEnter(hand);
        if (isOn)
        {
            isOn = false;
        }
        else
        {
            isOn = true;
        }

        switchAnimator.SetBool("IsOn", isOn);
        OnPress.Invoke(isOn);
    }
    
    // trigger onPress event with bool
    public void SetIsOn(bool newIsOn)
    {
        isOn = newIsOn;
        switchAnimator.SetBool("IsOn", isOn);
        OnPress.Invoke(isOn);
    }
}
