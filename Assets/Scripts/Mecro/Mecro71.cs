using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecro71 : Player
{
    [Header("Jumping")]
    //[SerializeField] private Transform feetPos;
    [SerializeField] private float radius;
    [SerializeField] private Vector2 feetDetectorSize;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpDelay = 0.25f;
    //public GameObject holder;
    public float acceleration;
    public float deceleration;
    public bool isFlying;
    public int jumpCount = 0;
    public float defaultMoveSpeed; //equal to starting MoveSpeed
    public Directions directionArrow;
    private float rotationSpeed = 720;
    public bool directionChosen = false;
    public bool started_flying = false;
    public Vector2 direction;

    private float xMoveInput;
    private float yMoveInput;
    private bool isCeilingHit;
    [SerializeField] private float accelerationCoefficient;

    protected override void Move()
    {
        bool isHorizontal = directionArrow == Directions.right || directionArrow == Directions.left;
        bool isVertical = directionArrow == Directions.up || directionArrow == Directions.down;

        float targetSpeed = moveInput * moveSpeed * velocityCoef;
        float accelerate = 0;
        if (isGrounded)
            accelerate = Mathf.Abs(targetSpeed) > 1f ? runAccelerationAmount : runDeccelerationAmount;
        else
            accelerate = Mathf.Abs(targetSpeed) > 1f ? runAccelerationAmount * accelerationInAir : runDeccelerationAmount * accelerationInAir;

        if (isHorizontal && Mathf.Abs(rigidBody.velocity.x) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(rigidBody.velocity.x) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f && !isGrounded 
            || isVertical && Mathf.Abs(rigidBody.velocity.y) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(rigidBody.velocity.y) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f && !isGrounded)
        {
            accelerate = 0;
        }

        float moveForce = 0;
        if(isHorizontal)
            moveForce = (targetSpeed - rigidBody.velocity.x) * accelerate;
        else if (isVertical)
            moveForce = (targetSpeed - rigidBody.velocity.y) * accelerate;
        rigidBody.AddForce(moveForce * direction, ForceMode2D.Force);
    }

    private void ChooseDirection()
    {
        xMoveInput = Input.GetAxisRaw("Horizontal") * (isMovementInverted ? -1f : 1f);
        yMoveInput = Input.GetAxisRaw("Vertical") * (isGravityInverted || isMovementInverted ? -1f : 1f);
        if (xMoveInput > 0)
        {
            directionArrow = Directions.right;
            moveInput = xMoveInput;
            directionChosen = true;
            direction = Vector2.right;
            AudioManager.instance.Play(16);
        }
        if (xMoveInput < 0)
        {
            directionArrow = Directions.left;
            moveInput = xMoveInput;
            directionChosen = true;
            direction = Vector2.right;
            AudioManager.instance.Play(16);
        }
        if (yMoveInput < 0)
        {
            directionArrow = Directions.down;
            moveInput = yMoveInput;
            directionChosen = true;
            direction = Vector2.up;
            AudioManager.instance.Play(16);
        }
        if (yMoveInput > 0)
        {
            directionArrow = Directions.up;
            moveInput = yMoveInput;
            directionChosen = true;
            direction = Vector2.up;
            AudioManager.instance.Play(16);
        }
    }


    private void Update()
    {
        //if (jumpCount == 0) with space
        if (isActive)
        {
            if (isAbleToMove)
            {
                if (directionChosen == false && rigidBody.velocity == Vector2.zero)
                    ChooseDirection();

                UpdateMovementAnimation();

                var damageCount = 0;
                isCeilingHit = Physics2D.OverlapBox(headPos.position, headDetectorSize, 0f, groundMask);     //for collision detection
                if (isCeilingHit && !ledgeDetected)
                {
                    //Player.instance.StartCoroutine(Respawn());
                    StopFlying();
                    DamagePlayer();
                    damageCount = 1;
                }
                if (ledgeDetected)
                {
                    StopFlying();
                }

                //Starting flying right after arrow (with directionChosen)
                if (directionChosen)
                {
                    isFlying = true;
                    moveSpeed = maxSpeed;
                    //started_flying = true;
                }
                if (Input.GetButtonDown("Jump") || damageCount == 1)
                {
                    directionChosen = false;
                    isFlying = false;
                    moveSpeed = 0;
                    AudioManager.instance.Stop(16);
                }
            }
            playerLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
            UpdateLedegGrabbing();
            if (isTouchingLedge)
                isGrounded = false;
            CheckVisability();
            ChangeVelocity();
        }
        else
        {
            StopFlying();
        }
    }

    private void FixedUpdate()
    {
        if (isFlying)
        {
            anim.SetBool("isFlying", true);
        }
        else
        {
            anim.SetBool("isFlying", false);
        }
        if (Mathf.Abs(rigidBody.velocity.x) < 0.5f && Mathf.Abs(rigidBody.velocity.y) < 0.5f)
            rigidBody.velocity = Vector2.zero;
        Move();
    }

    private void StopFlying()
    {
        isFlying = false;
        directionArrow = Directions.none;
        directionChosen = false;
        AudioManager.instance.Stop(16);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(ledgePos1, ledgePos2);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(feetPos.position, radius);
        Gizmos.color = Color.magenta;
        //Gizmos.DrawRay(transform.position, transform.right);
        //Gizmos.DrawWireCube(feetPos.position, feetDetectorSize);
    }

    private void UpdateMovementAnimation()
    {
        if (isFlying)
            anim.SetBool("isFlying", true);
        else
            anim.SetBool("isFlying", false);
        Flip();
    }

    private void Flip()
    {
        if (moveInput > 0f)
        {
            transform.localRotation = directionArrow == Directions.up && isFlying ? Quaternion.Euler(0, 180, 90) : Quaternion.Euler(0, 0, 0);
            isFacingRight = true;
        }
        else if (moveInput < 0f)
        {
            transform.localRotation = directionArrow == Directions.down && isFlying ? Quaternion.Euler(0, 180, -90) : Quaternion.Euler(0, 180, 0);
            isFacingRight = false;
        }
    }

    protected override void StopMoving()
    {
        isFlying = false;
        directionChosen = false;
        anim.SetBool("isFlying", false);
        anim.SetBool("isLedgeGrabbing", false);
        rigidBody.velocity = Vector2.zero;
        AudioManager.instance.Stop(16);
    }

    public override void DisableAbility() 
    {
        isAbilityActivated = false;
    }

    protected override IEnumerator TurnLedgeDetectorOff()
    {
        if(transform.localRotation.y == 0)
            transform.position -= new Vector3(1f, 0, 0);
        else
            transform.position += new Vector3(1f, 0, 0);
        ledgeDecetror.enabled = false;
        yield return new WaitForSeconds(ledgeCancelTime);
        ledgeDecetror.enabled = true;
    }

    public override void InvertGravity()
    {
        isGravityInverted = true;
        StartCoroutine(WaitAndInvertGravity());
    }

    private IEnumerator WaitAndInvertGravity()
    {
        yield return new WaitForSeconds(4f);
        isGravityInverted = false;
    }
}
