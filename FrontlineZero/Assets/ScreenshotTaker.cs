using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    public int startInt;

    int addInt;

    public GameObject screenshotCamera;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeScreenshot();
        }


        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick) || OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            TakeScreenshot();
            Debug.Log("TakeScreenshot");
            VRAudioDebugger.Instance.DebugTrue();
        }

    }

    [ContextMenu("TakeScreenshot")]
    public void TakeScreenshot()
    {
        screenshotCamera.SetActive(true);
        ScreenCapture.CaptureScreenshot("ScreenShots/Screenshot" + (startInt + addInt)+".png");
        addInt++;
        screenshotCamera.SetActive(false);
    }
}
