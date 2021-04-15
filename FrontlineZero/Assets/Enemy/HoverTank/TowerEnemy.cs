using UnityEngine;

public class TowerEnemy : DestructableUnit
{
    Transform target;

    
    public LayerMask targetMask;

    public ExplodingObject fortressExObj;

    public float aimRandomness;

    public float floorCheckDistance;
    bool fortessBuilt = true;
    public LayerMask mcTerrain;

    public Rigidbody rb;

    public Transform[] boosters;
    public RotationController rotPID;
    public float hoverDistance;
    public float hoverForce;

    public Transform COM;
    public Transform engine;

    public HoverTankState state = HoverTankState.Idle;

    
    public float timeToMoveAfterNotSeeingPlayer;

    public float movementDirectionChangeTime;
    public float movementDirectionChangeRandomness;
    Vector3 idleMovementDirection;
    public float movementForce;
    float lastMovementChangeTime;
    float lastMovementChangeCoolDown;
    public float navTolerance;
    public Animator animator;

    public AIWeapon aiWeapon;

    public int coverBuildSFXIndex;

    Vector2 lastCoverPos = Vector2.zero;
    
    public Transform thrusters;

    public enum HoverTankState
    {
        Idle, Burried
    }

    void Start()
    {
        if (Player.Instance != null)
        {
            target = Player.Instance.transform;
        }
        rb.centerOfMass = COM.localPosition;
    }


    public void Hover()
    {       
        // force from thrusters
        for (int i = 0; i < boosters.Length; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(boosters[i].transform.position, transform.TransformDirection(Vector3.down),out hit, hoverDistance))
            {
                rb.AddForceAtPosition(Time.fixedDeltaTime * transform.TransformDirection(Vector3.up) * Mathf.Pow(hoverDistance - hit.distance, 2) / hoverDistance * hoverForce, boosters[i].transform.position);
            }
        }
        rb.AddForce(-Time.fixedDeltaTime * transform.TransformVector(Vector3.right) * transform.InverseTransformVector(rb.velocity).x * 5f);

        // additional balance
        rb.AddTorque(rotPID.getAngularAcceleration(transform.rotation,rb.angularVelocity, Quaternion.identity));        
    }

    public void ToTargetMovement() 
    { 
        // occasionally update direction
        if(Time.time > lastMovementChangeTime + lastMovementChangeCoolDown)
        {           
            if (target != null)
            {
                idleMovementDirection = NavManager.Instance.GetMovementVector(transform.position, 20f, 10, 1f, navTolerance, 6f, target.position);
            }          

            lastMovementChangeTime = Time.time;
            lastMovementChangeCoolDown = movementDirectionChangeTime + Random.Range(0f, movementDirectionChangeRandomness);
        }
        // move in direction
        rb.AddForce(movementForce * idleMovementDirection);
    }

    private void FixedUpdate()
    {
        // behaviour management
        switch (state)
        {
            case HoverTankState.Idle:
                Hover();
                ToTargetMovement();
                if (aiWeapon.CheckLineOfSight())
                {
                    fortessBuilt = false;
                    state = HoverTankState.Burried;
                    animator.SetBool("isExtended", true);
                    Invoke("SetWeaponActive", 2f);
                    thrusters.gameObject.SetActive(false);
                }
                break;
            case HoverTankState.Burried:                
                if(Time.time > aiWeapon.lastSeenPlayer + timeToMoveAfterNotSeeingPlayer)
                {
                    aiWeapon.isActive = false;
                    state = HoverTankState.Idle;
                    animator.SetBool("isExtended", false);
                    thrusters.gameObject.SetActive(true);
                }
                break;
        }
    }

    void Update()
    {         
        // if able to build cover, and last cover is far enough away, build cover
        if (!fortessBuilt && Physics.Raycast(transform.position,-Vector3.up,floorCheckDistance, mcTerrain) && Vector2.Distance(new Vector2(transform.position.x, transform.position.z),lastCoverPos) > 15f)
        {           
            BuildCover();
        }                             
    }

   void SetWeaponActive()
    {
        aiWeapon.isActive = true;
    }

    [ContextMenu("BuildFortress")]
    public void BuildCover()
    {
        lastCoverPos = new Vector2(transform.position.x, transform.position.z);
        fortessBuilt = true;
        SFXManager.Instance.PlaySFX(coverBuildSFXIndex,transform.position);
        fortressExObj.Explode(transform.position,Quaternion.identity);
    }

    public override void Die()
    {
        canTakeDamage = false;
        aiWeapon.isActive = false;
        RoundManager.Instance.AddKill();
        Destruct(rb.velocity);
        base.Die();
    }
}
