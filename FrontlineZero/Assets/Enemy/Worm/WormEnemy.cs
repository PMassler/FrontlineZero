using UnityEngine;

public class WormEnemy : DestructableUnit
{
    public Transform[] segments;
    public float segmentdistance;

    public WormActivityState state;
    public float ugHeight;
    public float jumpHeight;
    public float jumpForce;
    public float moevementForce;

    public int updateWait;
    int updateCounter;

    public Rigidbody rb;

    MC_EditInterface mcEdit;

    Transform target;

    public float tunnelRadius;
    public float attackDistance;
    public float gravity;

    public int surfaceParticleIndex;
    public int diggingSFXIndex;
    ParticleManager particleManager;

    public float surfaceCheckHeight;

    public LayerMask mcTerrainMask;

    public int surfaceHillStrength;

    public AudioSource attackAS;

    public float spinSpeed;
    public float spinOffset;

    public float heightOverTarget;
    public float distanceFromTarget;

    public AIWeapon aiWeapon;

    float UGValue;

    public enum WormActivityState
    {
        UGTarget,  AttackJump, FleeJump, Falling
    }

    private void Start()
    {
        mcEdit = MC_EditInterface.Instance;
        target = Player.Instance.transform;
        particleManager = ParticleManager.Instance;
        state = WormActivityState.Falling;
    }

    void FixedUpdate()
    {        
        //Worm segments position and rotation
        for (int i = 0; i < segments.Length; i++)
        {
            if (i == 0)
            {
                segments[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, rb.velocity);
            }
            else
            {
                segments[i].position = segments[i - 1].position + (segments[i].position - segments[i - 1].position).normalized * segmentdistance;
                segments[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, segments[i - 1].transform.position - segments[i].transform.position);
            }
        }

        //Activity
        if (!rb.isKinematic && target != null)
        {
            UGValue = mcEdit.CheckSphere(rb.position, tunnelRadius * 2);

            // behaviour management
            switch (state)
            {
                case WormActivityState.UGTarget:
                    UGTargetMovemenet();
                    break;
                case WormActivityState.AttackJump:
                    AttackJumpMovement();
                    break;
                case WormActivityState.FleeJump:
                    FleeJumpMovement();
                    break;
                case WormActivityState.Falling:
                    FallingMovement();
                    break;
            }           

            if (updateCounter >= updateWait)
            {
                // Dig tunnel
                updateCounter = 0;
                if (UGValue > 0.2f)
                {
                    mcEdit.ModifySphere(rb.position, tunnelRadius, 100f, 0f);
                    SFXManager.Instance.PlaySFX(diggingSFXIndex, rb.position);
                }

                // create small hills on surface
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(rb.position.x, surfaceCheckHeight, rb.position.z), -Vector3.up, out hit, 200f, mcTerrainMask) && hit.point.y > rb.position.y)
                {
                    mcEdit.ModifySphere(hit.point, tunnelRadius, surfaceHillStrength, 1f);
                    particleManager.PlayParticleSystem(surfaceParticleIndex, hit.point, Vector3.up);
                }
            }
            updateCounter++;
        }
    }

    void UGTargetMovemenet()
    {
        // move towards target on set height
        Vector3 movementVec = (target.position - rb.position).normalized;
        movementVec.y = Mathf.Sign(ugHeight - rb.position.y);
        rb.AddForce(movementVec * moevementForce, ForceMode.VelocityChange);
        if (Vector2.Distance(new Vector2(rb.position.x, rb.position.z), new Vector2(target.position.x, target.position.z)) < attackDistance)
        {
            SwitchToAttackJump();
        }

        if (UGValue < 0.1f)
        {
            SwitchToFleeJump();
        }
    }

    void AttackJumpMovement()
    {
        // jump above surface, so that on the way down target can be shot at
        Vector3 targetPoint = target.position + Vector3.up * heightOverTarget + Vector3.ProjectOnPlane(transform.position - target.position, Vector3.up).normalized * distanceFromTarget;
        rb.AddForce((targetPoint - rb.position).normalized * jumpForce, ForceMode.Acceleration);
        if (rb.position.y > target.position.y)
        {
            aiWeapon.isActive = true;
            attackAS.Play();
            SwitchToFalling();
        }
    }

    void FleeJumpMovement()
    {
        // jump away from target
        Vector3 fleeVec = -(target.position - rb.position).normalized;
        fleeVec.y = Mathf.Sign(jumpHeight - rb.position.y);
        rb.AddForce(fleeVec * jumpForce, ForceMode.Acceleration);
        if (rb.position.y > jumpHeight)
        {
            SwitchToFalling();
        }
    }

    void FallingMovement()
    {
        rb.AddForce(Vector3.up * -gravity, ForceMode.Acceleration);
        if (UGValue > 0.8f)
        {
            SwitchToUGTarget();
        }
    }

    void SwitchToUGTarget()
    {
        aiWeapon.isActive = false;
        state = WormActivityState.UGTarget;
    }

    void SwitchToAttackJump()
    {
        state = WormActivityState.AttackJump;
    }
    void SwitchToFleeJump()
    {
        state = WormActivityState.FleeJump;
    }
    void SwitchToFalling()
    {
        state = WormActivityState.Falling;
    }


    public override void Die()
    {
        canTakeDamage = false;
        aiWeapon.isActive = false;
        RoundManager.Instance.AddKill();
        Destruct(rb.velocity);
        VFXManager.Instance.PlayVFX(DieVFXIndex, rb.position, Vector3.up);
        SFXManager.Instance.PlaySFX(DieSFXIndex, rb.position);
        Destroy(gameObject);
    }
}
