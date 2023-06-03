using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Enemy
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

    [Header("Attack")]
    [SerializeField] private float attackTime;
    [SerializeField] private float fullAttackTime;
    private bool canTurnOffTheLights = true;
    [SerializeField] private Transform attackPos1;

    [Header("Damage")]
    private bool isDamaged;
    [SerializeField] private Transform damagePos;
    [SerializeField] private Vector2 damageSize;

    private void Update()
    {
        if (isActive)
        {
            if (stateChangeTimer <= 0)
            {
                state = (EnemyState)Random.Range(0, 3);
                stateChangeTimer = stateChangeTime;
                if (state == EnemyState.Idle)
                {
                    ChangeDirection(Directions.none, 0);
                }
                else if (state == EnemyState.Attack)
                {
                    StartCoroutine(Attack());
                }
            }
            if (stateChangeTimer > 0)
                stateChangeTimer -= Time.deltaTime;
            if (state == EnemyState.Moving)
            {
                if (CheckPosition(leftPos.position.x) || directionCoef == 0 && transform.localRotation.y == 0f)
                {
                    ChangeDirection(Directions.right, 1);
                }
                if (CheckPosition(rightPos.position.x) || directionCoef == 0
                    && (transform.localRotation.y == -1f || transform.localRotation.y == 1f))
                {
                    ChangeDirection(Directions.left, -1);
                }
            }
            canDamagePlayer = Physics2D.OverlapBox(attackPos1.position, attackSize, 0f, masksToDamage);
            isDamaged = Physics2D.OverlapBox(damagePos.position, damageSize, 0f, masksAbleToDamage);
            if ((isDamaged || isDamaged && canDamagePlayer) 
                && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
                TakeDamage();
            if (canDamagePlayer 
                && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
            {
                StartCoroutine(DamagePlayer());
            }
            ChangeVelocity();
        }
        enemyLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
        UpdateMovementAnimation();
    }

    private IEnumerator Attack()
    {
        ChangeDirection(Directions.none, 0);
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(attackTime);
        LevelManager.instance.SetGlobalLightItensity(canTurnOffTheLights ? 0 : 1);
        canTurnOffTheLights = !canTurnOffTheLights;
        AudioManager.instance.Play(29);
        yield return new WaitForSeconds(fullAttackTime - attackTime);
        stateChangeTimer = 0;
    }

    private void FixedUpdate()
    {
        if(isActive)
            Move();
    }

    protected override void Move()
    {
        float targetSpeed = directionCoef * moveSpeed * velocityCoef;
        float accelerate = 0;

        accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount : runDeccelerationAmount;

        if (Mathf.Abs(rigidBody.velocity.x) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(rigidBody.velocity.x) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f)
        {
            accelerate = 0;
        }

        float moveForce = (targetSpeed - rigidBody.velocity.x) * accelerate;
        rigidBody.AddForce(moveForce * Vector2.right, ForceMode2D.Force);
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
        if (rigidBody.velocity.x > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0f, 0);
            anim.SetBool("isMoving", true);
        }
        else if (rigidBody.velocity.x < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 180f, 0);
            anim.SetBool("isMoving", true);
        }
        else
            anim.SetBool("isMoving", false);
        if (state == EnemyState.Moving && rigidBody.velocity.y == 0)
            AudioManager.instance.Play(10);
        if (state != EnemyState.Moving && AudioManager.instance.sounds[10].source.isPlaying)
            AudioManager.instance.Stop(10);
    }

    private bool CheckPosition(float xPos) => transform.position.x - xPos >= -0.25f && transform.position.x - xPos <= 0.25f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(damagePos.position, damageSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos1.position, attackSize);
    }

    private void TakeDamage()
    {
        ChangeDirection(Directions.none, 0);
        isActive = false;
        canDamagePlayer = false;
        Player.instance.MiniJump(12f);
        LevelManager.instance.SetGlobalLightItensity(1);
        AudioManager.instance.Stop(10);
        AudioManager.instance.Play(6);
        StartCoroutine(TurnOff());
    }

    protected override IEnumerator DamagePlayer()
    {
        isActive = false;
        canDamagePlayer = false;
        Player.instance.DamagePlayer();
        ChangeDirection(Directions.none, 0);
        stateChangeTimer = stateChangeTime;
        yield return new WaitForSeconds(1.5f);
    }
}
