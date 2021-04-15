using UnityEngine;

public class Unit : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int DieVFXIndex = 6;
    public int DieSFXIndex = 0;
    public MeshRenderer[] damageMatMesh;
    bool isStartingPeriod = true;    
    public bool canTakeDamage;
    float startTime;
    [HideInInspector]
    public MonoBehaviour lastDamageSource;

    // initialize health and damage shaders
    void Awake()
    {
        currentHealth = maxHealth;
        for (int i = 0; i < damageMatMesh.Length; i++)
        {
            damageMatMesh[i].material.SetFloat("DamageTaken", 0f);
        }
    }

    void Start()
    {
        startTime = Time.time;
    }

    // immunity during starting period, so arenaobjects are not getting destroyed at the start of game
    private void LateUpdate()
    {
        if (isStartingPeriod)
        {
            if (Time.time > startTime + 5f)
            {
                canTakeDamage = true;
                isStartingPeriod = false;
            }
        }
        if(lastDamageSource != null)
        {
            lastDamageSource = null;
        }
    }

    // change healtvalue
    public virtual void ModifyHealth(int value)
    {        
        if (canTakeDamage)
        {
            currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);

            for (int i = 0; i < damageMatMesh.Length; i++)
            {
                damageMatMesh[i].material.SetFloat("DamageTaken", 1f - (float)currentHealth / (float)maxHealth);
            }
            if (currentHealth == 0)
            {
                Die();
            }
        }

    }

    // damageSource parameter prevents taking damgage more than one time if multiple colliders are hit by explosion
    public virtual void ModifyHealth(int value, MonoBehaviour damageSource)
    {
        if (canTakeDamage)
        {
            if (damageSource != lastDamageSource)
            {
                currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);

                for (int i = 0; i < damageMatMesh.Length; i++)
                {
                    damageMatMesh[i].material.SetFloat("DamageTaken", 1f - (float)currentHealth / (float)maxHealth);
                }
                if (currentHealth == 0)
                {
                    Die();
                }

                lastDamageSource = damageSource;
            }
        }
    }    

    public virtual void Die()
    {
        VFXManager.Instance.PlayVFX(DieVFXIndex, transform.position, Vector3.up);
        SFXManager.Instance.PlaySFX(DieSFXIndex, transform.position);
        Destroy(gameObject);
    }     
}
