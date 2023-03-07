using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecro341 : Player
{
    [Header("Jumping")]
    private bool isGrounded;
    [SerializeField] private Transform wheelPos;
    [SerializeField] private Vector2 wheelDetectorSize;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Vector3 rotationDirection;
    [SerializeField] private Vector2 horizontalGravityDelta, verticalGravityDelta;
    [SerializeField] private float counteractingForce, originalForce;
    private float force = 0;
    private float spriteAngle = 0;

    void Update()
    {
        if(isActive)
        {
            if(isAbleToMove)
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
                        spriteAngle = 60;
                    }
                    else
                    {
                        force = originalForce;
                        spriteAngle = 0;
                    }
                }
                if(!isOnArcPlatform)
                {
                    force = originalForce;
                    spriteAngle = 0;
                }
                //if (!isOnArcPlatform || (isOn30 && !isOn60))
                //    moveInput = Input.GetAxisRaw("Horizontal");
                if (isOn60)
                {
                    moveInput = Input.GetAxisRaw("Vertical");
                    transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 90);
                    holder.transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else
                {
                    moveInput = Input.GetAxisRaw("Horizontal");
                    transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 0);
                    holder.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                anim.SetFloat("Rotation", spriteAngle);

                if(!isOnArcPlatform || (isOn30 && !isOn60))
                    rigidBody.velocity = new Vector2(rigidBody.velocity.x, originalForce);
                else if(isOn60)
                    rigidBody.velocity = new Vector2(counteractingForce, moveInput == 0 ? counteractingForce : rigidBody.velocity.y);

                isGrounded = Physics2D.OverlapBox(wheelPos.position, wheelDetectorSize, 0f, groundMask);
                UpdateMovementAnimation();
            }
            UpdateLedegGrabbing();
            if (isTouchingLedge)
            {
                isGrounded = false;
            }
            CheckVisability();
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
        if (moveInput > 0f)
        {
            if(isOn60)
                transform.localRotation = Quaternion.Euler(0, 0, 90);
            else
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            //transform.localScale = new Vector3(1f, 1f, 1f);
            isFacingRight = true;
        }
        else if (moveInput < 0f)
        {
            if(isOn60)
                transform.localRotation = Quaternion.Euler(180, 0, 90);
            else
                transform.localRotation = Quaternion.Euler(0, 180, 0);
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
        lightSwitcher = false;
    }

    protected override void Move()
    {
        float targetSpeed = moveInput * moveSpeed;
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
        if (isOn60)
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
        Gizmos.DrawWireCube(wheelPos.position, wheelDetectorSize);
    }
}
