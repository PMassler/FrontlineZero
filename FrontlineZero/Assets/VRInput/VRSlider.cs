using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRSlider : VRInputObject
{
    public Transform sliderStart;
    public Transform sliderEnd;
    public Transform sliderHandle;
    public UnityEvent<float> UpdateValue;
    public float currentValue;
    float sliderLength;
    public string playerPrefsName;
    public bool notSaving;
    public float baseValue;

    // load saved slider value
    private void Start()
    {
        sliderLength = Vector3.Distance(sliderStart.position, sliderEnd.position);

        if (!notSaving)
        {
            if (PlayerPrefs.HasKey(playerPrefsName))
            {
                SetValue(PlayerPrefs.GetFloat(playerPrefsName));
            }
            else
            {
                SetValue(baseValue);
            }
        }
        else
        {
            SetValue(baseValue);
        }
    }


    // slider handling
    private void Update()
    {
        if (cHand != null)
        {
            Vector3 slideVec = Vector3.Project(cHand.transform.position - sliderStart.position, sliderEnd.position - sliderStart.position);

            if (slideVec.magnitude > sliderLength)
            {
                sliderHandle.position = sliderEnd.position;
            }
            else if (Vector3.Dot(slideVec.normalized, (sliderEnd.position - sliderStart.position).normalized) <= 0)
            {
                sliderHandle.position = sliderStart.position;
            }
            else
            {
                sliderHandle.position = sliderStart.position + Vector3.Project(cHand.transform.position - sliderStart.position, sliderEnd.position - sliderStart.position);
            }
            currentValue = Mathf.Clamp(Vector3.Distance(sliderStart.position, sliderHandle.position) / sliderLength, 0.0001f, 1f);
            PlayerPrefs.SetFloat(playerPrefsName, currentValue);
            UpdateValue.Invoke(currentValue);
        }
    }
    

    

    // set value and save it to playerPrefs
    public void SetValue(float newValue)
    {
        PlayerPrefs.SetFloat(playerPrefsName, newValue);
        currentValue = newValue;
        sliderHandle.position = sliderStart.position + (sliderEnd.position - sliderStart.position).normalized * currentValue * sliderLength;
        UpdateValue.Invoke(currentValue);
    }

}
