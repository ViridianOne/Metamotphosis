using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecro206 : Player
{
    [SerializeField] private Transform feetPos;
    [SerializeField] private Vector2 feetDetectorSize;
    private bool wasOnGround;

    public float grav;


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
        else
        {
            accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount : runDeccelerationAmount;
            if (!isGrounded)
            {
                accelerate *= accelerationInAir;
            }
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
                    }
                    else
                    {
                        anim.SetLayerWeight(1, 100);
                        anim.SetLayerWeight(2, 0);
                        AudioManager.instance.Play(2);
                    }
                    Physics2D.IgnoreLayerCollision(7, 9, isAbilityActivated);
                    VisibilityManager.instance.ToggleVisibility(isAbilityActivated);
                }

                wasOnGround = isGrounded;
                isGrounded = Physics2D.OverlapBox(feetPos.position, feetDetectorSize, 0f, groundMask);
                if (!wasOnGround && isGrounded)
                {
                    rigidBody.gravityScale = gravity;
                    StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f));
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

        UpdateAudio();
    }

    private void UpdateMovementAnimation()
    {
        anim.SetBool("isMoving", moveInput != 0f);
        anim.SetBool("isFalling", !isGrounded);
        Flip();
    }

    private void UpdateAudio()
    {
        //if (rigidBody.velocity.y <= 0 && !isGrounded)
        //{
        //    AudioManager.instance.Play(8);  // звук приземления
        //}
        //if (moveInput != 0f && !AudioManager.instance.sounds[9].source.isPlaying)
        //{
        //    AudioManager.instance.Play(9);  // звук движения
        //}
        //if ((moveInput == 0 || !isGrounded) && AudioManager.instance.sounds[9].source.isPlaying)
        //{
        //    AudioManager.instance.Stop(9);
        //}
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
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
        anim.SetBool("isMoving", false);
        anim.SetBool("isFalling", false);
        anim.SetBool("isLedgeGrabbing", false);
        rigidBody.velocity = Vector2.zero;
        //AudioManager.instance.Stop(9);
    }

    public override void DisableAbility()
    {
        isAbilityActivated = false;
        VisibilityManager.instance.ToggleVisibility(false);
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
