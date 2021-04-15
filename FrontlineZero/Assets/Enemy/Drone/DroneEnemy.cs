using UnityEngine;

public class DroneEnemy : DestructableUnit
{
    public Weapon weapon;
    public RotationController rotPID;
    public PositionController posPID;
    public Rigidbody rb;

    public ParticleSystem[] normalParticles;
    public ParticleSystem diveParticle;

    Transform target;

    Vector3 movementTargetPos;
    Transform facingTarget;

    public float idleSpeed;
    public float attackSpeed;
    public float diveSpeed;

    public float diveHeight;
    public float diveStartDistance;

    public float diveAttackRange;

    public float attackHeight;
    public float attackRange;

    public float attackDuration;
    public float attackSwitchDirectionTime;


    float attackStartTime;
    float lastMovementSwitchTime;

    public DroneMovementState state = DroneMovementState.DivePrepare;

    public Animator droneAnimator;

    Vector3 attackDir;

    public float weaponAttackCooldown;
    public float weaponDiveCooldown;

    [Range(0f,1f)]
    public float weaponAttackSpread;
    [Range(0f, 1f)]
    public float weaponDiveSpread;

    

    public enum DroneMovementState
    {
        DivePrepare, Diving, AttackPrepare, Attacking
    }


    private void Start()
    {
        if (Player.Instance != null)
        {
            target = Player.Instance.transform;
        }
        ChoseAttack();
    }

    private void Update()
    {
        // behaviour management
        switch (state)
        {
            case DroneMovementState.DivePrepare:

                // when close to dive start position, switch to dive behaviour
                if(Vector3.Distance(transform.position,movementTargetPos)< 5f)
                {
                    SwitchToDive();
                }
                break;
            case DroneMovementState.Diving:

                // when close to target shoot weapon
                if (target != null && Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up).magnitude < diveAttackRange)
                {
                    weapon.triggerValue = 1f;
                }
                else
                {
                    weapon.triggerValue = 0f;
                }

                // when close to dive end position, switch to new attack
                if (Vector3.Distance(transform.position, movementTargetPos) < 5f)
                {
                    weapon.triggerValue = 0f;
                    droneAnimator.SetBool("IsDiving", false);
                    for (int i = 0; i < normalParticles.Length; i++)
                    {
                        normalParticles[i].gameObject.SetActive(true);
                        diveParticle.gameObject.SetActive(false);
                    }
                    ChoseAttack();
                }               
                break;
            case DroneMovementState.AttackPrepare:

                // when close to attack start position, switch to attack behaviour
                if (Vector3.Distance(transform.position, movementTargetPos) < attackRange)
                {
                    droneAnimator.SetBool("IsAttacking", true);
                    SwitchToAttack();
                }
                break;
            case DroneMovementState.Attacking:

                // move randomly while attacking
                if (Time.time > lastMovementSwitchTime + attackSwitchDirectionTime)
                {
                    SetAttackPoint();
                }
                if (target != null)
                {
                    movementTargetPos = target.position + Vector3.up * (attackHeight - target.position.y) + attackDir * attackRange;
                }
               
                // chose new attack after certain time
                if(Time.time > attackStartTime + attackDuration)
                {
                    weapon.triggerValue = 0f;
                    droneAnimator.SetBool("IsAttacking", false);
                    ChoseAttack();
                }
                break;
        }       
    }  

    void ChoseAttack()
    {
        // randomly choose between dive and direct attack
        if (Random.Range(0,4) > 0)
        {
            SwitchToDivePrepare();
        }
        else
        {
            SwitchToAttackPrepare();
        }
    }

    void SwitchToDivePrepare()
    {       
        SetDiveStartPos();
        facingTarget = null;
        posPID.maxVel = idleSpeed;        
        state = DroneMovementState.DivePrepare;
    }

    void SwitchToDive()
    {
        for (int i = 0; i < normalParticles.Length; i++)
        {
            normalParticles[i].gameObject.SetActive(false);
            diveParticle.gameObject.SetActive(true);
        }
        weapon.coolDown = weaponDiveCooldown;
        weapon.spread = weaponDiveSpread;

        if (target != null)
        {
            movementTargetPos = movementTargetPos + 2 * (target.position - movementTargetPos);
            movementTargetPos = NavManager.Instance.PutPosInBounds(movementTargetPos,10f);
        }
        movementTargetPos.y = diveHeight;
        facingTarget = null;
        posPID.maxVel = diveSpeed;
        droneAnimator.SetBool("IsDiving", true);
        state = DroneMovementState.Diving;
    }

    void SwitchToAttackPrepare()
    {        
        facingTarget = null;
        posPID.maxVel = idleSpeed;
        if (target != null)
        {
            movementTargetPos = target.position;
        }
        movementTargetPos.y = attackHeight;
        state = DroneMovementState.AttackPrepare;
    }

    void SwitchToAttack()
    {
        weapon.coolDown = weaponAttackCooldown;
        weapon.spread = weaponAttackSpread;

        if(target != null)
        {
            facingTarget = target;
        }
        posPID.maxVel = attackSpeed;
        attackStartTime = Time.time;
        weapon.triggerValue = 1f;
        SetAttackPoint();
        state = DroneMovementState.Attacking;
    }

    void SetDiveStartPos()
    {
        if (target != null)
        {      
            // set a position far away from the target as dive start position
            Vector3 newPos = NavManager.Instance.GetValidPosOnRadius(target.position,diveStartDistance);
            newPos.y = diveHeight;                
            movementTargetPos = newPos;                     
        }
    }

    void SetAttackPoint()
    {
        attackDir =  Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Vector3.forward;        
        lastMovementSwitchTime = Time.time;
    }

    private void FixedUpdate()
    {
        RotStabilize();
        MoveToTarget();
    }

    void MoveToTarget()
    {
        rb.AddForce(posPID.getAcceleration(transform.position,rb.velocity,movementTargetPos), ForceMode.Acceleration);
    }

    void RotStabilize()
    {
        // if has target, turn towards it, else turn towards movement direction
        if (facingTarget != null)
        {
            rb.AddTorque(rotPID.getAngularAcceleration(transform.rotation,rb.angularVelocity,Quaternion.LookRotation(Vector3.ProjectOnPlane(facingTarget.position - transform.position,Vector3.up))), ForceMode.Acceleration);
        }
        else
        {
            Vector3 velo;
            if(rb.velocity == Vector3.zero)
            {
                velo = transform.forward;
            }
            else
            {
                velo = rb.velocity;
            }
            rb.AddTorque(rotPID.getAngularAcceleration(transform.rotation, rb.angularVelocity, Quaternion.LookRotation(Vector3.ProjectOnPlane(velo, Vector3.up))), ForceMode.Acceleration);
        }
    }

    public override void Die()
    {
        canTakeDamage = false;
        weapon.triggerValue = 0f;
        RoundManager.Instance.AddKill();
        Destruct(rb.velocity);
        base.Die();
    }
}
