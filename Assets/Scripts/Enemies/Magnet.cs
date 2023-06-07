using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : Enemy
{
    [Header("Physics")]
    private bool isPlacedOnWall = false;
    private float stateChangeTimer = 0f;
    private int moveDirectionCoef = 0;
    private bool isMoveDirectionChanged = false;
    [SerializeField] bool isMoving;
    [SerializeField] private Directions gravityDirection = Directions.down;
    [SerializeField] private Transform leftBottomPos, rightTopPos;
    [SerializeField] private float stateChangeTime;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] protected float runAcceleration;
    [SerializeField] protected float runDecceleration;
    [SerializeField] protected float runAccelerationAmount;
    [SerializeField] protected float runDeccelerationAmount;
    private readonly Quaternion[] rotations = 
        { Quaternion.Euler(180, 0, 0), Quaternion.Euler(0, 0, 90), Quaternion.Euler(0, 0, 0),Quaternion.Euler(0, 180, 90),
          Quaternion.Euler(180, 180, 0), Quaternion.Euler(0, 180, -90), Quaternion.Euler(0, 180, 0), Quaternion.Euler(0, 0, -90) };

    [Header("Attack")]
    private bool isPlayerDamaged;
    [SerializeField] private float attackTime;
    [SerializeField] private float forceMagnitude = -30f;
    [SerializeField] private float maxForceMagnitude = -50f;
    [SerializeField] private GameObject pointEffector;
    [SerializeField] private PointEffector2D pointEffectorComponent;

    [Header("Damage")]
    private bool isDamaged;
    [SerializeField] private Vector2 damagePos, damageSize;


    protected override void Start()
    {
        base.Start();

        stateChangeTimer = stateChangeTime;
        UpdateGravity();
    }

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
                moveDirectionCoef = 0;
                stateChangeTimer = stateChangeTime;
                anim.SetBool("isAttacking", false);
                anim.SetBool("isMoving", false);
                state = (EnemyState)Random.Range(0, 3);
                if (state == EnemyState.Attack)
                {
                    if (MecroSelectManager.instance.IsPlayerInvisible)
                    {
                        state = EnemyState.Idle;
                    }
                    else
                    {
                        StartCoroutine(Attack());
                    }
                }
                else if (state == EnemyState.Moving && isMoving)
                {
                    moveDirectionCoef = (isMoveDirectionChanged ? -1 : 1);
                    anim.SetBool("isMoving", true);
                }
            }

            if (state == EnemyState.Moving && isMoving)
            {
                if ((moveDirectionCoef < 0 && CheckPosition(leftBottomPos.position)) || (moveDirectionCoef > 0 && CheckPosition(rightTopPos.position)))
                {
                    isMoveDirectionChanged = !isMoveDirectionChanged;
                    moveDirectionCoef = -moveDirectionCoef;
                    Flip();
                }
            }
            else if (state == EnemyState.Attack)
            {
                pointEffectorComponent.forceMagnitude = Player.instance.IsGrounded ? maxForceMagnitude : forceMagnitude;
            }

            if (canTakeDamage && state != EnemyState.Attack && Player.instance.isActive 
                && !MecroSelectManager.instance.IsPlayerInvisible)
            {
                isDamaged = Physics2D.OverlapBox(new Vector2(transform.position.x + damagePos.x, transform.position.y + damagePos.y),
                    damageSize, gravityDirection == Directions.up || gravityDirection == Directions.down ? 0f : 90f, masksAbleToDamage);
                if (isDamaged)
                {
                    TakeDamage();
                }
            }

            if (canDamagePlayer && Player.instance.isActive && !MecroSelectManager.instance.IsPlayerInvisible)
            {
                isPlayerDamaged = Physics2D.OverlapBox(new Vector2(transform.position.x + attackPos.x, transform.position.y + attackPos.y), 
                    attackSize, gravityDirection == Directions.up || gravityDirection == Directions.down ? 0f : 90f, masksToDamage);
                if (isPlayerDamaged)
                {
                    StartCoroutine(DamagePlayer());
                }
            }

            ChangeVelocity();
            if(rigidBody.velocity != Vector2.zero)
                AudioManager.instance.Play(9);
            else
                AudioManager.instance.Stop(9);
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
        float targetSpeed = moveSpeed * moveDirectionCoef * velocityCoef;
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

    private void UpdateGravity()
    {
        if (gravityDirection == Directions.right || gravityDirection == Directions.left)
        {
            rigidBody.gravityScale = 0;
            isPlacedOnWall = true;
        }
        else
        {
            rigidBody.gravityScale = 4f * (gravityDirection == Directions.down ? 1f : -1f);
        }
        transform.rotation = rotations[(int)gravityDirection];
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
        if (moveDirectionCoef > 0f)
        {
            transform.rotation = rotations[(int)gravityDirection];
        }
        else if (moveDirectionCoef < 0f)
        {
            transform.rotation = rotations[(int)gravityDirection + 4];
        }
    }

    private IEnumerator Attack()
    {
        anim.SetBool("isAttacking", true);
        pointEffector.SetActive(true);
        AudioManager.instance.Play(26);
        yield return new WaitForSeconds(attackTime);

        anim.SetBool("isAttacking", false);
        pointEffector.SetActive(false);
        AudioManager.instance.Stop(26);
        stateChangeTimer = 0;
    }

    private void TakeDamage()
    {
        isActive = false;
        canTakeDamage = canDamagePlayer = false;
        moveDirectionCoef = 0;
        anim.SetBool("isMoving", false);
        anim.SetTrigger("damage");
        Player.instance.MiniJump(12f);
        AudioManager.instance.Stop(26);
        AudioManager.instance.Stop(9);
        AudioManager.instance.Play(6);
        StartCoroutine(TurnOff());
    }

    protected override IEnumerator DamagePlayer()
    {
        moveDirectionCoef = 0;
        isActive = false;
        canTakeDamage = canDamagePlayer = false;
        Player.instance.DamagePlayer();
        yield break;
    }

    public override void Recover()
    {
        base.Recover();

        isMoveDirectionChanged = false;
        moveDirectionCoef = 0;
        stateChangeTimer = 0;
        state = EnemyState.Idle;
        stateChangeTimer = stateChangeTime;
        pointEffectorComponent.forceMagnitude = forceMagnitude;
        anim.SetBool("isAttacking", false);
        anim.SetBool("isMoving", false);
        pointEffector.SetActive(false);
        Flip();
        UpdateGravity();
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
}