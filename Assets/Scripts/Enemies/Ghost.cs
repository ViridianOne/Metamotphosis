using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    [Header("Physics")]
    private float moveDirectionCoef;
    private float currentSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] protected float runAcceleration;
    [SerializeField] protected float runDecceleration;
    [SerializeField] protected float runAccelerationAmount;
    [SerializeField] protected float runDeccelerationAmount;
    [SerializeField] protected float accelerationInAir;

    [Header("Attack")]
    private bool isPlayerNear;
    private bool isPlayerDamaged;
    private bool canAttack = true;
    private bool isAttacking = false;
    private Vector2 vectorToPlayer;
    [SerializeField] private float attackAreaRadius;
    [SerializeField] private float attackTime;
    [SerializeField] private Vector2 detectorOffset, detectorSize;


    private void Update()
    {
        if (isActive)
        {
            if (canAttack)
            {
                isPlayerNear = Physics2D.OverlapBox(transform.position.AsVector2() + detectorOffset, detectorSize, 0, masksAbleToDamage);
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

            if (canDamagePlayer)
            {
                isPlayerDamaged = Physics2D.OverlapBox(transform.position.AsVector2() + attackPos, attackSize, 0f, masksAbleToDamage);
                if (isPlayerDamaged && Player.instance.isActive)
                {
                    StartCoroutine(DamagePlayer());
                    StopAttacking();
                }
            }
            ChangeVelocity();
        }
        enemyLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
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
        currentSpeed = rigidBody.velocity.magnitude * moveDirectionCoef;

        var targetSpeed = moveSpeed * moveDirectionCoef * velocityCoef;
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
        rigidBody.AddForce(moveForce * vectorToPlayer * moveDirectionCoef, ForceMode2D.Force);
        rigidBody.velocity = vectorToPlayer * rigidBody.velocity.magnitude;
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
        moveDirectionCoef = vectorToPlayer.x > 0 ? 1 : -1;
    }

    private void UpdateMovementAnimation()
    {
        if (moveDirectionCoef == 0)
        {
            anim.SetBool("isChasing", false);
            state = EnemyState.Idle;
        }
        else
        {
            anim.SetBool("isChasing", true);
            state = EnemyState.Moving;
        }
        if(rigidBody.velocity != Vector2.zero)
            AudioManager.instance.Play(27);
        Flip();
    }

    private void TakeDamage()
    {
        isActive = canAttack = isAttacking = false;
        canTakeDamage = canDamagePlayer = false;
        moveDirectionCoef = 0;
        rigidBody.velocity = Vector2.zero;
        state = EnemyState.Destroying;
        anim.SetBool("isChasing", false);
        anim.SetTrigger("damage");
        AudioManager.instance.Stop(27);
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

    private void StopAttacking()
    {
        canAttack = isAttacking = false;
        moveDirectionCoef = 0;
        state = EnemyState.Idle;
        anim.SetBool("isChasing", false);        
    }

    protected override IEnumerator DamagePlayer()
    {
        Player.instance.DamagePlayer();
        TakeDamage();
        yield break;
    }

    public override void Recover()
    {
        base.Recover();

        canAttack = true;
        isAttacking = false;
        moveDirectionCoef = 0;
        state = EnemyState.Idle;
        anim.SetBool("isChasing", false);
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position.AsVector2() + attackPos, attackSize);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position.AsVector2() + detectorOffset, detectorSize);
    }
}

