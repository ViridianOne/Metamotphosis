using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Enemy
{
    [Header("Physics")]
    private float moveDirectionCoef;
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
    private bool isPlayerDamaged;
    private bool canAttack = true;
    private bool isPlayerNear = false;
    [SerializeField] private bool isAttacking = false;
    private Vector2 vectorToPlayer;
    [SerializeField] private float delayBetweenAttacks;
    [SerializeField] private float attackTime;
    [SerializeField] Transform playerDetectorPos;
    [SerializeField] Vector2 playerDetectorSize;

    [Header("Damage")]
    private bool isDamaged = false;
    [SerializeField] private Transform damagePos;
    [SerializeField] private Vector2 damageSize;
    [SerializeField] private LayerMask groundMask;

    private float accelerate = 0;

    private void Update()
    {
        isGrounded = Physics2D.OverlapBox(feetPos.position, feetDetectorSize, 0f, groundMask);
        if (isActive)
        {
            isPlayerNear = Physics2D.OverlapBox(playerDetectorPos.position, playerDetectorSize, 0f, masksAbleToDamage);
            if (canAttack && isPlayerNear && Player.instance.isActive)
            {
                Jump();
                StartCoroutine(AttackPlayer());
            }
            if(!isPlayerNear)
            {
                isAttacking = false;
                canAttack = true;
                moveDirectionCoef = 0;
                state = EnemyState.Idle;
            }
            if (isAttacking)
            {
                CalculateMoveDirection();
                //UpdateMovementAnimation();
            }

            if (canTakeDamage && Player.instance.isActive)
            {
                isDamaged = Physics2D.OverlapBox(damagePos.position, damageSize, 0f, masksToDamage);
                if (isDamaged)
                {
                    TakeDamage();
                }
            }
            if (canDamagePlayer && Player.instance.isActive && !MecroSelectManager.instance.IsPlayerInvisible 
                && MecroSelectManager.instance.GetIndex() != 7)
            {
                isPlayerDamaged = Physics2D.OverlapBox(new Vector2(transform.position.x + attackPos.x,
                    transform.position.y + attackPos.y), attackSize, 0f, masksAbleToDamage);
                if (isPlayerDamaged)
                {
                    StartCoroutine(DamagePlayer());
                    StopAttacking();
                }
            }
            ChangeVelocity();
            UpdateMovementAnimation();
        }
        enemyLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
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
        float targetSpeed = moveDirectionCoef * moveSpeed * velocityCoef;

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
        rigidBody.AddForce(moveForce * vectorToPlayer * moveDirectionCoef, ForceMode2D.Force);
    }

    public void Jump()
    {
        anim.SetTrigger("attack");
        anim.SetBool("landingMoment", true);
        AudioManager.instance.Play(19);
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
        moveDirectionCoef = vectorToPlayer.x > 0 ? 1 : -1;
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
            anim.speed = Mathf.Abs(rigidBody.velocity.normalized.x);
        }
        else
        {
            anim.SetBool("isMoving", false);
            anim.speed = 1;
        }
        if(rigidBody.velocity != Vector2.zero && isGrounded)
            AudioManager.instance.Play(28);
        else
            AudioManager.instance.Stop(28);
        Flip();
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        isAttacking = true;
        state = EnemyState.Attack;

        CalculateMoveDirection();
        //UpdateMovementAnimation();

        yield return new WaitForSeconds(attackTime);
        //TakeDamage();
    }

    private void TakeDamage()
    {
        isActive = canAttack = isAttacking = false;
        canTakeDamage = canDamagePlayer = false; 
        moveDirectionCoef = 0;
        rigidBody.velocity = Vector2.zero;
        state = EnemyState.Destroying;
        anim.SetBool("isMoving", false);
        anim.SetTrigger("damage");
        AudioManager.instance.Stop(28);
        AudioManager.instance.Play(6);
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        StartCoroutine(TurnOff());
    }

    private void Flip()
    {
        if (moveDirectionCoef == 1f)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveDirectionCoef == -1f)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private IEnumerator StopAttacking()
    {
        canAttack = isAttacking = false;
        moveDirectionCoef = 0;
        state = EnemyState.Idle;
        //anim.SetBool("isMoving", false);
        yield return new WaitForSeconds(delayBetweenAttacks);
        canAttack = true;
    }

    protected override IEnumerator DamagePlayer()
    {
        //isActive = false;
        canTakeDamage = canDamagePlayer = false;
        Player.instance.DamagePlayer();
        yield return new WaitForSeconds(1.5f);
        //isActive = true;
    }

    public override void Recover()
    {
        base.Recover();

        canAttack = true;
        isAttacking = false;
        moveDirectionCoef = 0;
        state = EnemyState.Idle;
    }

    private void OnValidate()
    {
        runAccelerationAmount = (50 * runAcceleration) / maxSpeed;
        runDeccelerationAmount = (50 * runDecceleration) / maxSpeed;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, maxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, maxSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(playerDetectorPos.position, playerDetectorSize);
    }
}
