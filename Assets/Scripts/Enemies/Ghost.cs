using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    [Header("Physics")]
    private float moveInput;
    private float currentSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] protected float runAcceleration;
    [SerializeField] protected float runDecceleration;
    [SerializeField] protected float runAccelerationAmount;
    [SerializeField] protected float runDeccelerationAmount;
    [SerializeField] protected float accelerationInAir;

    [Header("Attack")]
    private bool canAttack = true;
    private bool isPlayerNear = false;
    private bool isAttacking = false;
    private Vector2 vectorToPlayer;
    [SerializeField] private float attackAreaRadius;
    [SerializeField] private float attackTime;


    private void Update()
    {
        if (isActive)
        {
            if (canAttack)
            {
                isPlayerNear = Physics2D.OverlapCircle(transform.position, attackAreaRadius, masksAbleToDamage);
                if (isPlayerNear)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
            else if (isAttacking)
            {
                CalculateMoveDirection();
                UpdateMovementAnimation();
            }

            canDamagePlayer = Physics2D.OverlapBox(new Vector2(transform.position.x + attackPos.x,
                transform.position.y + attackPos.y), attackSize, 0f, masksAbleToDamage);
            if (canDamagePlayer)
            {
                StartCoroutine(DamagePlayer());
                StopAttacking();
            }
            ChangeVelocity();
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
            if (Mathf.Abs(currentSpeed) < 0.5f)
            {
                rigidBody.velocity = Vector2.zero;
            }
        }
    }

    protected override void Move()
    {
        currentSpeed = rigidBody.velocity.magnitude * moveInput;

        var targetSpeed = moveSpeed * moveInput * velocityCoef;
        float accelerate;

        if (Mathf.Abs(currentSpeed) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(currentSpeed) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f)
        {
            accelerate = 0;
        }
        else
        {
            accelerate = (Mathf.Abs(targetSpeed) > 1f ? runAccelerationAmount : runDeccelerationAmount) * accelerationInAir;
        }

        var moveForce = (targetSpeed - currentSpeed) * accelerate;
        rigidBody.AddForce(moveForce * vectorToPlayer * moveInput, ForceMode2D.Force);
        rigidBody.velocity = vectorToPlayer * rigidBody.velocity.magnitude;
    }

    private void OnValidate()
    {
        runAccelerationAmount = (50 * runAcceleration) / maxSpeed;
        runDeccelerationAmount = (50 * runDecceleration) / maxSpeed;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, maxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, maxSpeed);
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        isAttacking = true;
        state = EnemyState.Attack;

        CalculateMoveDirection();
        UpdateMovementAnimation();

        yield return new WaitForSeconds(attackTime);
        TakeDamage();
    }

    private void CalculateMoveDirection()
    {
        vectorToPlayer = Vector3.Normalize(Player.instance.transform.position - transform.position);
        moveInput = vectorToPlayer.x > 0 ? 1 : -1;
    }

    private void UpdateMovementAnimation()
    {
        if (moveInput == 0)
        {
            anim.SetBool("isChasing", false);
            state = EnemyState.Idle;
        }
        else
        {
            anim.SetBool("isChasing", true);
            state = EnemyState.Moving;
        }

        Flip();
    }

    private void TakeDamage()
    {
        isActive = canAttack = isAttacking = false;
        moveInput = 0;
        rigidBody.velocity = Vector2.zero;
        state = EnemyState.Destroying;
        anim.SetBool("isChasing", false);
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

    private void StopAttacking()
    {
        canAttack = isAttacking = false;
        moveInput = 0;
        state = EnemyState.Idle;
        anim.SetBool("isChasing", false);        
    }

    protected override IEnumerator DamagePlayer()
    {
        Player.instance.DamagePlayer();
        TakeDamage();
        yield return new WaitForSeconds(0f);
    }

    public override void Recover()
    {
        base.Recover();

        canAttack = true;
    }
}

