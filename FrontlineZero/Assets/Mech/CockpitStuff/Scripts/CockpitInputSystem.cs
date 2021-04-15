using UnityEngine;
using UnityEngine.Audio;

public class CockpitInputSystem : MonoBehaviour
{
    public MechPhysics mechScript;
    public PlayerUnit playerScript;
    public SmoothCamCarrier camCarrier;
    public Transform vrRigContainer;
    public Transform vrRig;
    public Transform vrCam;
    public Vector3 VRUpOffset;

    public AudioMixer sfxMixer;
    public AudioMixer musicMixer;


    public void SetRotStabilisation(float value)
    {
        mechScript.rotStabelization = 0.9f + value*0.1f;
    }

    public void SetMechHeight(float value)
    {
        mechScript.targetHeight = 4f + value * 4f;
    }

    public void ChangeMusicVolume(float value)
    {
        musicMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
    }

    public void ChangeSFXVolume(float value)
    {
        sfxMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);        
    }

    public void UnStuckMech()
    {
        mechScript.UnStuck();
    }

    public void Exit()
    {
        playerScript.ModifyHealth(-playerScript.maxHealth);
    }

    public void SetVRPos()
    {
        vrRig.localPosition = vrRigContainer.localPosition - vrCam.localPosition + vrRigContainer.right* VRUpOffset.x + vrRigContainer.up * VRUpOffset.y + vrRigContainer.forward * VRUpOffset.z;
    }

    public void ChangeCamView()
    {
        camCarrier.SwitchViewDirection();
    }
}
