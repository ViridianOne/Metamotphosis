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

    //[Header("PLatform Pull Up")]
    //[SerializeField] private Vector2 ledgePlatPos1, ledgePlatPos2;
    //[SerializeField] private Vector2 diffPlat1, diffPlat2, diffPlat3, diffPlat4;
    //[SerializeField] private float platformGrabbingTime;
    //private float platformGrabbingTimer;

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
                    isOn90 = false;
                    isOn0 = false;
                }
                if(!isOnArcPlatform)
                {
                    force = originalForce;
                    spriteAngle = 0;
                    isOn90 = isOn60;
                    isOn0 = !isGrounded || isOn30;
                }
                //if (!isOnArcPlatform || (isOn30 && !isOn60))
                //    moveInput = Input.GetAxisRaw("Horizontal");
                if (isOn60 || isOn90)
                {
                    rigidBody.gravityScale = 0;
                    moveInput = Input.GetAxisRaw("Vertical") * (isMovementInverted ? -1f : 1f);
                    //transform.rotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 90 * ceilCoef);
                    //if(isOn90)
                    //    holder.transform.rotation = Quaternion.Euler(0, transform.rotation.y, 90 * ceilCoef);
                    //else if (isOn60)
                    //    holder.transform.rotation = Quaternion.Euler(0, transform.rotation.y, ceilCoef > 0 ? 0 : 180);
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
                    //transform.rotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, ceilCoef == 1 ? 0 : 180);
                    //holder.transform.rotation = Quaternion.Euler(0, transform.rotation.y, ceilCoef == 1 ? 0 : 180);
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
            UpdateLedegGrabbing();
            //UpdatePlatformGrabbing();
            if (isTouchingLedge)
            {
                isGrounded = false;
            }
            CheckVisability();
            ChangeVelocity();
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
        if (moveInput != 0f && !AudioManager.instance.sounds[9].source.isPlaying)
        {
            AudioManager.instance.Play(9);
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
            //transform.localScale = new Vector3(1f, 1f, 1f);
            isFacingRight = true;
        }
        else if (moveInput * ceilCoef < 0f)
        {
            if(isOn60 || isOn90)
                transform.rotation = Quaternion.Euler(180, 0, 90 * ceilCoef);
            else
                transform.rotation = Quaternion.Euler(0, 180, ceilCoef == 1 ? 0 : 180);
            //transform.localScale = new Vector3(-1f, 1f, 1f);
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
        isOn0 = true;
        isOn30 = false;
        isOn60 = false;
        isOn90 = false;
        gravity = 4;
        transform.rotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 0);
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
        //if (!isOnArcPlatform || (isOn30 && !isOn60))
        //{
        //    moveForce = (targetSpeed - rigidBody.velocity.x) * accelerate;
        //    rigidBody.AddForce(moveForce * Vector2.right, ForceMode2D.Force);
        //}
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
        AudioManager.instance.Stop(9);
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
        //Gizmos.DrawWireCube(wheelPos.position, wheelDetectorSize);
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
        isGravityInverted = !isGravityInverted;
    }
}
