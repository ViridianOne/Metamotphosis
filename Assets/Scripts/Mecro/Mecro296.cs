using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecro296 : Player
{
    [Header("Jumping")]
    private bool wasOnGround;
    private bool isCeilingHitted; 
    [SerializeField] private Transform feetPos;
    [SerializeField] private Vector2 feetDetectorSize;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask noPlatfromMask;
    //public GameObject holder;

    private bool isChargingJump = false;
    private bool isJumpCharged = false;
    private float jumpTimer;
    [SerializeField] private float jumpForce;
    [SerializeField] private float timeBetweenJump;
    [SerializeField] private float minJumpTime;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float jumpChargeSpeed;
    private float betweenJumpTimer = 0f;
    [SerializeField] private JumpBarBehaviour jumpBarBehaviour;
    [SerializeField] private float jumpBarChangeSpeed;

    private void Update() 
    {
        if (isActive)
        {
            if (isAbleToMove)
            {
                moveInput = Input.GetAxisRaw("Horizontal") * (isMovementInverted ? -1f : 1f);

                wasOnGround = isGrounded;
                isGrounded = Physics2D.OverlapBox(feetPos.position, feetDetectorSize, 0f, groundMask);
                if (!wasOnGround && isGrounded)
                {
                    rigidBody.gravityScale = gravity;
                    isCeilingHitted = false;
                    betweenJumpTimer = timeBetweenJump;
                    jumpTimer = 0f;
                    isJumpCharged = false;
                    StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f));
                }

                if (!isCeilingHitted)
                {
                    isCeilingHitted = Physics2D.OverlapBox(headPos.position, headDetectorSize, 0f, noPlatfromMask);
                    if (isCeilingHitted)
                        jumpTimer = 0;
                }

                isChargingJump = Input.GetButton("Jump");
                if(Input.GetButtonDown("Jump"))
                    AudioManager.instance.Play(7);
                if (isGrounded && jumpTimer < maxJumpTime && isChargingJump)
                {
                    jumpTimer += jumpChargeSpeed * Time.deltaTime; 
                    jumpBarBehaviour.AddValueToJumpSlider((maxJumpTime - minJumpTime) * jumpChargeSpeed * jumpBarChangeSpeed * Time.deltaTime);
                }
                if (isGrounded && Input.GetButtonUp("Jump"))
                {
                    isJumpCharged = true;
                    jumpBarBehaviour.AddValueToJumpSlider(-1000);
                    AudioManager.instance.Stop(7);
                }

                if (isGrounded && isChargingJump && jumpTimer > 0f)
                    jumpBarBehaviour.ToggleJumpSlider(true);
                else
                {
                    jumpBarBehaviour.ToggleJumpSlider(false);
                }

                UpdateMovementAnimation();
                CheckCeilingTouch();
            }
            playerLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
            UpdateLedegGrabbing();
            if (isTouchingLedge)
            {
                isGrounded = false;
                AudioManager.instance.Stop(7);
            }
            CheckVisability();
            ChangeVelocity();
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            if ((betweenJumpTimer < 0f || isJumpCharged) && !isChargingJump)
            {
                if (!isJumpCharged)
                {
                    jumpTimer = minJumpTime;
                    isJumpCharged = true;
                }

                if (jumpTimer > 0f)
                {
                    Jump();
                    jumpTimer -= Time.deltaTime;
                }
            }
            betweenJumpTimer -= Time.deltaTime;
            Move();
            if (!isGrounded)
            {
                if (rigidBody.velocity.y < 0 && rigidBody.gravityScale < maxGravity)
                    rigidBody.gravityScale *= gravityMultiplier;
                if (rigidBody.gravityScale > maxGravity)
                    rigidBody.gravityScale = maxGravity;
            }
        }
    }

    private void UpdateMovementAnimation()
    {
        anim.SetBool("isListening", false);

        anim.SetFloat("xVelocity", Mathf.Abs(rigidBody.velocity.x));
        anim.SetFloat("yVelocity", rigidBody.velocity.y);

        if (isGrounded)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("landingMoment", false);
        }
        else
            anim.SetBool("isJumping", true);
        if (!wasOnGround && isGrounded)
            anim.SetBool("landingMoment", true);

        anim.SetBool("isCeilingHitted", isCeilingHitted);

        anim.SetBool("isMoving", Mathf.Abs(moveInput) > 0f);

        Flip();
    }

    private void Flip()
    {
        if (moveInput > 0f)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            isFacingRight = true;
        }
        else if (moveInput < 0f)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            isFacingRight = false;
        }
    }

    protected override void Move()
    {
        float targetSpeed = moveInput * moveSpeed * velocityCoef;
        float accelerate = 0; 
        
        if(!isGrounded)
            accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount * accelerationInAir : runDeccelerationAmount * accelerationInAir;


        if (Mathf.Abs(rigidBody.velocity.x) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(rigidBody.velocity.x) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f && !isGrounded || isGrounded)
        {
            accelerate = 0;
        }

        float moveForce = (targetSpeed - rigidBody.velocity.x) * accelerate;
        rigidBody.AddForce(moveForce * Vector2.right, ForceMode2D.Force);
        if (isGrounded)
            rigidBody.velocity *= Vector2.up;

        // Version 1
        //if (isCeilingHitted && rigidBody.velocity.y > 0)
        //    rigidBody.velocity = new Vector2(0, 0);
        //else
        //    rigidBody.velocity = new Vector2(isGrounded ? 0 : moveInput * moveSpeed, rigidBody.velocity.y);
    }

    public void Jump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        StartCoroutine(JumpSqueeze(0.8f, 1.15f, 0.05f));
        AudioManager.instance.Play(5);
    }

    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originSize.z);
        float timer = 0f;
        while (timer <= 1f)
        {
            timer += Time.deltaTime / seconds;
            holder.transform.localScale = Vector3.Lerp(originSize, newSize, timer);
            yield return null;
        }
        timer = 0;
        while (timer <= 1f)
        {
            timer += Time.deltaTime / seconds;
            holder.transform.localScale = Vector3.Lerp(newSize, originSize, timer);
            yield return null;
        }
    }

    protected override void StopMoving()
    {
        //anim.SetFloat("xVelocity", 0f);
        //anim.SetFloat("yVelocity", 0f);
        //anim.SetBool("isMoving", false);
        //anim.SetBool("isCeilingHitted", false);
        //anim.SetBool("isLedgeGrabbing", false);
        //anim.SetBool("isJumping", false);
        //anim.SetBool("landingMoment", false);

        anim.SetTrigger("listen");
        anim.SetBool("isListening", true);
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
    }

    public override void DisableAbility() 
    {
        isAbilityActivated = false;
    }

    protected override IEnumerator TurnLedgeDetectorOff()
    {
        ledgeDecetror.enabled = false;
        yield return new WaitForSeconds(ledgeCancelTime);
        ledgeDecetror.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(feetPos.position, feetDetectorSize);
    }
}
