using UnityEngine;

public class AndroidEnemy : DestructableUnit
{
    public bool isDummy;

    Transform target;
    Vector3 movementDirection;
    Vector3 facingDirection;
    public AIWeapon aiWeapon;
    public MechPhysics mechScript;

    float lastMovementChangeTime;
    float lastMovementChangeCoolDown;

    public float movementDirectionChangeTime;
    public float movementDirectionChangeRandomness;

    Vector3 originalPos;
    Vector3 originalForward;

    public float navTolerance;

    public AndroidMovementState state = AndroidMovementState.Idle;
    public enum AndroidMovementState
    {
        Idle, Attacking
    }

    void Start()
    {
        // set target
        if (Player.Instance != null)
        {
            target = Player.Instance.transform;
        }
        // set original pos for respawn if dummy
        originalPos = transform.position;
        originalForward = transform.forward;
    }

    void Update()
    {
        if (!isDummy)
        {
            // behaviour management
            switch (state)
            {
                case AndroidMovementState.Idle:
                    ToTargetMovement();
                    if (aiWeapon.CheckLineOfSight())
                    {
                        aiWeapon.isActive = true;
                        state = AndroidMovementState.Attacking;
                    }
                    break;
                case AndroidMovementState.Attacking:
                    AttackingMovement();
                    if (!aiWeapon.CheckLineOfSight())
                    {
                        aiWeapon.isActive = false;
                        state = AndroidMovementState.Idle;
                    }
                    break;
            }
        }
    }

    // move towards target, face in movement direction
    void ToTargetMovement()
    {
        if (target != null)
        {
            if (Time.time > lastMovementChangeTime + lastMovementChangeCoolDown)
            {
                movementDirection = NavManager.Instance.GetMovementVector(transform.position, 20f, 10, 1f, navTolerance, 6f, target.position);
                lastMovementChangeTime = Time.time;
                lastMovementChangeCoolDown = movementDirectionChangeTime + Random.Range(0f, movementDirectionChangeRandomness);
            }
            facingDirection = movementDirection;
        }
        Move();     
    }

    // move randomly, always face target
    void AttackingMovement()
    {
        if (Time.time > lastMovementChangeTime + lastMovementChangeCoolDown)
        {
            movementDirection = NavManager.Instance.GetMovementVector(transform.position, 20f, 10, 1f, 4f, 6f);
            lastMovementChangeTime = Time.time;
            lastMovementChangeCoolDown = movementDirectionChangeTime + Random.Range(0f, movementDirectionChangeRandomness);
        }
        if (target != null)
        {
            facingDirection = target.position - transform.position;
        }
        Move();
    }

    // set movement input for MechPhysics script
   void Move()
    {
        float vertInput = Vector3.Dot(movementDirection,transform.forward);
        float horiInput = Vector3.Dot(movementDirection, transform.right);
        float rotInput = Mathf.Clamp(Vector3.SignedAngle(transform.forward,facingDirection,Vector3.up)/180f,-1f,1f);

        mechScript.verticalInput = vertInput;
        mechScript.horizontalInput = horiInput;
        mechScript.rotInput = rotInput;
    }

    public override void Die()
    {
        canTakeDamage = false;
        aiWeapon.isActive = false;
        if (isDummy)
        {
            SimulationManager.Instance.SpawnDummy(originalPos, Quaternion.LookRotation(originalForward));
        }
        RoundManager.Instance.AddKill();
        Destruct(GetComponent<Rigidbody>().velocity);
        base.Die();
    }    
}
