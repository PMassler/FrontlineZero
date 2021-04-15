using System.Collections;
using UnityEngine;

public class SuicideEnemy : DestructableUnit
{
    Transform target;
    public float speed;
    public float triggerRadius;
    public float triggerInterval;
    public int triggerValue;
   
    public ExplodingObject exObj;
    public Rigidbody rb;

    public bool isTriggered;

    public Transform model;

    public Animator animator;

    public int triggerSFXIndex;

    public GameObject shrapnelPrefab;
    public int shrapnelCount;
    [Range(0f, 1f)]
    public float shrapnelSpread;
    public float shrapnelSpawnRadius;

    public float unstuckCooldown;
    public float jumpForce;
   

    Vector3 lastUnstuckCheckPos;

    public AudioSource alarmAS;    

    void Start()
    {
        target = Player.Instance.transform;
        StartCoroutine(StuckCheck());        
    }

    void Update()
    {
        // rotate towards movement direction
        if(rb.isKinematic == false && rb.velocity != Vector3.zero)
        {
            model.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    void FixedUpdate()
    {
        // move towards target, when in range trigger explosion countdown
        if (target != null)
        {
            rb.AddForce((target.position - transform.position).normalized * speed, ForceMode.VelocityChange);
            if (!isTriggered && Vector3.Distance(target.position, transform.position) < triggerRadius)
            {
                alarmAS.Play();
                SFXManager.Instance.PlaySFX(triggerSFXIndex, transform.position);
                isTriggered = true;
                animator.SetBool("isTriggered", true);
                StartCoroutine(TriggerSuicide());
            }
        }
    }

    public override void Die()
    {
        canTakeDamage = false;
        RoundManager.Instance.AddKill();
        Destruct(rb.velocity);
        SpawnShrapnel();
        exObj.Explode();        
        base.Die();
    }

    // occasionally check if stuck, if yes jump up
    IEnumerator StuckCheck()
    {
        while (true)
        {
            if(Vector3.Distance(transform.position, lastUnstuckCheckPos) < 5f)
            {
                UnStuck();
            }
            lastUnstuckCheckPos = transform.position;
            yield return new WaitForSeconds(unstuckCooldown);
        }        
    }

    void UnStuck()
    {
        rb.AddForce(Vector3.up * jumpForce);
    }

    public IEnumerator TriggerSuicide()
    {
        while (true)
        {
            ModifyHealth(-triggerValue);
            yield return new WaitForSeconds(triggerInterval);
        }
    }

    void SpawnShrapnel()
    {
        for (int i = 0; i < shrapnelCount; i++)
        {
            Instantiate(shrapnelPrefab, transform.position + (Vector3.right * Random.Range(-shrapnelSpread, shrapnelSpread) + Vector3.forward * Random.Range(-shrapnelSpread, shrapnelSpread)) * shrapnelSpawnRadius, Quaternion.identity);
        }
    }
}
