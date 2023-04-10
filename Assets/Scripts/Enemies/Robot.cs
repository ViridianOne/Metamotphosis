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

    [Header("Damage")]
    private bool isDamaged;
    [SerializeField] private Vector2 damagePos, damageSize;
    private bool isActive = true;

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
            UpdateMovementAnimation();
            attackPos.x = transform.position.x;
            canDamagePlayer = Physics2D.OverlapBox(attackPos, attackSize, 0f, masksAbleToDamage);
            damagePos.x = transform.position.x;
            isDamaged = Physics2D.OverlapBox(damagePos, damageSize, 0f, masksAbleToDamage);
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
    }

    private IEnumerator Attack()
    {
        ChangeDirection(Directions.none, 0);
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(attackTime);
        if (Darkness.instance.gameObject != null)
        {
            Darkness.instance.TurnOn(canTurnOffTheLights);
            canTurnOffTheLights = !canTurnOffTheLights;
        }
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
    }

    private bool CheckPosition(float xPos) => transform.position.x - xPos >= -0.25f && transform.position.x - xPos <= 0.25f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(damagePos, damageSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos, attackSize);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        ChangeDirection(Directions.none, 0);
    //        isActive = false;
    //        Player.instance.MiniJump(12f);
    //        //rigidBody.gravityScale = 0;
    //        //gameObject.GetComponent<Collider2D>().enabled = false;
    //        StartCoroutine(TurnOff());
    //    }
    //}

    private void TakeDamage()
    {
        //masksAbleToDamage = LayerMask.NameToLayer("NoDamage");
        ChangeDirection(Directions.none, 0);
        isActive = false;
        Player.instance.MiniJump(12f);
        if (Darkness.instance.gameObject != null)
        {
            Darkness.instance.TurnOn(false);
            canTurnOffTheLights = !canTurnOffTheLights;
        }
        //rigidBody.gravityScale = 0;
        //gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(TurnOff());
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.transform.CompareTag("Player"))
    //        Player.instance.DamagePlayer();
    //    if (!gameObject.activeInHierarchy)
    //        gameObject.SetActive(true);
    //    ChangeDirection(Directions.none, 0);
    //    stateChangeTimer = stateChangeTime;
    //}

    protected override IEnumerator DamagePlayer()
    {
        isActive = false;
        Player.instance.DamagePlayer();
        ChangeDirection(Directions.none, 0);
        stateChangeTimer = stateChangeTime;
        yield return new WaitForSeconds(1.5f);
        isActive = true;
    }
}
