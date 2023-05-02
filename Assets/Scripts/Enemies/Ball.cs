using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Enemy
{
    [Header("Physics")]
    private float moveInput;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] protected float runAcceleration;
    [SerializeField] protected float runDecceleration;
    [SerializeField] protected float runAccelerationAmount;
    [SerializeField] protected float runDeccelerationAmount;
    [SerializeField] protected float accelerationInAir;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform feetPos;
    [SerializeField] private Vector2 feetDetectorSize;
    [SerializeField] private float jumpForce;

    [Header("Attack")]
    private bool canAttack = true;
    private bool isPlayerNear = false;
    [SerializeField] private bool isAttacking = false;
    private Vector2 vectorToPlayer;
    [SerializeField] private float delayBetweenAttacks;
    [SerializeField] private float attackTime;
    [SerializeField] Transform playerDetectorPos;
    [SerializeField] Vector2 playerDetectorSize;

    [Header("Damage")]
    [SerializeField] private bool isActive = true;
    private bool isDamaged = false;
    [SerializeField] private Transform damagePos;
    [SerializeField] private Vector2 damageSize;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform respawnPoint;

    private float accelerate = 0;

    private void Update()
    {
        isGrounded = Physics2D.OverlapBox(feetPos.position, feetDetectorSize, 0f, groundMask);
        if (isActive)
        {
            isPlayerNear = Physics2D.OverlapBox(playerDetectorPos.position, playerDetectorSize, 0f, masksAbleToDamage);
            if (canAttack)
            {
                if (isPlayerNear)
                {
                    Jump();
                    StartCoroutine(AttackPlayer());
                }
            }
            if(!isPlayerNear)
            {
                isAttacking = false;
                canAttack = true;
                moveInput = 0;
                state = EnemyState.Idle;
            }
            if (isAttacking)
            {
                CalculateMoveDirection();
                //UpdateMovementAnimation();
            }

            canDamagePlayer = Physics2D.OverlapBox(new Vector2(transform.position.x + attackPos.x,
                transform.position.y + attackPos.y), attackSize, 0f, masksAbleToDamage);
            if (canDamagePlayer)
            {
                StartCoroutine(DamagePlayer());
                StopAttacking();
            }
            isDamaged = Physics2D.OverlapBox(damagePos.position, damageSize, 0f, masksToDamage);
            if (isDamaged)
            {
                TakeDamage();
            }
            UpdateMovementAnimation();
            ChangeVelocity();
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
        }
    }

    protected override void Move()
    {
        float targetSpeed = moveInput * moveSpeed * velocityCoef;

        if (isGrounded)
            if (targetSpeed > 0.01f && rigidBody.velocity.x < -1f || targetSpeed < -0.01f && rigidBody.velocity.x > 1f)
                accelerate = runDeccelerationAmount;
            else
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
        rigidBody.AddForce(moveForce * vectorToPlayer * moveInput, ForceMode2D.Force);
    }

    private void OnValidate()
    {
        runAccelerationAmount = (50 * runAcceleration) / maxSpeed;
        runDeccelerationAmount = (50 * runDecceleration) / maxSpeed;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, maxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, maxSpeed);
    }

    public void Jump()
    {
        anim.SetTrigger("attack");
        anim.SetBool("landingMoment", true);
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

    private void CalculateMoveDirection()
    {
        vectorToPlayer = Vector3.Normalize(Player.instance.transform.position - transform.position);
        moveInput = vectorToPlayer.x > 0 ? 1 : -1;
    }

    private void UpdateMovementAnimation()
    {
        /*if (moveInput == 0)
        {
            anim.SetBool("isMoving", false);
            state = EnemyState.Idle;
        }
        else
        {
            anim.SetBool("isMoving", true);
            state = EnemyState.Moving;
        }*/
        if (Mathf.Abs(rigidBody.velocity.x) > 0.2f)
        {
            anim.SetBool("isMoving", true);
            anim.speed = !isGrounded ? 1 : Mathf.Abs(rigidBody.velocity.normalized.x);
        }
        else
        {
            anim.SetBool("isMoving", false);
            anim.speed = 1;
        }

        Flip();
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        isAttacking = true;
        state = EnemyState.Attack;

        CalculateMoveDirection();
        UpdateMovementAnimation();

        yield return new WaitForSeconds(attackTime);
        //TakeDamage();
    }

    private void TakeDamage()
    {
        isActive = canAttack = isAttacking = false;
        moveInput = 0;
        rigidBody.velocity = Vector2.zero;
        state = EnemyState.Destroying;
        anim.SetBool("isMoving", false);
        anim.SetTrigger("damage");
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        StartCoroutine(TurnOff());
    }

    private void Flip()
    {
        if (moveInput == 1f)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveInput == -1f)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private IEnumerator StopAttacking()
    {
        canAttack = isAttacking = false;
        moveInput = 0;
        state = EnemyState.Idle;
        //anim.SetBool("isMoving", false);
        yield return new WaitForSeconds(delayBetweenAttacks);
        canAttack = true;
    }

    protected override IEnumerator DamagePlayer()
    {
        //isActive = false;
        //canDamagePlayer = false;
        if (MecroSelectManager.instance.GetIndex() != 7
            && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
        { Player.instance.DamagePlayer(); }
        yield return new WaitForSeconds(1.5f);
        //transform.position = respawnPoint.position;
        //isActive = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(playerDetectorPos.position, playerDetectorSize);
    }
}
