using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecrobot : MonoBehaviour
{
    [Header("Anim")]
    private List<SpriteRenderer> spritesToHideInDarkness;
    private SpriteRenderer mecrobotSprite, flaskSprite, bot296Sprite, mecrobotBGSprite;
    [HideInInspector] private Animator anim;
    [SerializeField] private GameObject holder;

    [Header("Components")]
    private Vector2 activeFlaskPosition;
    [SerializeField] private GameObject flask;
    [SerializeField] private GameObject bot161;
    [SerializeField] private GameObject bot296;
    [SerializeField] private GameObject mecrobotBG;
    [SerializeField] private GameObject ceilingTraps;
    [SerializeField] private GameObject platforms;
    [SerializeField] private GameObject groundTraps;
    [SerializeField] private GameObject bossRoomEnter;
    [SerializeField] private SpriteRenderer flashEffect;

    [Header("Psycics")]
    private Rigidbody2D rigidBody;
    private CapsuleCollider2D bossCollider;
    private Vector2 startColliderOffset, startColliderSize;
    private Vector2 extendedColliderOffset, extendedColliderSize;
    private Vector2 reducedColliderOffset, reducedColliderSize;
    private int enemyLayerMask, playerLayerMask, platformLayerMask;
    private float attackChangeTimer;
    [SerializeField] private float attackChangeTime;
    [SerializeField] private float gravityScale = 4f;
    [SerializeField] protected float minGravity;
    [SerializeField] protected float maxGravity;
    [SerializeField] protected float gravityMultiplier;
    [SerializeField] protected float gravityAddition;
    [SerializeField] private Transform leftPos, rightPos;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float runAcceleration;
    [SerializeField] private float runDecceleration;
    [SerializeField] private float runAccelerationAmount;
    [SerializeField] private float runDeccelerationAmount;
    [SerializeField] protected float accelerationInAir;

    [Header("Damage")]
    private bool isActive = true;
    private bool isFlaskHidden = true;
    private int bossDamageCount = 0;
    private bool ableToTakeDamage;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float timeToDamageFlask;
    [SerializeField] private float bossDefeatTime;
    [SerializeField] private Vector2 damagePos, damageSize;
    [SerializeField] private LayerMask masksAbleToDamage;

    [Header("Attack")]
    private int attacksCount = 0;
    private bool isHardLandingAttack, isBreakthroughAttack;
    private bool isCeilingTrapsShown, isFloorTrapsShown;
    [SerializeField] private Vector2 attackPos, attackSize;
    [SerializeField] private Vector2 attackAreaPos, attackAreaSize;
    [SerializeField] private LayerMask masksToAttack;
    [SerializeField] private float darknessAttackTime, flashAttackTime,
        shownCeilingTrapsTime, shownFloorTrapsTime;

    [Header("Jumping")]
    private int moveInput;
    private bool wasOnGround, isGrounded;
    private float betweenJumpTimer = 0f;
    [SerializeField] private Vector2 feetPos, feetDetectorSize;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float minJumpForce;
    [SerializeField] private float maxJumpForce;
    [SerializeField] private float timeBetweenJump;

    [Header("States")]
    private MecroStates currentBot;
    private MecroStates previousBot;
    private MecroStates currentFlask;
    private bool isBossDefeated = false;
    public bool IsFightStarted { get; private set; }
    public bool IsPlayerDefeated { get; private set; }


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<CapsuleCollider2D>();
        mecrobotSprite = holder.GetComponent<SpriteRenderer>();
        flaskSprite = flask.GetComponent<SpriteRenderer>();
        bot296Sprite = bot296.GetComponent<SpriteRenderer>();
        mecrobotBGSprite = mecrobotBG.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        spritesToHideInDarkness = new List<SpriteRenderer>() { mecrobotSprite, flaskSprite, bot296Sprite, mecrobotBGSprite };
        (startColliderOffset, startColliderSize) = (bossCollider.offset, bossCollider.size);
        (extendedColliderOffset, extendedColliderSize) = (new Vector2(0, -1.05f), new Vector2(5f, 7.2f));
        (reducedColliderOffset, reducedColliderSize) = (new Vector2(0, -0.8f), new Vector2(5f, 6.65f));
        enemyLayerMask = (int)Mathf.Log(LayerMask.GetMask("Enemy"), 2);
        playerLayerMask = (int)Mathf.Log(LayerMask.GetMask("Player"), 2);
        platformLayerMask = (int)Mathf.Log(LayerMask.GetMask("Platform"), 2);
        ableToTakeDamage = true;
        attackChangeTimer = 3f;
        activeFlaskPosition = flask.transform.localPosition;
        flask.transform.localPosition = Vector2.zero;
        IsFightStarted = false;
        IsPlayerDefeated = false;
        currentBot = previousBot = MecroStates.none;
    }

    void Update()
    {
        if (isActive)
        {
            wasOnGround = isGrounded;
            isGrounded = Physics2D.OverlapBox(feetPos.Add(transform.position), feetDetectorSize, 0f, groundMask);
            if (!wasOnGround && isGrounded)
            {
                if (isHardLandingAttack)
                {
                    StartCoroutine(EndHardLanding());
                }
                else if (isBreakthroughAttack)
                {
                    EndBreakthrough();
                }
                rigidBody.gravityScale = gravityScale;
                StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f, true));
            }

            if (isFlaskHidden)
            {
                if (currentBot == MecroStates.none)
                {
                    if (attackChangeTimer > 0)
                    {
                        attackChangeTimer -= Time.deltaTime;
                    }

                    if (previousBot != MecroStates.form296 || (previousBot == MecroStates.form296
                        && Physics2D.OverlapBox(attackAreaPos.Add(transform.position), attackAreaSize, 0, masksToAttack)))
                    {
                        if (attackChangeTimer <= 0)
                        {
                            attackChangeTimer = attackChangeTime;
                            Attack();
                        }
                    }
                }
                else
                {
                    if (CheckPosition(leftPos.position.x))
                    {
                        moveInput = 1;
                    }
                    else if (CheckPosition(rightPos.position.x))
                    {
                        moveInput = -1;
                    }
                }

                if (Physics2D.OverlapCapsule(attackPos.Add(transform.position), attackSize, CapsuleDirection2D.Vertical, 0f, masksToAttack))
                {
                    DamagePlayer();
                }
            }
            else if (ableToTakeDamage)
            {
                if (Physics2D.OverlapBox(damagePos.Add(transform.position), damageSize, 0, masksAbleToDamage))
                {
                    StartCoroutine(TakeDamage());
                }
            }

            UpdateMovementAnimation();
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            if (isGrounded)
            {
                if (currentBot == MecroStates.form161)
                {
                    if (betweenJumpTimer <= 0)
                    {
                        betweenJumpTimer = timeBetweenJump;
                        Jump(Random.Range(minJumpForce, maxJumpForce));
                    }
                    else
                        betweenJumpTimer -= Time.deltaTime;
                }
                else if (currentBot == MecroStates.form296)
                {
                    if (isHardLandingAttack)
                    {
                        Jump(maxJumpForce);
                    }
                }
            }
            else
            {
                if (currentBot == MecroStates.form296)
                {
                    if (isHardLandingAttack)
                    {
                        if (rigidBody.velocity.y <= 0)
                        {
                            StartCoroutine(AccumulateHardLanding());
                        }
                    }
                }

                if (!isHardLandingAttack)
                {
                    if (rigidBody.velocity.y < 0 && rigidBody.gravityScale < maxGravity)
                        rigidBody.gravityScale *= gravityMultiplier;
                    if (rigidBody.gravityScale > maxGravity)
                        rigidBody.gravityScale = maxGravity;
                }
            }

            Move();
        }
    }

    private void OnValidate()
    {
        runAccelerationAmount = (50 * runAcceleration) / maxSpeed;
        runDeccelerationAmount = (50 * runDecceleration) / maxSpeed;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, maxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, maxSpeed);
    }

    private void Move()
    {
        var targetSpeed = moveInput * moveSpeed;
        float accelerate = 0;

        if (isGrounded)
        {
            rigidBody.velocity *= Vector2.up;
        }
        else
        {
            accelerate = (Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount : runDeccelerationAmount) * accelerationInAir;
        }

        var moveForce = (targetSpeed - rigidBody.velocity.x) * accelerate;
        rigidBody.AddForce(moveForce * Vector2.right, ForceMode2D.Force);
    }

    private void Jump(float jumpForce)
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        StartCoroutine(JumpSqueeze(0.8f, 1.15f, 0.05f, false));
    }

    private IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds, bool isLandingMoment)
    {
        if (isLandingMoment)
            anim.SetBool("landingMoment", true);
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
        if (isLandingMoment)
            anim.SetBool("landingMoment", false);
    }

    private bool CheckPosition(float xPos) => Mathf.Abs(transform.position.x - xPos) <= 3f;

    private void UpdateMovementAnimation()
    {
        anim.SetBool("isJumping", !isGrounded);
        anim.SetFloat("xVelocity", Mathf.Abs(rigidBody.velocity.x));
        anim.SetFloat("yVelocity", rigidBody.velocity.y);
            
        Flip();
    }

    private void Flip()
    {
        if (moveInput > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveInput < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private int ChooseDirection()
    {
        return Mathf.Abs(transform.position.x - leftPos.position.x) > Mathf.Abs(transform.position.x - rightPos.position.x)
                ? -1 : 1;
    }

    private void Attack()
    {
        attacksCount += 1;
        var state = Random.Range(0, 4);
        if (state < 2 && bossDamageCount < 3)
        {
            currentBot = MecroStates.form161;
            moveInput = ChooseDirection();
            if (state == 0 || attacksCount % 3 == 0)
                StartCoroutine(DarknessAttack());
            else
                StartCoroutine(FlashAttack());
        }
        else
        {
            currentBot = MecroStates.form296;
            if (state == 2 || attacksCount % 3 == 0)
            {
                moveInput = ChooseDirection();
                StartHardLanding();
            }
            else
            {
                moveInput = 0;
                StartCoroutine(StartBreakthrough());
            }
        }
        anim.SetBool("isAttacking", true);
        anim.SetInteger("botNumber", (int)currentBot);
    }

    private IEnumerator DarknessAttack()
    {
        anim.SetInteger("attackNumber", 0);
        yield return new WaitForSeconds(0.5f);

        Darkness.instance.TurnOn(true);
        HideInDarkness();
        yield return new WaitForSeconds(darknessAttackTime);

        anim.SetBool("isAttacking", false);
        (previousBot, currentBot) = (currentBot, MecroStates.none);
        Darkness.instance.TurnOn(false);
        ShowFromDarkness();
        if (attacksCount % 3 == 0)
            StartCoroutine(ShowFlask());
    }

    private IEnumerator FlashAttack()
    {
        anim.SetInteger("attackNumber", 1);
        yield return new WaitForSeconds(0.5f);

        flashEffect.enabled = true;
        Player.instance.InvertMovement(true);
        yield return new WaitForSeconds(flashAttackTime);

        anim.SetBool("isAttacking", false);
        (previousBot, currentBot) = (currentBot, MecroStates.none);
        flashEffect.enabled = false;
        Player.instance.InvertMovement(false);
    }

    private void StartHardLanding()
    {
        isHardLandingAttack = true;
        anim.SetInteger("attackNumber", 0);
    }

    private IEnumerator AccumulateHardLanding()
    {
        if (moveInput != 0)
        {
            moveInput = 0;
            rigidBody.gravityScale = 0;
            rigidBody.velocity = Vector2.zero;
            yield return new WaitForSeconds(1.5f);

            rigidBody.gravityScale = maxGravity * 2;
        }
    }

    private IEnumerator EndHardLanding()
    {
        isHardLandingAttack = false;
        anim.SetBool("isAttacking", false);
        (previousBot, currentBot) = (currentBot, MecroStates.none);
        ShowCeilingTraps();
        Player.instance.MiniJump(30f);
        TransformCollider(reducedColliderOffset, reducedColliderSize);
        if (attacksCount % 3 == 0)
            StartCoroutine(ShowFlask());
        yield return new WaitForSeconds(0.55f);

        TransformCollider(startColliderOffset, startColliderSize);
        yield return new WaitForSeconds(shownCeilingTrapsTime - 0.55f);
        StartCoroutine(
                HideCeilingTraps());
    }

    private IEnumerator StartBreakthrough()
    {
        isBreakthroughAttack = true;
        anim.SetInteger("attackNumber", 1);
        TransformCollider(extendedColliderOffset, extendedColliderSize);
        yield return new WaitForSeconds(2f);

        Jump(maxJumpForce);
        ShowFloorTraps();
        TransformCollider(startColliderOffset, startColliderSize);
        yield return new WaitForSeconds(shownFloorTrapsTime);
        StartCoroutine(
                HideFloorTraps());
    }

    private void EndBreakthrough()
    {
        isBreakthroughAttack = false;
        anim.SetBool("isAttacking", false);
        (previousBot, currentBot) = (currentBot, MecroStates.none);
    }

    private void HideInDarkness()
    {
        foreach (var sprite in spritesToHideInDarkness)
            sprite.sortingLayerID = 0;
    }

    private void ShowFromDarkness()
    {
        foreach (var sprite in spritesToHideInDarkness)
            sprite.sortingLayerName = "Enemies";
    }

    private void ShowCeilingTraps()
    {
        if (!isCeilingTrapsShown)
        {
            isCeilingTrapsShown = true;
            ceilingTraps.SetActive(true);
            StartCoroutine(ceilingTraps.MoveObjectSmoothly(ceilingTraps.transform.localPosition + Vector3.down * 4, 2f));
        }
    }

    private IEnumerator HideCeilingTraps()
    {
        if (isCeilingTrapsShown)
        {
            isCeilingTrapsShown = false; 
            StartCoroutine(ceilingTraps.MoveObjectSmoothly(ceilingTraps.transform.localPosition + Vector3.up * 4, 2f));
            yield return new WaitForSeconds(2.1f);

            ceilingTraps.SetActive(false);
        }
    }

    private void ShowFloorTraps()
    {
        if (!isFloorTrapsShown)
        {
            isFloorTrapsShown = true;
            groundTraps.SetActive(true);
            StartCoroutine(platforms.MoveObjectSmoothly(platforms.transform.localPosition + Vector3.up * 7, 1.5f));
            StartCoroutine(groundTraps.MoveObjectSmoothly(groundTraps.transform.localPosition + Vector3.up * 7, 1.5f));
        }
    }

    private IEnumerator HideFloorTraps()
    {
        if (isFloorTrapsShown)
        {
            isFloorTrapsShown = false;
            StartCoroutine(platforms.MoveObjectSmoothly(platforms.transform.localPosition + Vector3.down * 7, 1.5f));
            StartCoroutine(groundTraps.MoveObjectSmoothly(groundTraps.transform.localPosition + Vector3.down * 7, 1.5f));
            yield return new WaitForSeconds(1.6f);

            groundTraps.SetActive(false);
        }
    }

    private IEnumerator ShowFlask()
    {
        isFlaskHidden = false;
        currentFlask = bossDamageCount % 2 == 0 ? MecroStates.form161 : MecroStates.form296;
        anim.SetBool("isDamaged", true);
        anim.SetFloat("flaskNumber", (int)currentFlask);
        anim.SetInteger("damageCount", bossDamageCount / 2);
        Physics2D.IgnoreLayerCollision(enemyLayerMask, playerLayerMask, true);
        StartCoroutine(flask.MoveObjectSmoothly(currentFlask == MecroStates.form161 
            ? activeFlaskPosition : new Vector2(0.8f, 2.4f), 1f));
        yield return new WaitForSeconds(timeToDamageFlask);

        HideFlask();
    }

    private void HideFlask()
    {
        if (!isFlaskHidden)
        {
            previousBot = currentBot = MecroStates.none;
            anim.SetTrigger("recover");
            anim.SetBool("isDamaged", false);
            ableToTakeDamage = isFlaskHidden = true;
            StartCoroutine(flask.MoveObjectSmoothly(Vector3.zero, 1f));
            Physics2D.IgnoreLayerCollision(enemyLayerMask, playerLayerMask, false);
        }
    }

    private IEnumerator TakeDamage()
    {
        ableToTakeDamage = false;
        bossDamageCount += 1;
        anim.SetInteger("damageCount", (bossDamageCount + 1) / 2);

        if (bossDamageCount > 3)
        {
            StartCoroutine(TurnOff());
        }
        else
        {
            if (bossDamageCount > 2)
            {
                bot161.SetActive(false);
            }

            Player.instance.AddJumpForce(10f);
            yield return new WaitForSeconds(3f);

            HideFlask();
        }
    }

    private IEnumerator TurnOff()
    {
        isActive = false;
        isBossDefeated = true;
        anim.SetTrigger("destroy");
        rigidBody.velocity = Vector2.zero;
        rigidBody.gravityScale = 0;
        bossCollider.enabled = false;
        yield return new WaitForSeconds(bossDefeatTime);

        bossRoomEnter.SetActive(false);
        StartCoroutine(HideCeilingTraps());
        StartCoroutine(HideFloorTraps());
        SetActive(false);
    }

    private void DamagePlayer()
    {
        isActive = false;
        IsFightStarted = false;
        IsPlayerDefeated = true;
        ShowFromDarkness();
        Darkness.instance.TurnOn(false);
        Player.instance.InvertMovement(false);
        Physics2D.IgnoreLayerCollision(enemyLayerMask, playerLayerMask, false);
        Physics2D.IgnoreLayerCollision(enemyLayerMask, platformLayerMask, false);
        Player.instance.DamagePlayer();
    }

    public void RestoreInitialStates()
    {
        if (!isBossDefeated)
        {
            IsFightStarted = true;
            IsPlayerDefeated = false;
            isActive = true;
            ableToTakeDamage = true;
            isFlaskHidden = true;
            moveInput = 0;
            attacksCount = 0;
            bossDamageCount = 0;
            previousBot = currentBot = MecroStates.none;
            attackChangeTimer = 3f;
            betweenJumpTimer = timeBetweenJump;
            bossCollider.enabled = true;
            flashEffect.enabled = false;
            transform.position = respawnPoint.position;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            rigidBody.gravityScale = gravityScale;

            bot161.SetActive(true);
            Physics2D.IgnoreLayerCollision(enemyLayerMask, platformLayerMask, true);
            HideFlask();
            StartCoroutine(HideCeilingTraps());
            StartCoroutine(HideFloorTraps());
            SetActive(true);
        }
    }

    private void TransformCollider(Vector2 newOffset, Vector2 newSize)
    {
        bossCollider.offset = newOffset;
        bossCollider.size = newSize;
    }

    private void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(damagePos.Add(transform.position), damageSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.Add(transform.position), attackSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(attackAreaPos.Add(transform.position), attackAreaSize);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(feetPos.Add(transform.position), feetDetectorSize);
    }
}
