using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public Weapon[] leftWeapons;
    public Weapon[] rightWeapons;

    Weapon currentLeftWeapon;
    Weapon currentRightWeapon;


    List<int> collectedLeftWeapons;
    List<int> collectedRightWeapons;

    int currentCollLeftWeapon = 0;
    int currentCollRightWeapon = 0;

    public HandsController leftController;
    public HandsController rightController;

    public Transform leftAmmoDisplay;
    public Transform rightAmmoDisplay;



    private void Start()
    {
        currentLeftWeapon = leftWeapons[0];
        currentRightWeapon = rightWeapons[0];

        collectedLeftWeapons = new List<int>(1) { 0 };
        collectedRightWeapons = new List<int>(1) { 0 };
    }


    void Update()
    {       
        if (leftController.isGrabbed)
        {
            // Shooting

            float triggerValue = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
            currentLeftWeapon.triggerValue = triggerValue;
            if(triggerValue > 0f)
            {
                if (currentLeftWeapon.maxAmmo != 0)
                {
                    leftAmmoDisplay.localScale = new Vector3(1f, (float)currentLeftWeapon.currentAmmo / (float)currentLeftWeapon.maxAmmo, 1f);
                }
            }

            // Weapon Switch

            if (OVRInput.GetDown(OVRInput.Button.Three))
            {
                currentCollLeftWeapon--;
                if (currentCollLeftWeapon < 0)
                {
                    currentCollLeftWeapon = collectedLeftWeapons.Count - 1;
                }
                ChangeToWeapon(collectedLeftWeapons[currentCollLeftWeapon], -1);


            }
            if (OVRInput.GetDown(OVRInput.Button.Four))
            {
                currentCollLeftWeapon++;
                if (currentCollLeftWeapon >= collectedLeftWeapons.Count)
                {
                    currentCollLeftWeapon = 0;
                }
                ChangeToWeapon(collectedLeftWeapons[currentCollLeftWeapon], -1);
            }
           
        }
        if (rightController.isGrabbed)
        {
            // Shooting

            float triggerValue = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
            currentRightWeapon.triggerValue = triggerValue;
            if (triggerValue > 0f)
            {
                if (currentRightWeapon.maxAmmo != 0)
                {
                    rightAmmoDisplay.localScale = new Vector3(1f, (float)currentRightWeapon.currentAmmo / (float)currentRightWeapon.maxAmmo, 1f);
                }
            }

            // Weapon Switch

            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                currentCollRightWeapon--;
                if (currentCollRightWeapon < 0)
                {
                    currentCollRightWeapon = collectedRightWeapons.Count - 1;
                }
                ChangeToWeapon(collectedRightWeapons[currentCollRightWeapon], 1);


            }
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                currentCollRightWeapon++;
                if (currentCollRightWeapon >= collectedRightWeapons.Count)
                {
                    currentCollRightWeapon = 0;
                }
                ChangeToWeapon(collectedRightWeapons[currentCollRightWeapon], 1);
            }
        }

        

    }

    // change to weapon with Index index on leftOrRight hand
    void ChangeToWeapon(int index, int leftOrRight)
    {
        switch (leftOrRight)
        {
            case -1:
                currentLeftWeapon = leftWeapons[index];
                for (int i = 0; i < leftWeapons.Length; i++)
                {
                    leftWeapons[i].gameObject.SetActive(false);
                }
                currentLeftWeapon.gameObject.SetActive(true);
                currentCollLeftWeapon = collectedLeftWeapons.IndexOf(index);
                if (currentLeftWeapon.currentAmmo != 0)
                {
                    leftAmmoDisplay.localScale = new Vector3(1f, (float)currentLeftWeapon.currentAmmo / (float)currentLeftWeapon.maxAmmo, 1f);
                }
                else
                {
                    leftAmmoDisplay.localScale = new Vector3(1f, 0f, 1f);
                }
                break;

            case 1:
                currentRightWeapon = rightWeapons[index];
                for (int i = 0; i < rightWeapons.Length; i++)
                {
                    rightWeapons[i].gameObject.SetActive(false);
                }
                currentRightWeapon.gameObject.SetActive(true);
                currentCollRightWeapon = collectedRightWeapons.IndexOf(index);
                if (currentRightWeapon.currentAmmo != 0)
                {
                    rightAmmoDisplay.localScale = new Vector3(1f, (float)currentRightWeapon.currentAmmo / (float)currentRightWeapon.maxAmmo, 1f);
                }
                else
                {
                    rightAmmoDisplay.localScale = new Vector3(1f, 0f, 1f);
                }
                break;
        }
    }

    // pick up weapon with Index weaponIndex with leftOrRight hand
    public void PickUpWeapon(int weaponIndex, int leftOrRight)
    {
        switch (leftOrRight)
        {
            case -1:
                leftWeapons[weaponIndex].currentAmmo = leftWeapons[weaponIndex].maxAmmo;
                if (!collectedLeftWeapons.Contains(weaponIndex))
                {                                   
                    collectedLeftWeapons.Add(weaponIndex);
                }
                ChangeToWeapon(weaponIndex, leftOrRight);

                break;

            case 1:
                rightWeapons[weaponIndex].currentAmmo = rightWeapons[weaponIndex].maxAmmo;
                if (!collectedRightWeapons.Contains(weaponIndex))
                {
                    collectedRightWeapons.Add(weaponIndex);
                }
                ChangeToWeapon(weaponIndex, leftOrRight);
                break;
        }
    }

    // change to grabber with leftOrRightHand
    public void ChangeToGrabber(int leftOrRight)
    {       
        switch (leftOrRight)
        {
            case -1:
                collectedLeftWeapons.Remove(collectedLeftWeapons[currentCollLeftWeapon]);                
                break;

            case 1:
                collectedRightWeapons.Remove(collectedRightWeapons[currentCollRightWeapon]);
                break;
        }
        ChangeToWeapon(0, leftOrRight);

    }    

    // Controller vibrations
    public void WeaponShootVibrate(float frequency, float amplitude, float duration, int side)
    {
        StartCoroutine(Haptics(frequency, amplitude, duration, side));
    }

    IEnumerator Haptics(float frequency, float amplitude, float duration, int side)
    {
        if (side == 1)
        {
            OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        }
        else
        {
            OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
        }

        yield return new WaitForSeconds(duration);

        if (side == 1)
        {
            OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
        }
        else
        {
            OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.LTouch);
        }
    }
}
