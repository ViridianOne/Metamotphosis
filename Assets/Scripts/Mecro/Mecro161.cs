using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecro161 : Player
{
    [Header("Jumping")]
    [SerializeField] private Transform feetPos;
    [SerializeField] private float radius;
    [SerializeField] private Vector2 feetDetectorSize;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpDelay = 0.25f;
    private float jumpTimer;
    private bool wasOnGround;
    [SerializeField] private float jumpChargingTime;
    private float jumpChargingTimer;

    [Header("Rendering")]
    [SerializeField] private float strongInnerRadius, strongOuterRadius;

    protected override void Move()
    {
        float targetSpeed = moveInput * moveSpeed * velocityCoef;
        float accelerate = 0;

        if (isGrounded)
            accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount : runDeccelerationAmount;
        else
            accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount * accelerationInAir : runDeccelerationAmount * accelerationInAir;

        if (Mathf.Abs(rigidBody.velocity.x) > Mathf.Abs(targetSpeed) 
            && Mathf.Sign(rigidBody.velocity.x) == Mathf.Sign(targetSpeed) 
            && Mathf.Abs(targetSpeed) > 0.01f && !isGrounded)
        {
            accelerate = 0;
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
                if (Input.GetButtonDown("Fire1"))
                {
                    isAbilityActivated = !isAbilityActivated;
                    if (isAbilityActivated)
                    {
                        anim.SetLayerWeight(1, 0);
                        anim.SetLayerWeight(2, 100);
                        AudioManager.instance.Play(1);
                        playerLight.pointLightInnerRadius = LevelManager.instance.completedAchievements[0] ? strongInnerRadius + 4 : strongInnerRadius;
                        playerLight.pointLightOuterRadius = LevelManager.instance.completedAchievements[0] ? strongOuterRadius + 5 : strongOuterRadius;
                        LevelManager.instance.UpdateDarkVisionStats();
                    }
                    else
                    {
                        anim.SetLayerWeight(1, 100);
                        anim.SetLayerWeight(2, 0);
                        AudioManager.instance.Play(2);
                        playerLight.pointLightInnerRadius = lightInnerRadius;
                        playerLight.pointLightOuterRadius = lightOuterRadius;
                    }
                }
                wasOnGround = isGrounded;
                isGrounded = Physics2D.OverlapBox(feetPos.position, feetDetectorSize, 0f, groundMask);
                if (!wasOnGround && isGrounded)
                {
                    rigidBody.gravityScale = gravity;
                    StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f));
                }
                if (isGrounded && Input.GetButtonDown("Jump"))
                {
                    jumpTimer = Time.time + jumpDelay;
                    jumpChargingTimer = jumpChargingTime;
                }
                if(rigidBody.velocity.y > 0f && Input.GetButton("Jump"))
                {
                    if (jumpChargingTimer > 0f)
                    {
                        rigidBody.gravityScale -= gravityAddition;
                        jumpChargingTimer -= Time.deltaTime;
                    }
                    else
                    {
                        jumpChargingTimer = 0f;
                    }
                    if(rigidBody.gravityScale < minGravity)
                    {
                        rigidBody.gravityScale = minGravity;
                    }
                }
                if(Input.GetButtonUp("Jump"))
                {
                    jumpChargingTimer = 0f;
                    rigidBody.gravityScale = gravity;
                }
                if (!isGrounded)
                {
                    anim.SetBool("isJumping", true);
                    //runSound.Stop();
                }
                else
                {
                    anim.SetBool("isJumping", false);
                    anim.SetBool("landingMoment", false);
                }
                if (rigidBody.velocity.y <= 0 && !isGrounded)
                {
                    anim.SetBool("landingMoment", true);
                    AudioManager.instance.Play(8);
                }
                UpdateMovementAnimation();
                CheckCeilingTouch();
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
        //else
        //{
        //    lightSwitcher = false;
        //    anim.SetLayerWeight(1, 100);
        //    anim.SetLayerWeight(2, 0);
        //}
        /*if (isActive && !runSound.isPlaying)
        {
            runSound.Play();
        }*/
        if ((moveInput == 0 || !isGrounded) && AudioManager.instance.sounds[9].source.isPlaying)
        {
            AudioManager.instance.Stop(9);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(ledgePos1, ledgePos2);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(feetPos.position, radius);
        Gizmos.color = Color.magenta;
        //Gizmos.DrawRay(transform.position, transform.right);
        Gizmos.DrawWireCube(feetPos.position, feetDetectorSize);
    }

    private void UpdateMovementAnimation()
    {
        if (moveInput > 0f)
        {
            //transform.localScale = new Vector3(1f, 1f, 1f);
            anim.SetBool("isMoving", true);
        }
        else if (moveInput < 0f)
        {
            //transform.localScale = new Vector3(-1f, 1f, 1f);
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
        if (moveInput!=0f && !AudioManager.instance.sounds[9].source.isPlaying)
        {
            AudioManager.instance.Play(9);
        }
        Flip();
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
            if (jumpTimer > Time.time)
                jumpTimer -= Time.deltaTime;
            if (jumpTimer <= Time.time && jumpTimer > 0f)
            {
                Jump();
            }
            if (!isGrounded)
            {
                if (rigidBody.velocity.y < 0 && rigidBody.gravityScale < maxGravity)
                    rigidBody.gravityScale *= gravityMultiplier;
                if(rigidBody.gravityScale > maxGravity)
                    rigidBody.gravityScale = maxGravity;
            }
        }
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

    public void Jump()
    {
        //if (isJumping)
        //{
        //    //rigidBody.velocity = Vector2.up * jumpForce;
        //    rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        //    rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //}
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //isJumping = false;
        jumpTimer = 0;
        StartCoroutine(JumpSqueeze(0.8f, 1.15f, 0.05f));
    }

    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originSize.z);
        float timer = 0f;
        while(timer <= 1f)
        {
            timer += Time.deltaTime / seconds;
            holder.transform.localScale = Vector3.Lerp(originSize, newSize, timer);
            yield return null;
        }
        timer = 0;
        while(timer <= 1f)
        {
            timer += Time.deltaTime / seconds;
            holder.transform.localScale = Vector3.Lerp(newSize, originSize, timer);
            yield return null;
        }
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
        playerLight.pointLightInnerRadius = lightInnerRadius;
        playerLight.pointLightOuterRadius = lightOuterRadius;
        isAbilityActivated = false;
        anim.SetLayerWeight(1, 100);
        anim.SetLayerWeight(2, 0);
    }

    protected override IEnumerator TurnLedgeDetectorOff()
    {
        ledgeDecetror.enabled = false;
        yield return new WaitForSeconds(ledgeCancelTime);
        ledgeDecetror.enabled = true;
    }
}
