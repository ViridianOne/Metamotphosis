using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : Enemy
{
    [Header("Physics")]
    private float moveDirectionCoef;
    private Vector2 direction = Vector2.right;
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
    private bool isPlayerDamaged;
    private Vector3 predictedPlayerPos;
    private Vector2 vectorToPlayer;
    private Vector2 vectorFromPlayer;
    [SerializeField] private float attackAreaRadius;
    [SerializeField] private float delayBetweenAttacks;
    [SerializeField] private float predictionScale = 10000;
    [SerializeField] private Vector2 detectorSize, detectorOffset;

    [Header("Damage")]
    private bool isDamaged = false;
    private bool readyToTakeDamage = false;
    [SerializeField] protected Vector2 damagePos, damageSize;
    [SerializeField] private LayerMask groundMask;


    private void Update()
    {
        if (isActive)
        {
            if (isAttacking)
            {
                if (Vector3.Distance(transform.position, predictedPlayerPos) < 0.7f)
                {
                    StartCoroutine(StopAttacking());
                }
            }
            else if (canAttack && !MecroSelectManager.instance.IsPlayerInvisible)
            {
                isPlayerNear = Physics2D.OverlapBox(transform.position.AsVector2() + detectorOffset, detectorSize, 0, masksAbleToDamage);
                if (isPlayerNear)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
            
            if (canTakeDamage && readyToTakeDamage)
            {
                isDamaged = Physics2D.OverlapBox(
                    new Vector2(transform.position.x + damagePos.x, transform.position.y + damagePos.y), damageSize, 0f, groundMask);
                if (isDamaged)
                {
                    TakeDamage();
                }
            }

            if (canDamagePlayer && Player.instance.isActive && !MecroSelectManager.instance.IsPlayerInvisible)
            {
                isPlayerDamaged = Physics2D.OverlapBox(
                    new Vector2(transform.position.x + attackPos.x, transform.position.y + attackPos.y), attackSize, 0f, masksToDamage);
                if (isPlayerDamaged)
                {
                    StartCoroutine(DamagePlayer());
                }

            }
            ChangeVelocity();
        }
        enemyLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
    }

    private void FixedUpdate()
    {
        if (isActive/* && isAttacking*/)
        {
            Move();
            if (Mathf.Abs(currentSpeed) < 0.5f)
            {
                rigidBody.velocity = Vector2.zero;
                AudioManager.instance.Stop(16);
            }
            if(rigidBody.velocity != Vector2.zero)
                AudioManager.instance.Play(16);
        }
    }

    protected override void Move()
    {
        currentSpeed = rigidBody.velocity.magnitude * moveDirectionCoef;

        var targetSpeed = moveDirectionCoef * moveSpeed * velocityCoef;
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
        rigidBody.AddForce(moveForce * (moveDirectionCoef == 1 ? vectorToPlayer : vectorFromPlayer), ForceMode2D.Force);
    }

    private void TakeDamage()
    {
        isActive = false;
        canTakeDamage = canDamagePlayer = false;
        moveDirectionCoef = 0;
        rigidBody.velocity = Vector2.zero;
        state = EnemyState.Destroying;
        anim.SetBool("isFlying", false);
        anim.SetFloat("idleCoef", 0);
        anim.SetTrigger("damage");
        AudioManager.instance.Stop(16);
        AudioManager.instance.Play(29);
        StartCoroutine(TurnOff());
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        isAttacking = true;
        rigidBody.gravityScale = 0;
        enemyCollider.enabled = false;
        state = EnemyState.Attack;
        anim.SetFloat("idleCoef", 1);

        PredictPlayerPosition();
        CalculateMoveDirection();
        UpdateMovementAnimation();
        yield return new WaitForSeconds(0.5f);

        readyToTakeDamage = true;
    }

    private void PredictPlayerPosition()
    {
        var playerPos = Player.instance.transform.position;
        predictedPlayerPos = new Vector2(playerPos.x, playerPos.y)
            + Player.instance.GetVelocity() * Time.deltaTime * Time.deltaTime * predictionScale;
        //predictedPlayerPos = new Vector2(Player.instance.transform.position.x, Player.instance.transform.position.y);
    }

    private void CalculateMoveDirection()
    {
        vectorToPlayer = Vector3.Normalize(predictedPlayerPos - transform.position);
        vectorFromPlayer = new Vector2(-vectorToPlayer.x, -vectorToPlayer.y);
        var angleToPlayer = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg;
        if (angleToPlayer <= 45f && angleToPlayer >= -45f)
        {
            moveDirectionCoef = 1f;
            direction = Vector2.right;
        }
        else if (angleToPlayer >= 135f || angleToPlayer <= -135f)
        {
            moveDirectionCoef = -1f;
            direction = Vector2.left;
        }
        else if (angleToPlayer < 135f && angleToPlayer > 45f)
        {
            moveDirectionCoef = 1f;
            direction = Vector2.up;
        }
        else if (angleToPlayer > -135f && angleToPlayer < -45f)
        {
            moveDirectionCoef = -1f;
            direction = Vector2.down;
        }

        if (angleToPlayer >= 22.5f && angleToPlayer <= 67.5f || angleToPlayer <= -22.5f && angleToPlayer >= -67.5f
            || angleToPlayer >= 112.5f && angleToPlayer <= 157.5f || angleToPlayer <= -112.5f && angleToPlayer >= -157.5f)
            anim.SetFloat("angle", 45);
        else
            anim.SetFloat("angle", 0);
    }

    private void UpdateMovementAnimation()
    {
        if (moveDirectionCoef == 0)
        {
            anim.SetBool("isFlying", false);
            state = EnemyState.Idle;
        }
        else
        {
            anim.SetBool("isFlying", true);
            state = EnemyState.Moving;
        }

        Flip();
    }

    private void Flip()
    {
        if (moveDirectionCoef == 1f)
        {
            transform.localRotation = direction == Vector2.up ? Quaternion.Euler(0, 180, 90) : Quaternion.Euler(0, 0, 0);
        }
        else if (moveDirectionCoef == -1f)
        {
            transform.localRotation = direction == Vector2.down ? Quaternion.Euler(0, 0, -90) : Quaternion.Euler(0, 180, 0);
        }
    }

    private IEnumerator StopAttacking()
    {
        canAttack = isAttacking = false;
        moveDirectionCoef = 0;
        state = EnemyState.Idle;
        anim.SetBool("isFlying", false);
        yield return new WaitForSeconds(delayBetweenAttacks);

        canAttack = true;
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
        readyToTakeDamage = false;
        moveDirectionCoef = 0;
        state = EnemyState.Idle;
        anim.SetBool("isFlying", false);
        rigidBody.gravityScale = gravityScale;
        enemyCollider.enabled = true;
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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position.AsVector2() + damagePos, damageSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position.AsVector2() + detectorOffset, detectorSize);
    }
}
