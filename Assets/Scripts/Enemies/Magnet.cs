using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : Enemy
{
    [Header("Physics")]
    private bool isGravitySet = false;
    private bool isPlacedOnWall = false;
    private float stateChangeTimer = 0f;
    private int moveInput = 0;
    private bool isMoveDirectionChanged = false;
    [SerializeField] private float gravityScale = 4f;
    [SerializeField] private Directions gravityDirection = Directions.down;
    [SerializeField] private Transform leftBottomPos, rightTopPos;
    [SerializeField] private float stateChangeTime;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] protected float runAcceleration;
    [SerializeField] protected float runDecceleration;
    [SerializeField] protected float runAccelerationAmount;
    [SerializeField] protected float runDeccelerationAmount;
    private Quaternion[] rotations = { Quaternion.Euler(180, 0, 0), Quaternion.Euler(0, 0, 90), Quaternion.Euler(0, 0, 0),Quaternion.Euler(0, 180, 90),
                                       Quaternion.Euler(180, 180, 0), Quaternion.Euler(0, 180, -90), Quaternion.Euler(0, 180, 0), Quaternion.Euler(0, 0, -90) };

    [Header("Attack")]
    [SerializeField] private float attackTime;
    [SerializeField] private float forceMagnitude = -30f;
    [SerializeField] private float maxForceMagnitude = -50f;
    [SerializeField] private GameObject pointEffector;
    [SerializeField] private PointEffector2D pointEffectorComponent;

    [Header("Damage")]
    private bool isDamaged;
    private bool isActive = true;
    [SerializeField] private Vector2 damagePos, damageSize;


    private void Update()
    {
        if (isActive)
        {
            if (stateChangeTimer > 0f)
            {
                stateChangeTimer -= Time.deltaTime;
            }
            if (stateChangeTimer <= 0f)
            {
                if (!isGravitySet)
                {
                    if (gravityDirection == Directions.right || gravityDirection == Directions.left)
                    {
                        gravityScale = rigidBody.gravityScale;
                        rigidBody.gravityScale = 0;
                        isPlacedOnWall = true;
                    }
                    else
                    {
                        rigidBody.gravityScale = 4f * (gravityDirection == Directions.down ? 1f : -1f);
                    }
                    transform.rotation = rotations[(int)gravityDirection];
                    isGravitySet = true;
                }

                state = (EnemyState)Random.Range(0, 3);
                stateChangeTimer = stateChangeTime;
                moveInput = 0;
                anim.SetBool("isAttacking", false);
                anim.SetBool("isMoving", false);
                if (state == EnemyState.Attack)
                {
                    if (MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
                    {
                        state = EnemyState.Idle;
                    }
                    else
                    {
                        StartCoroutine(Attack());
                    }
                }
                else if (state == EnemyState.Moving)
                {
                    moveInput = (isMoveDirectionChanged ? -1 : 1);
                    anim.SetBool("isMoving", true);
                }
            }

            if (state == EnemyState.Moving)
            {
                if ((moveInput < 0 && CheckPosition(leftBottomPos.position)) || (moveInput > 0 && CheckPosition(rightTopPos.position)))
                {
                    isMoveDirectionChanged = !isMoveDirectionChanged;
                    moveInput = -moveInput;
                    Flip();
                }
            }
            else if (state == EnemyState.Attack)
            {
                pointEffectorComponent.forceMagnitude = Player.instance.IsGrounded ? maxForceMagnitude : forceMagnitude;
            }

            isDamaged = Physics2D.OverlapBox(new Vector2(transform.position.x + damagePos.x,
                transform.position.y + damagePos.y), damageSize, gravityDirection == Directions.up || gravityDirection == Directions.down ? 0f : 90f, masksAbleToDamage);
            if (isDamaged && state != EnemyState.Attack
                && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
            {
                TakeDamage();
            }

            canDamagePlayer = Physics2D.OverlapBox(new Vector2(transform.position.x + attackPos.x,
                transform.position.y + attackPos.y), attackSize, gravityDirection == Directions.up || gravityDirection == Directions.down ? 0f : 90f, masksAbleToDamage);
            if (canDamagePlayer
                && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
            {
                StartCoroutine(DamagePlayer());
            }
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
        float targetSpeed = moveSpeed * moveInput * velocityCoef;
        float currentSpeed = isPlacedOnWall ? rigidBody.velocity.y : rigidBody.velocity.x;
        float accelerate;

        if (Mathf.Abs(currentSpeed) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(currentSpeed) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f)
        {
            accelerate = 0f;
        }
        else
        {
            accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount : runDeccelerationAmount;
        }

        float moveForce = (targetSpeed - currentSpeed) * accelerate;
        if (isPlacedOnWall)
        {
            rigidBody.AddForce(moveForce * Vector2.up, ForceMode2D.Force);
            rigidBody.AddForce(gravityScale * Vector2.right * (gravityDirection == Directions.right ? 1 : -1), ForceMode2D.Force);
        }
        else
        {
            rigidBody.AddForce(moveForce * Vector2.right, ForceMode2D.Force);
        }
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + damagePos.x, transform.position.y + damagePos.y), damageSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + attackPos.x, transform.position.y + attackPos.y), attackSize);
    }

    private bool CheckPosition(Vector2 pos)
    {
        if (isPlacedOnWall)
        {
            return Mathf.Abs(transform.position.y - pos.y) <= 0.5f;
        }
        else
        {
            return Mathf.Abs(transform.position.x - pos.x) <= 0.5f;
        }
    }

    private void Flip()
    {
        if (moveInput > 0f)
        {
            transform.rotation = rotations[(int)gravityDirection];
        }
        else if (moveInput < 0f)
        {
            transform.rotation = rotations[(int)gravityDirection + 4];
        }
    }

    private IEnumerator Attack()
    {
        anim.SetBool("isAttacking", true);
        pointEffector.SetActive(true);
        yield return new WaitForSeconds(attackTime);
        pointEffector.SetActive(false);
        anim.SetBool("isAttacking", false);
        stateChangeTimer = 0f;
    }

    private void TakeDamage()
    {
        isActive = false;
        moveInput = 0;
        anim.SetBool("isMoving", false);
        anim.SetTrigger("damage");
        Player.instance.MiniJump(12f);
        StartCoroutine(TurnOff());
    }

    protected override IEnumerator DamagePlayer()
    {
        isActive = false;
        Player.instance.DamagePlayer();
        yield return new WaitForSeconds(1f);
        stateChangeTimer = 0;
        isActive = true;
    }
}