using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecro251 : Player
{
    [Header("Jumping")]
    private bool isGrounded;
    private bool wasOnGround;
    private float jumpTimer;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform feetPos;
    [SerializeField] private Vector2 feetDetectorSize;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpDelay = 0.25f;
    [SerializeField] private float jumpChargingTime;
    private float jumpChargingTimer;

    [Header("Attack")]
    private bool isShooting = false;
    [SerializeField] private float attackTime;
    [SerializeField] private Bullet251 speedUpBullet;

    void Update()
    {
        if(isActive)
        {
            if (isAbleToMove)
            {
                moveInput = Input.GetAxisRaw("Horizontal");
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
                if (rigidBody.velocity.y > 0f && Input.GetButton("Jump"))
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
                    if (rigidBody.gravityScale < minGravity)
                    {
                        rigidBody.gravityScale = minGravity;
                    }
                }
                if (Input.GetButtonUp("Jump"))
                {
                    jumpChargingTimer = 0f;
                    rigidBody.gravityScale = gravity;
                }
                if (!isGrounded)
                {
                    anim.SetBool("isJumping", true);
                    anim.SetFloat("jumpCoef", 1);
                }
                else
                {
                    anim.SetBool("isJumping", false);
                    anim.SetBool("landingMoment", false);
                    anim.SetFloat("jumpCoef", 0);
                }
                if (rigidBody.velocity.y <= 0 && !isGrounded)
                {
                    anim.SetBool("landingMoment", true);
                    AudioManager.instance.Play(8);
                }
                if(Input.GetKeyDown(KeyCode.O))
                {
                    StartCoroutine(Shoot(BulletType.SlowDown));
                }
                else if (Input.GetKeyDown(KeyCode.P))
                {
                    StartCoroutine(Shoot(BulletType.SpeedUp));
                }
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
                if (rigidBody.gravityScale > maxGravity)
                    rigidBody.gravityScale = maxGravity;
            }
        }
    }

    public void Jump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpTimer = 0;
        StartCoroutine(JumpSqueeze(0.8f, 1.15f, 0.05f));
    }

    public override void DisableAbility()
    {
        isAbilityActivated = false;
    }

    protected override void Move()
    {
        float targetSpeed = moveInput * moveSpeed;
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

    protected override void StopMoving()
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isJumping", false);
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
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(ledgePos1, ledgePos2);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(feetPos.position, radius);
        Gizmos.color = Color.magenta;
        //Gizmos.DrawRay(transform.position, transform.right);
        Gizmos.DrawWireCube(feetPos.position, feetDetectorSize);
    }

    private IEnumerator Shoot(BulletType bulletType)
    {
        isAbleToMove = false;
        anim.SetFloat("speedCoef", (int)bulletType);
        anim.SetTrigger("shoot");
        rigidBody.velocity = Vector2.zero;
        if (!isGrounded)
        {
            rigidBody.gravityScale = 0;
        }
        yield return new WaitForSeconds(attackTime);
        if(bulletType == BulletType.SpeedUp)
        {
            speedUpBullet.gameObject.SetActive(true);
            speedUpBullet.Shoot(isFacingRight ? 1 : -1);
        }
        rigidBody.gravityScale = 4;
        isAbleToMove = true;
    }
}
