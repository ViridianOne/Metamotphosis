using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecro341 : Player
{
    [Header("Physics")]
    private bool isOnNonMetalGround;
    [SerializeField] private Transform wheelPos;
    [SerializeField] private float wheelRadius;
    [SerializeField] private Vector2 wheelDetectorSize;
    [SerializeField] private LayerMask nonMetalMask;
    [SerializeField] private Vector3 rotationDirection;
    [SerializeField] private Vector2 horizontalGravityDelta, verticalGravityDelta;
    [SerializeField] private float counteractingForce, originalForce;
    private float force = 0;
    private float spriteAngle = 0;
    private float xGravity;
    public float deffectTime;

    void Update()
    {
        if(isActive)
        {
            if (isAbleToMove)
            {
                if (isOnArcPlatform)
                {
                    if(isOn30)
                    {
                        force = originalForce;
                        spriteAngle = 30;
                    }
                    else if(isOn60)
                    {
                        force = counteractingForce;
                        spriteAngle = 30;
                    }
                    else
                    {
                        force = originalForce;
                        spriteAngle = 0;
                    }
                    rigidBody.gravityScale = 0;
                    isOn90 = false;
                    isOn0 = false;
                }
                else
                {
                    force = originalForce;
                    spriteAngle = 0;
                    isOn90 = isOn60;
                    isOn0 = !isGrounded || isOn30;
                }
                if (isOn60 || isOn90)
                {
                    rigidBody.gravityScale = 0;
                    moveInput = Input.GetAxisRaw("Vertical") * (isMovementInverted ? -1f : 1f);;
                    if(ceilCoef > 0)
                        xGravity = rigidBody.velocity.x <= counteractingForce ? counteractingForce : 5;
                    else
                        xGravity = rigidBody.velocity.x >= counteractingForce * ceilCoef ? counteractingForce : 4;
                    rigidBody.velocity = new Vector2(xGravity * ceilCoef, enableVelocityRight || enableVelocityLeft ? counteractingForce * moveInput * -3: rigidBody.velocity.y);
                }
                else
                {
                    rigidBody.gravityScale = 4 * ceilCoef * (isGravityInverted ? -1f : 1f);
                    moveInput = Input.GetAxisRaw("Horizontal") * (isMovementInverted ? -1f : 1f);
                    rigidBody.velocity = new Vector2(enableVelocityRight || enableVelocityLeft ? counteractingForce * moveInput * -3 : rigidBody.velocity.x, rigidBody.velocity.y);
                }
                if (moveInput == 0 && !isOn0)
                {
                    rigidBody.velocity = Vector2.zero;
                }
                anim.SetFloat("Rotation", spriteAngle);
                isGrounded = Physics2D.OverlapCircle(wheelPos.position, wheelRadius, groundMask);
                isOnNonMetalGround = Physics2D.OverlapCircle(wheelPos.position, wheelRadius, nonMetalMask) & moveInput != 0;
                UpdateMovementAnimation();
                CheckCeilingTouch();
                if (isOnNonMetalGround)
                {
                    StartCoroutine(TakeDeffect());
                }
            }
            playerLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
            UpdateLedegGrabbing();
            if (isTouchingLedge)
            {
                isGrounded = false;
            }
            CheckVisability();
            ChangeVelocity();
            if ((moveInput == 0 || !isGrounded) && AudioManager.instance.sounds[28].source.isPlaying)
            {
                AudioManager.instance.Stop(28);
            }
        }
    }

    private void UpdateMovementAnimation()
    {
        if (moveInput > 0f)
        {
            anim.SetBool("isMoving", true);
        }
        else if (moveInput < 0f)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
        if (moveInput != 0f && !AudioManager.instance.sounds[28].source.isPlaying)
        {
            AudioManager.instance.Play(28);
        }
        anim.SetBool("isFalling", !isGrounded);
        Flip();
    }

    private void Flip()
    {
        if (moveInput * ceilCoef > 0f)
        {
            if(isOn60 || isOn90)
                transform.rotation = Quaternion.Euler(0, 0, 90 * ceilCoef);
            else
                transform.rotation = Quaternion.Euler(0, 0, ceilCoef == 1 ? 0 : 180);
            isFacingRight = true;
        }
        else if (moveInput * ceilCoef < 0f)
        {
            if(isOn60 || isOn90)
                transform.rotation = Quaternion.Euler(180, 0, 90 * ceilCoef);
            else
                transform.rotation = Quaternion.Euler(0, 180, ceilCoef == 1 ? 0 : 180);
            isFacingRight = false;
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
        }
    }

    public override void DisableAbility()
    {
        isAbilityActivated = false;
        isOnArcPlatform = false;
        isOn0 = true;
        isOn30 = false;
        isOn60 = false;
        isOn90 = false;
        gravity = 4;
        transform.rotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 0);
        isGravityInverted = false;
        isVertical = false;
        ceilCoef = 1;
    }

    protected override void Move()
    {
        float targetSpeed = moveInput * moveSpeed * velocityCoef;
        float accelerate = 0;

        if (isGrounded)
            accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount : runDeccelerationAmount;
        else
            accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount * accelerationInAir : runDeccelerationAmount * accelerationInAir;

        if (force == originalForce && Mathf.Abs(rigidBody.velocity.x) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(rigidBody.velocity.x) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f && !isGrounded
            || force == counteractingForce && Mathf.Abs(rigidBody.velocity.y) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(rigidBody.velocity.y) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f && !isGrounded)
        {
            accelerate = 0;
        }

        float moveForce = 0;
        if (isOn60 || isOn90)
        {
            moveForce = (targetSpeed - rigidBody.velocity.y) * accelerate;
            rigidBody.AddForce(moveForce * Vector2.up, ForceMode2D.Force);
        }
        else
        {
            moveForce = (targetSpeed - rigidBody.velocity.x) * accelerate;
            rigidBody.AddForce(moveForce * Vector2.right, ForceMode2D.Force);
        }
    }

    protected override void StopMoving()
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("landingMoment", false);
        anim.SetBool("isLedgeGrabbing", false);
        rigidBody.velocity = Vector2.zero;
        AudioManager.instance.Stop(28);
    }

    protected override IEnumerator TurnLedgeDetectorOff()
    {
        ledgeDecetror.enabled = false;
        yield return new WaitForSeconds(ledgeCancelTime);
        ledgeDecetror.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(wheelPos.position, wheelRadius);
    }

    private IEnumerator TakeDeffect()
    {
        isActive = false;
        rigidBody.velocity = Vector2.zero;
        moveInput = 0;
        anim.SetTrigger("deffect");
        yield return new WaitForSeconds(deffectTime);
        isActive = true;
    }

    public override void InvertGravity()
    {
        isGravityInverted = false;
    }
}
