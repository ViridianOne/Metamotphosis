using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Player : MonoBehaviour
{
    public static Player instance;

    public GameObject holder;
    private SpriteRenderer holderSprite;

    protected Animator anim;
    protected bool isFacingRight = true;

    protected Collider2D playerCollider;
    [SerializeField] float respawnTime;
    private float respawnTimer;
    [HideInInspector] public Transform respawnPoint;
    protected bool isActive;
    [HideInInspector] public bool isAbilityActivated = false;
    public bool isOnMovingPlatform = false;
    [HideInInspector] public bool isOnArcPlatform, isOn30, isOn60, isOn90 = false;
    [HideInInspector] public bool isOn0 = true;
    [HideInInspector] public int ceilCoef = 1;
    [HideInInspector] public bool isVertical = false;
    [HideInInspector] public bool enableVelocityRight, enableVelocityLeft = false;

    [Header("Physics")]
    protected bool isGrounded;
    protected Rigidbody2D rigidBody;
    protected float gravity;
    [SerializeField] protected float minGravity;
    [SerializeField] protected float maxGravity;
    [SerializeField] protected float gravityMultiplier;
    [SerializeField] protected float gravityAddition;
    [SerializeField] protected float moveSpeed;
    protected float movementForce;
    protected float moveInput;
    [SerializeField] protected float maxSpeed;
    protected bool isAbleToMove = true;
    [SerializeField] protected float runAcceleration;
    [SerializeField] protected float runDecceleration;
    [SerializeField] protected float runAccelerationAmount;
    [SerializeField] protected float runDeccelerationAmount;
    [SerializeField] protected float accelerationInAir;
    protected bool isInverted = false;

    [Header("Ledge Grabbing")]
    [HideInInspector] public bool isTouchingLedge;
    protected bool canClimbLedge;
    protected bool ledgeDetected;
    private Vector2 ledgePos1, ledgePos2;
    private float ledgeGrabbingTimer;
    [SerializeField] private float ledgeGrabbingTime;
    [SerializeField] protected Vector3 difference1, difference2, difference3, difference4;
    public Vector2 movingPlatDif = Vector2.zero;
    protected bool isClimbing = false;
    [SerializeField] protected Collider2D ledgeDecetror;
    [SerializeField] protected float ledgeCancelTime;

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        holderSprite = holder.GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        gravity = rigidBody.gravityScale;
        isActive = true;
        anim.SetBool("isLedgeGrabbing", false);
        anim.SetBool("isMoving", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("landingMoment", false);
        anim.SetBool("isFlying", false);
    }

    protected abstract void Move();

    protected void UpdateLedegGrabbing() 
    {
        if (isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
        }
        if (ledgeDetected && !canClimbLedge)
        {
            rigidBody.gravityScale = gravity * (isInverted ? -1 : 1);
            canClimbLedge = true;
            anim.SetBool("isGrabbed", true);
        }
        if (canClimbLedge)
        {
            if (isOnMovingPlatform)
                ledgePos1 -= movingPlatDif;
            transform.position = ledgePos1;
            if(Input.GetButtonDown("Jump"))
            {
                isClimbing = true;
            }
            else if(Input.GetButtonDown("Fire3"))
            {
                CancelLedegeGrabbing();
            }
            if (isClimbing)
            {
                anim.SetBool("isLedgeGrabbing", true);
                anim.SetBool("isGrabbed", false);
                if (ledgeGrabbingTimer <= 0)
                    ledgeGrabbingTimer = ledgeGrabbingTime;
                else
                {
                    ledgeGrabbingTimer -= Time.deltaTime;
                }
                if (ledgeGrabbingTimer <= 0)
                    FinishLedgeGrabbing();
            }
        }
    }

    protected void CancelLedegeGrabbing()
    {
        canClimbLedge = false;
        isClimbing = false;
        movingPlatDif = Vector2.zero;
        rigidBody.velocity = Vector2.zero;
        isOnMovingPlatform = false;
        ledgeDetected = false;
        isAbleToMove = true;
        isTouchingLedge = false;
        anim.SetBool("isGrabbed", false);
        StartCoroutine(TurnLedgeDetectorOff());
    }

    protected abstract IEnumerator TurnLedgeDetectorOff();

    private void FinishLedgeGrabbing()
    {
        canClimbLedge = false;
        isClimbing = false;
        transform.position = ledgePos2;
        movingPlatDif = Vector2.zero;
        //ledgeFlag = false;
        rigidBody.velocity = Vector2.zero;
        isOnMovingPlatform = false;
        ledgeDetected = false;
        isAbleToMove = true;
        isTouchingLedge = false;
        anim.SetBool("isLedgeGrabbing", false);
    }

    public void GrabLedge(Vector3 grabPos, bool isRight, float coefficient)
    {
        isAbleToMove = false;
        if (!isRight && !isFacingRight || isRight && isFacingRight)
        {
            if (!isRight)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
        isTouchingLedge = true;
        if (!isRight)
        {
            ledgePos1 = grabPos - difference1;
            ledgePos2 = grabPos + difference2;
        }
        else
        {
            ledgePos1 = grabPos - difference3;
            ledgePos2 = grabPos + difference4;
        }
    }

    public void MiniJump(float miniJumpForce) 
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce((isInverted ? Vector2.down : Vector2.up) * miniJumpForce, ForceMode2D.Impulse);
    }

    public void DamagePlayer()
    {
        AudioManager.instance.Play(3);
        if (canClimbLedge)
        {
            CancelLedegeGrabbing();
        }
        anim.SetBool("isDamaged", true);
        anim.SetTrigger("damage");
        isActive = false;
        playerCollider.enabled = false;
        MiniJump(12f);
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        rigidBody.gravityScale = 4 * (isInverted ? -1 : 1);
        yield return new WaitForSeconds(0.3f);
        rigidBody.gravityScale = 0;
        yield return new WaitForSeconds(respawnTime - 0.3f);
        if (!isActive)
        {
            rigidBody.gravityScale = gravity;
            transform.position = respawnPoint.position;
            rigidBody.velocity = Vector2.zero;
            isActive = true;
            isInverted = false;
            playerCollider.enabled = true;
            anim.SetBool("isDamaged", false);
        }
    }

    protected abstract void StopMoving();

    public void ToggleActive(bool state)
    {
        isActive = state;
        /*anim.SetBool("isMoving", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("landingMoment", false);
        anim.SetBool("isFlying", false);*/
        anim.SetBool("landingMoment", true);
        anim.SetBool("landingMoment", false);
        if (!state)
        {
            StopMoving();
        }
    }

    protected void CheckVisability()
    {
        if(!holderSprite.enabled && isActive)
        {
            rigidBody.gravityScale = gravity;
            transform.position = respawnPoint.position;
            isActive = true;
            playerCollider.enabled = true;
            anim.SetBool("isDamaged", false);
        }
    }

    public abstract void DisableAbility();

    private void OnValidate()
    {
        runAccelerationAmount = (50 * runAcceleration) / maxSpeed;
        runDeccelerationAmount = (50 * runDecceleration) / maxSpeed;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, maxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, maxSpeed);
    }

    public Vector2 GetVelocity()
    {
        return rigidBody.velocity;
    }

    public void AddJumpForce(float jumpForceCoef)
    {
        rigidBody.AddForce(Vector2.up * jumpForceCoef, ForceMode2D.Impulse);
    }

    public bool IsGrounded { get => isGrounded; }
}
