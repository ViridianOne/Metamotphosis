using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Player : MonoBehaviour
{
    public static Player instance;

    public GameObject holder;
    protected SpriteRenderer holderSprite;

    protected Animator anim;
    protected bool isFacingRight = true;

    protected Collider2D playerCollider;
    [SerializeField] float respawnTime;
    private float respawnTimer;
    [HideInInspector] public Transform respawnPoint;
    public bool isActive { get; protected set; }
    [HideInInspector] public bool isAbilityActivated = false;
    [HideInInspector] public bool isOnArcPlatform, isOn30, isOn60, isOn90 = false;
    [HideInInspector] public bool isOn0 = true;
    [HideInInspector] public int ceilCoef = 1;
    [HideInInspector] public bool isVertical = false;
    [HideInInspector] public bool enableVelocityRight, enableVelocityLeft = false;

    public bool isInBossRoom = false;

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
    protected bool isGravityInverted = false;
    protected bool isMovementInverted = false;
    [SerializeField] protected Transform headPos;
    [SerializeField] protected Vector2 headDetectorSize;
    [SerializeField] protected LayerMask groundMask;
    [HideInInspector] protected float velocityCoef;
    [HideInInspector] protected float velocityChangeTime;

    [Header("Ledge Grabbing")]
    [HideInInspector] public bool isTouchingLedge;
    protected bool canClimbLedge;
    protected bool ledgeDetected;
    private Vector2 ledgePos1, ledgePos2;
    private float ledgeGrabbingTimer;
    [SerializeField] private float ledgeGrabbingTime;
    [SerializeField] protected Vector3 difference1, difference2, difference3, difference4;
    protected bool isClimbing = false;
    [SerializeField] protected Collider2D ledgeDecetror;
    [SerializeField] protected float ledgeCancelTime;
    private bool isOnPlatformLedge;

    [Header("Rendering")]
    [SerializeField] protected Light2D playerLight;
    [SerializeField] protected float lightInnerRadius, lightOuterRadius;

    //public Color defaultColor;
    //private SpriteRenderer currentColor;

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
        Physics2D.IgnoreLayerCollision(6, 7, false);
        gravity = rigidBody.gravityScale;
        isActive = true;
        playerLight.pointLightInnerRadius = lightInnerRadius;
        playerLight.pointLightOuterRadius = lightOuterRadius;
        playerLight.intensity = 0;
        //currentColor = holder.GetComponent<SpriteRenderer>();
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
            rigidBody.gravityScale = gravity * (isGravityInverted ? -1f : 1f);
            canClimbLedge = true;
            anim.SetBool("isGrabbed", true);
        }
        if (canClimbLedge)
        {
            if (isOnPlatformLedge)
                transform.localPosition = ledgePos1;
            else
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
        rigidBody.velocity = Vector2.zero;
        ledgeDetected = false;
        isAbleToMove = true;
        isTouchingLedge = false;
        anim.SetBool("isGrabbed", false);
        if(isOnPlatformLedge)
        {
            transform.SetParent(null);
            isOnPlatformLedge = false;
        }
        StartCoroutine(TurnLedgeDetectorOff());
    }

    protected abstract IEnumerator TurnLedgeDetectorOff();

    private void FinishLedgeGrabbing()
    {
        canClimbLedge = false;
        isClimbing = false;
        if (isOnPlatformLedge)
            transform.localPosition = ledgePos2;
        else
            transform.position = ledgePos2;
        rigidBody.velocity = Vector2.zero;
        ledgeDetected = false;
        isAbleToMove = true;
        isTouchingLedge = false;
        anim.SetBool("isLedgeGrabbing", false);
    }

    public void GrabLedge(Vector3 grabPos, bool isRight, float coefficient, bool isOnPlatform = false)
    {
        isAbleToMove = false;
        isOnPlatformLedge = isOnPlatform;
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
        rigidBody.AddForce((isGravityInverted ? Vector2.down : Vector2.up) * miniJumpForce, ForceMode2D.Impulse);
    }

    public void DamagePlayer()
    {
        AudioManager.instance.Play(3);
        if (canClimbLedge)
        {
            CancelLedegeGrabbing();
        }
        isActive = false;
        if (transform.parent != null && transform.parent.tag == "Platform")
        {
            transform.SetParent(null);
        }
        anim.SetBool("isDamaged", true);
        anim.SetBool("isMoving", false);
        anim.SetTrigger("damage");
        DisableAbility();
        Physics2D.IgnoreLayerCollision(7, 9, true);
        Physics2D.IgnoreLayerCollision(7, 17, true);
        MiniJump(12f);
        if(gameObject.activeInHierarchy)
            StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        rigidBody.gravityScale = 4 * (isGravityInverted ? -1f : 1f);
        yield return new WaitForSeconds(0.3f);
        rigidBody.gravityScale = 0;
        yield return new WaitForSeconds(respawnTime - 0.3f);
        if (!isActive)
        {
            Physics2D.IgnoreLayerCollision(6, 7, false);
            //instance.GetHolder().GetComponent<SpriteRenderer>().color = defaultColor;
            //currentColor.color = defaultColor;
            if (transform.parent != null && transform.parent.tag == "Platform")
            {
                transform.SetParent(null);
            }
            ceilCoef = 1;
            isVertical = false;
            rigidBody.gravityScale = gravity;
            transform.position = respawnPoint.position;
            rigidBody.velocity = Vector2.zero;
            isActive = true;
            isGravityInverted = false;
            //playerCollider.enabled = true;
            Physics2D.IgnoreLayerCollision(7, 9, false);
            Physics2D.IgnoreLayerCollision(7, 17, false);
            anim.SetBool("isDamaged", false);
            //isInBossRoom = false;
        }
        RoomActiveZone.RecoverEnemies();
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

    public void AddForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        rigidBody.AddForce(force, forceMode);
    }

    public void AddJumpForce(float jumpForceCoef)
    {
        AddForce((isGravityInverted ? Vector2.down : Vector2.up) * jumpForceCoef, ForceMode2D.Impulse);
    }

    public void AddGravityForce(Vector2 gravity)
    {
        AddForce(gravity);
    }

    public bool IsGrounded { get => isGrounded; }

    public bool IsGravityInverted { get => isGravityInverted; }
    public virtual void InvertGravity()
    {
        isGravityInverted = !isGravityInverted;
        rigidBody.gravityScale = -rigidBody.gravityScale;
    }

    protected virtual void CheckCeilingTouch()
    {
        if (isGravityInverted && Physics2D.OverlapBox(headPos.position, headDetectorSize, 0f, groundMask))
        {
            InvertGravity();
        }
    }

    public bool IsMovementInverted { get => isMovementInverted; }
    public void InvertMovement(bool state)
    {
        isMovementInverted = state;
    }

    public void ReactToFlashExplosion(float effectOnPlayerTime, float speedChangeCoef, Color changeColor)
    {
        velocityChangeTime = effectOnPlayerTime;
        velocityCoef = speedChangeCoef;
        anim.speed = speedChangeCoef;
        holderSprite.color = changeColor;
    }

    public IEnumerator ReactToChemical(float effectTime, Color changedColor)
    {
        holderSprite.color = changedColor;
        yield return new WaitForSeconds(effectTime);
        holderSprite.color = new Color(1, 1, 1, 1);
        DamagePlayer();
    }

    protected void ChangeVelocity()
    {
        if (velocityChangeTime > 0)
            velocityChangeTime -= Time.deltaTime;
        else if (velocityChangeTime <= 0 && velocityCoef != 1)
        {
            velocityCoef = 1;
            anim.speed = 1;
            holderSprite.color = new Color(1, 1, 1, 1);
        }
    }

    public GameObject GetHolder()
    {
        return holder;
    }

    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
    }
}
