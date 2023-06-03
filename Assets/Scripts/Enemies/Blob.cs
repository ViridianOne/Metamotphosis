using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : Enemy
{
    [Header("Physics")]
    [SerializeField] private Transform leftPos, rightPos;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] protected float runAcceleration;
    [SerializeField] protected float runDecceleration;
    [SerializeField] protected float runAccelerationAmount;
    [SerializeField] protected float runDeccelerationAmount;
    private Directions moveDirection = Directions.none;
    private int directionCoef = 0;
    [SerializeField] private float stateChangeTime;
    private float stateChangeTimer;
    [SerializeField] private Transform downGroundPos, forwardGroundPos;
    [SerializeField] private Vector2 downGroundSize, playerDetectorSize, playerDetectorPos;
    public bool isOnGround, isGroundInFront;
    private bool isPlayerNear;

    [Header("Attack")]
    [SerializeField] private float attackTime;
    [SerializeField] private float fullAttackTime;
    private bool canTurnOffTheLights = true;
    [SerializeField] private Transform attackPos1;

    [Header("Damage")]
    private bool isDamaged;
    [SerializeField] private Transform damagePos;
    [SerializeField] private Vector2 damageSize;

    [SerializeField]private bool isGrounded;
    [SerializeField]private bool wasOnGround;
    [SerializeField] private float jumpForce;
    private float betweenJumpTimer = 0f;
    private float jumpTimer;
    [SerializeField] protected float maxGravity;
    [SerializeField] protected float gravityMultiplier;
    [SerializeField] private Transform feetPos;
    [SerializeField] private Vector2 feetDetectorSize;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float timeBetweenJump;
    private bool isJumpCharged = false;
    [SerializeField] private float minJumpTime;
    private bool isChargingJump = false;
    [SerializeField] protected float accelerationInAir;

    private void Update()
    {
        wasOnGround = isGrounded;
        isGrounded = Physics2D.OverlapBox(feetPos.position, feetDetectorSize, 0f, groundMask);
        if (!wasOnGround && isGrounded)
        {
            rigidBody.gravityScale = gravityScale;
            betweenJumpTimer = timeBetweenJump;
            jumpTimer = 0f;
            isJumpCharged = false;
            AudioManager.instance.Play(17);
            StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f));
        }
        if (isActive)
        {
            isPlayerNear = Physics2D.OverlapBox(playerDetectorPos, playerDetectorSize, 0f, masksAbleToDamage);

            //if (isGroundInFront)
            //    jumpForce = 20f;
            //else
            //    jumpForce = 11.5f;

            if (stateChangeTimer <= 0)
            {
                state = (EnemyState)Random.Range(0, 2);
                stateChangeTimer = stateChangeTime;
                if (state == EnemyState.Idle)
                {
                    ChangeDirection(Directions.none, 0);
                }
            }
            if (stateChangeTimer > 0)
                stateChangeTimer -= Time.deltaTime;
            if (state == EnemyState.Moving)
            {
                if (Player.instance.transform != null && CheckPositionRight()
                    || directionCoef == 0 && transform.localRotation.y == 0f)
                {
                    ChangeDirection(Directions.right, 1);
                }
                if (Player.instance.transform != null && CheckPositionLeft()
                    || directionCoef == 0 && (transform.localRotation.y == -1f || transform.localRotation.y == 1f))
                {
                    ChangeDirection(Directions.left, -1);
                }
            }

            canDamagePlayer = Physics2D.OverlapBox(attackPos1.position, attackSize, 0f, masksToDamage);
            isDamaged = Physics2D.OverlapBox(damagePos.position, damageSize, 0f, masksAbleToDamage);
            if ((isDamaged || isDamaged && canDamagePlayer)
                && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
                TakeDamage();
            if (canDamagePlayer)
            {
                StartCoroutine(DamagePlayer());
            }
            ChangeVelocity();
        }
        enemyLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
        UpdateMovementAnimation();
    }

    private void FixedUpdate()
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
        if (!isGrounded)
        {
            if (rigidBody.velocity.y < 0 && rigidBody.gravityScale < maxGravity)
                rigidBody.gravityScale *= gravityMultiplier;
            if (rigidBody.gravityScale > maxGravity)
                rigidBody.gravityScale = maxGravity;
        }
        if (isActive)
        {
            Move();
        }
    }

    protected override void Move()
    {
        float targetSpeed = directionCoef * moveSpeed * velocityCoef;
        float accelerate = 0;
        if (!isGrounded)
            accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount *accelerationInAir : runDeccelerationAmount * accelerationInAir;


        if (Mathf.Abs(rigidBody.velocity.x) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(rigidBody.velocity.x) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f && !isGrounded)
        {
            accelerate = 0;
        }

        float moveForce = (targetSpeed - rigidBody.velocity.x) * accelerate;
        rigidBody.AddForce(moveForce * Vector2.right, ForceMode2D.Force);
    }

    public void Jump()
    {
        AudioManager.instance.Play(17);
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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

    private void OnValidate()
    {
        runAccelerationAmount = (50 * runAcceleration) / maxSpeed;
        runDeccelerationAmount = (50 * runDecceleration) / maxSpeed;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, maxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, maxSpeed);
    }

    private void ChangeDirection(Directions direction, int coef)
    {
        moveDirection = direction;
        directionCoef = coef;
    }

    private void UpdateMovementAnimation()
    {
        if (isGrounded)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("landingMoment", false);
        }
        else
        {
            anim.SetBool("isJumping", true);
            anim.SetFloat("yVelocity", rigidBody.velocity.y);
        }
        if (!wasOnGround && isGrounded)
            anim.SetBool("landingMoment", true);

        if (rigidBody.velocity.x > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0f, 0);
        }
        else if (rigidBody.velocity.x < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 180f, 0);
        }
    }

    private bool CheckPositionRight() => isPlayerNear ? transform.position.x - Player.instance.transform.position.x < 0f : transform.position.x - leftPos.position.x <= 0.25f;

    private bool CheckPositionLeft() => isPlayerNear ? transform.position.x - Player.instance.transform.position.x > 0f : transform.position.x - rightPos.position.x >= -0.25f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(damagePos.position, damageSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos1.position, attackSize);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(playerDetectorPos, playerDetectorSize);
    }

    private void TakeDamage()
    {
        ChangeDirection(Directions.none, 0);
        isActive = false;
        canDamagePlayer = false;
        Player.instance.MiniJump(12f);
        AudioManager.instance.Play(31);
        StartCoroutine(TurnOff());
    }

    protected override IEnumerator DamagePlayer()
    {
        isActive = false;
        canDamagePlayer = false;
        Player.instance.DamagePlayer();
        //if(Mathf.Abs(transform.position.x - Player.instance.respawnPoint.position.x) < 2f)
        //    ChangeDirection(Directions.left, -1);
        //else
        ChangeDirection(Directions.none, 0);
        stateChangeTimer = stateChangeTime;
        yield return new WaitForSeconds(2f);
        transform.position = respawnPoint.position;
        isActive = true;
    }
}
