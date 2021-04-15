using UnityEngine;
using UnityEngine.Events;

public class VRButton : VRInputObject
{
    public Animator buttonAnimator;
    public UnityEvent OnButtonDown;
    public UnityEvent OnButtonUp;
    public UnityEvent OnButtonHold;
    public float coolDown = 0.5f;
    public int clickSFX = 21;
    float lastPressTime;

    // trigger onButtonDown event
    public override void OnHandEnter(CustomInteractiveHands hand)
    {
        base.OnHandEnter(hand);

        if (lastPressTime + coolDown < Time.time)
        {
            lastPressTime = Time.time;
            SFXManager.Instance.PlaySFX(clickSFX, transform.position);
            buttonAnimator.SetBool("IsPressed", true);
            OnButtonDown.Invoke();
        }
    }

    // trigger onButtonUp event
    public override void OnHandExit()
    {
        buttonAnimator.SetBool("IsPressed", false);
        OnButtonUp.Invoke();
    }

    // trigger on buttonHoldEvent
    private void Update()
    {
        if(OnButtonHold.GetPersistentEventCount() != 0 && cHand != null)
        {
            OnButtonHold.Invoke();
        }
    }

    // for virtually pressing button from script
    public void PressButton()
    {
        OnButtonDown.Invoke();
        OnButtonUp.Invoke();
    }
}
