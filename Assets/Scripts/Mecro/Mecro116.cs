using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecro116 : Player
{
    [Header("Jumping")]
    private bool wasOnGround;
    private float jumpTimer;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform feetPos;
    [SerializeField] private Vector2 feetDetectorSize;
    [SerializeField] private float jumpDelay = 0.25f;

    [SerializeField] private Vector3 invertedDifference1, invertedDifference2, invertedDifference3, invertedDifference4;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(feetPos.position, feetDetectorSize);
    }

    protected override void Move()
    {
        float targetSpeed = moveInput * moveSpeed * velocityCoef;
        float accelerate;

        if (Mathf.Abs(rigidBody.velocity.x) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(rigidBody.velocity.x) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f && !isGrounded)
        {
            accelerate = 0;
        }
        else if (isGrounded)
        {
            accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount : runDeccelerationAmount;
        }
        else
        {
            accelerate = (Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount : runDeccelerationAmount) * accelerationInAir;
        }

        float moveForce = (targetSpeed - rigidBody.velocity.x) * accelerate;
        rigidBody.AddForce(moveForce * Vector2.right, ForceMode2D.Force);
    }

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
                    rigidBody.gravityScale = gravity * (isGravityInverted ? -1f : 1f);
                    StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f));
                }
                if (isGrounded && Input.GetButtonDown("Jump"))
                {
                    jumpTimer = Time.time + jumpDelay;
                }
                if ((rigidBody.velocity.y < 0f && isGravityInverted || rigidBody.velocity.y > 0f && !isGravityInverted) && Input.GetButton("Jump"))
                {
                    if (rigidBody.gravityScale > minGravity && isGravityInverted)
                    {
                        rigidBody.gravityScale = -minGravity;
                    }
                    else if (rigidBody.gravityScale < minGravity && !isGravityInverted)
                    {
                        rigidBody.gravityScale = minGravity;
                    }
                }

                if (Input.GetButtonUp("Jump"))
                {
                    GravityJump();
                }

                UpdateMovementAnimation();
            }
            playerLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
            UpdateLedegGrabbing();
            if (isTouchingLedge)
            {
                isGrounded = false;
            }

            CheckVisability();
            ChangeVelocity();
        }

        if ((moveInput == 0 || !isGrounded) && AudioManager.instance.sounds[9].source.isPlaying)
        {
            AudioManager.instance.Stop(9);
        }
    }


    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
            if (jumpTimer > Time.time)
            {
                jumpTimer -= Time.deltaTime;
            }
            if (jumpTimer <= Time.time && jumpTimer > 0f)
            {
                Jump();
            }
            if (!isGrounded)
            {
                if (isGravityInverted)
                {
                    if (rigidBody.velocity.y > 0 && rigidBody.gravityScale > -maxGravity)
                        rigidBody.gravityScale *= gravityMultiplier;
                    if (rigidBody.gravityScale < -maxGravity)
                        rigidBody.gravityScale = -maxGravity;
                }
                else
                {
                    if (rigidBody.velocity.y < 0 && rigidBody.gravityScale < maxGravity)
                        rigidBody.gravityScale *= gravityMultiplier;
                    if (rigidBody.gravityScale > maxGravity)
                        rigidBody.gravityScale = maxGravity;
                }
            }
        }
    }

    private void UpdateMovementAnimation()
    {
        anim.SetBool("isMoving", moveInput != 0f);
        if (!isGrounded)
        {
            anim.SetBool("isJumping", true);
        }
        else
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("landingMoment", false);
        }
        if ((rigidBody.velocity.y >= 0 && isGravityInverted || rigidBody.velocity.y <= 0 && !isGravityInverted) && !isGrounded)
        {
            anim.SetBool("landingMoment", true);
            AudioManager.instance.Play(8);
        }
        if (moveInput != 0f && !AudioManager.instance.sounds[9].source.isPlaying)
        {
            AudioManager.instance.Play(9);
        }
        Flip();
    }

    private void Flip()
    {
        if (moveInput > 0f)
        {
            isFacingRight = true;
        }
        else if (moveInput < 0f)
        {
            isFacingRight = false;
        }

        transform.localRotation = Quaternion.Euler(isGravityInverted ? 180 : 0, isFacingRight ? 0 : 180, 0);
    }

    private void GravityJump()
    {
        isGravityInverted = !isGravityInverted;
        InvertLedgeDifferencies();
        rigidBody.gravityScale = gravity * (isGravityInverted ? -1f : 1f);
        //feetPos.localPosition = new Vector3(feetPos.localPosition.x, -feetPos.localPosition.y, feetPos.localPosition.z);
    }

    public void Jump()
    {
        jumpTimer = 0;
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce((isGravityInverted ? Vector2.up : Vector2.down) * jumpForce, ForceMode2D.Impulse);
        StartCoroutine(JumpSqueeze(0.8f, 1.15f, 0.05f));
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

    private void InvertLedgeDifferencies()
    {
        (difference1, invertedDifference1) = (invertedDifference1, difference1);
        (difference2, invertedDifference2) = (invertedDifference2, difference2);
        (difference3, invertedDifference3) = (invertedDifference3, difference3);
        (difference4, invertedDifference4) = (invertedDifference4, difference4);
    }

    protected override void StopMoving()
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("landingMoment", false);
        anim.SetBool("isLedgeGrabbing", false);
        rigidBody.velocity = Vector2.zero;
        AudioManager.instance.Stop(9);
    }

    public override void DisableAbility()
    {
        if (isGravityInverted)
        {
            InvertLedgeDifferencies();
        }
        isGravityInverted = false;
        isAbilityActivated = false;
        rigidBody.gravityScale = gravity;
        UpdateMovementAnimation();
    }

    protected override IEnumerator TurnLedgeDetectorOff()
    {
        ledgeDecetror.enabled = false;
        yield return new WaitForSeconds(ledgeCancelTime);
        ledgeDecetror.enabled = true;
    }

    public override void InvertGravity()
    {
        GravityJump();
    }
}
