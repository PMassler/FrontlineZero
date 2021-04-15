using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [HideInInspector]
    public float triggerValue;
    public Animator animator;
    public float coolDown;
    [HideInInspector]
    public float lastShotTime;
    public Rigidbody handRB;
    [Range(0f,1f)]
    public float spread;
    public bool usesAmmo = true;
    public int maxAmmo;
    [HideInInspector]
    public int currentAmmo;
    public UnityEvent emptyAmmo;
    public HandRB mechHandScript;


    // Returns the shooting direction affected by spread
    public Vector3 GetShootingVector()
    {
        if(spread == 0f)
        {
            return transform.forward;
        }
        else
        {
            return transform.forward + transform.right * Random.Range(-spread, spread) + transform.up * Random.Range(-spread, spread);
        }
    }

    // use up one ammo and if ammo is empty trigger emptyAmmo event
    public void AmmoCheck()
    {
        if (usesAmmo)
        {
            if (currentAmmo > 0)
            {
                currentAmmo--;
            }
            else
            {
                emptyAmmo.Invoke();
            }
        }
    }
}
