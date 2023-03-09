using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : Enemy
{
    /*    [Header("Physics")]
        [SerializeField] private Transform leftPos, rightPos;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float maxSpeed;
        [SerializeField] protected float runAcceleration;
        [SerializeField] protected float runDecceleration;
        [SerializeField] protected float runAccelerationAmount;
        [SerializeField] protected float runDeccelerationAmount;
        [SerializeField] protected float accelerationInAir; //maybe not protected
        private Directions moveDirection = Directions.none;
        private int directionCoef = 0;
        [SerializeField] private float stateChangeTime;
        private float stateChangeTimer;
        [SerializeField] protected float maxGravity;
        [SerializeField] protected float gravityMultiplier;

        [Header("Attack")]
        [SerializeField] private float attackTime;
        [SerializeField] private float fullAttackTime;
        private bool canTurnOffTheLights = true;

        [Header("Damage")]
        private bool isDamaged;
        [SerializeField] private Vector2 damagePos, damageSize;
        private bool isActive = true;

        private bool isGrounded;
        private bool wasOnGround;
        [SerializeField] private Transform feetPos;
        [SerializeField] private Vector2 feetDetectorSize;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask groundMask;
        protected float gravity;
        [SerializeField] private float jumpForce;

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
                    wasOnGround = isGrounded;
                    isGrounded = Physics2D.OverlapBox(feetPos.position, feetDetectorSize, 0f, groundMask);
                    if (!wasOnGround && isGrounded)
                    {
                        rigidBody.gravityScale = gravity;
                        StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f));
                    }
                }
                UpdateMovementAnimation();
                attackPos.x = transform.position.x;
                canDamagePlayer = Physics2D.OverlapBox(attackPos, attackSize, 0f, masksAbleToDamage);
                damagePos.x = transform.position.x;
                isDamaged = Physics2D.OverlapBox(damagePos, damageSize, 0f, masksAbleToDamage);
                if (isDamaged || isDamaged && canDamagePlayer)
                    TakeDamage();
                if (canDamagePlayer)
                {
                    StartCoroutine(DamagePlayer());
                }
            }
        }

        private IEnumerator Attack()
        {
            ChangeDirection(Directions.none, 0);
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
            if (isActive)
            {
                Jump();
                Move();
                if (!isGrounded)
                {
                    if (rigidBody.velocity.y < 0 && rigidBody.gravityScale < maxGravity)
                        rigidBody.gravityScale *= gravityMultiplier;
                    if (rigidBody.gravityScale > maxGravity)
                        rigidBody.gravityScale = maxGravity;
                }
            }
        }

        protected override void Move()
        {
            float targetSpeed = directionCoef * moveSpeed;
            float accelerate = 0;

            if (!isGrounded)
                accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount * accelerationInAir : runDeccelerationAmount * accelerationInAir;


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
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            StartCoroutine(JumpSqueeze(0.8f, 1.15f, 0.05f));
            AudioManager.instance.Play(5);
        }

        private void OnValidate()
        {
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
                anim.SetBool("isJumping", true);
            if (!wasOnGround && isGrounded)
                anim.SetBool("landingMoment", true);
        }

        private bool CheckPosition(float xPos) => transform.position.x - xPos >= -0.25f && transform.position.x - xPos <= 0.25f;

        private void OnDrawGizmos()
        {
        }

        private void TakeDamage()
        {
            ChangeDirection(Directions.none, 0);
            isActive = false;
            Player.instance.MiniJump(12f);
            if (Darkness.instance.gameObject != null)
            {
                Darkness.instance.TurnOn(false);
                canTurnOffTheLights = !canTurnOffTheLights;
            }
            StartCoroutine(TurnOff());
        }

        protected override IEnumerator DamagePlayer()
        {
            isActive = false;
            Player.instance.DamagePlayer();
            ChangeDirection(Directions.none, 0);
            stateChangeTimer = stateChangeTime;
            yield return new WaitForSeconds(1.5f);
            isActive = true;
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
        }*/
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
    public float gravity;
    [SerializeField] private float minJumpTime;
    private bool isChargingJump = false;
    [SerializeField] protected float accelerationInAir;

    private void Update()
    {
        if (isActive)
        {
            wasOnGround = isGrounded;
            isGrounded = Physics2D.OverlapBox(feetPos.position, feetDetectorSize, 0f, groundMask);
            if (!wasOnGround && isGrounded)
            {
                rigidBody.gravityScale = gravity;
                betweenJumpTimer = timeBetweenJump;
                jumpTimer = 0f;
                isJumpCharged = false;
                StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f));
            }

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
            if (isDamaged || isDamaged && canDamagePlayer)
                TakeDamage();
            if (canDamagePlayer)
            {
                StartCoroutine(DamagePlayer());
            }
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
        /*if (isActive)
            Move();*/
        if (isActive)
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
            Move();
            if (!isGrounded)
            {
                if (rigidBody.velocity.y < 0 && rigidBody.gravityScale < maxGravity)
                    rigidBody.gravityScale *= gravityMultiplier;
                if (rigidBody.gravityScale > maxGravity)
                    rigidBody.gravityScale = maxGravity;
            }
        }
    }

    protected override void Move()
    {
        float targetSpeed = directionCoef * moveSpeed;
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
            anim.SetBool("isJumping", true);
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

    private bool CheckPosition(float xPos) => transform.position.x - xPos >= -0.25f && transform.position.x - xPos <= 0.25f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(damagePos, damageSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos, attackSize);
    }


    private void TakeDamage()
    {
        ChangeDirection(Directions.none, 0);
        isActive = false;
        Player.instance.MiniJump(12f);
        if (Darkness.instance.gameObject != null)
        {
            Darkness.instance.TurnOn(false);
            canTurnOffTheLights = !canTurnOffTheLights;
        }
        StartCoroutine(TurnOff());
    }


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
