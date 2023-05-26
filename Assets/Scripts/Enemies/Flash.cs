using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flash : Enemy
{
    [Header("Animation")]
    private SpriteRenderer holder2Sprite;
    [SerializeField] private GameObject holder2;

    [Header("Attack")]
    private bool isPlayerNear = false;
    private bool isAttacking = false, isExploding = false;
    private int attacksCount = 0;
    private float explosionTimer;
    [SerializeField] private float currentAttackRadius;
    [SerializeField] private float attackRadius, maxAttackRadius;
    [SerializeField] private Vector2 checkPlayerAreaSize;
    [SerializeField] private float minDistanceToPlayer, maxDistanceToPlayer;  // after teleportation
    [SerializeField] private float effectOnPlayerTime = 4f;
    [SerializeField] private float slowingSpeedChangeCoef, acceleratingSpeedChangeCoef;
    [SerializeField] private float delayAfterRelocating, explosionTime, returnTime , delayBetweenAttacks;

    private Tuple<Color, Color> changedColor;
    private readonly Dictionary<int, Tuple<string, string>> changedColors = new()
        { { 0, Tuple.Create("#FFE880", "#CCA800") },
          { 1, Tuple.Create("#80FFA9", "#00CC41") },
          { 2, Tuple.Create("#8097FF", "#0025CC") },
          { 3, Tuple.Create("#FF80D7", "#CC008C") } };

    protected override void Start()
    {
        base.Start();

        Color colorA, colorB;
        ColorUtility.TryParseHtmlString(changedColors[animationLayer].Item1, out colorA);
        ColorUtility.TryParseHtmlString(changedColors[animationLayer].Item2, out colorB);
        changedColor = Tuple.Create(colorA, colorB);

        holder2Sprite = holder2.GetComponent<SpriteRenderer>();
        holder2Sprite.enabled = false;

        currentAttackRadius = attackRadius;
    }

    private void Update()
    {
        if (isActive)
        {
            if (isExploding)
            {
                if (explosionTimer <= 1.2f)
                {
                    explosionTimer += Time.deltaTime;
                }
                currentAttackRadius = Mathf.Lerp(attackRadius, maxAttackRadius, explosionTimer / 1.2f);
            }
         
            if (!isAttacking)
            {
                isPlayerNear = Physics2D.OverlapBox(transform.position, checkPlayerAreaSize, 0f, masksToDamage);
                if (isPlayerNear && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
                {
                    StartCoroutine(AttackPlayer());
                }
            }

            canDamagePlayer = Physics2D.OverlapCircle(
                new Vector2(transform.position.x + attackPos.x, transform.position.y + attackPos.y), currentAttackRadius, masksToDamage);
            if (canDamagePlayer && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated && MecroSelectManager.instance.GetIndex() != 7)
            {
                StartCoroutine(DamagePlayer());
            }
        }
        enemyLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
    }

    protected override void Move() {}

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        state = EnemyState.Attack;

        StartCoroutine(Relocate());

        yield return new WaitForSeconds(delayAfterRelocating);

        if(attacksCount >= 2)
            TakeDamage();
        else
            StartCoroutine(Explode());

        yield return new WaitForSeconds(explosionTime);

        if (attacksCount < 2)
        {
            ReturnAfterExplosion();

            yield return new WaitForSeconds(returnTime);

            state = EnemyState.Idle;
            attacksCount++;
            yield return new WaitForSeconds(delayBetweenAttacks);
            StartCoroutine(AttackPlayer());
        }
    }

    private Vector2 GetNearToPlayerPosition()
    {
        var playerPos = Player.instance.transform.position;
        var xDifference = Random.Range(minDistanceToPlayer, maxDistanceToPlayer);
        var yDifference = Random.Range(minDistanceToPlayer, maxDistanceToPlayer);
        var xSign = Random.Range(0, 2);
        var ySign = Random.Range(0, 2);
        return new Vector2(playerPos.x + xDifference * (xSign == 0 ? 1 : -1), 
            playerPos.y + yDifference * (ySign == 0 ? 1 : -1));
    }

    private IEnumerator Relocate()
    {
        anim.SetTrigger("relocate");

        var newPos = GetNearToPlayerPosition();

        holder2Sprite.enabled = true;
        holder2.transform.position = newPos;

        yield return new WaitForSeconds(0.57f);

        holder.transform.position = transform.position;
        transform.position = newPos;
        holder2.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(0.43f);

        holder.transform.localPosition = Vector3.zero;
    }

    private IEnumerator Explode()
    {
        isExploding = true;
        anim.SetTrigger("attack");

        yield return new WaitForSeconds(1.2f);   // 72 frames

        isExploding = false;
        currentAttackRadius = 0;
        explosionTimer = 0;

        yield return new WaitForSeconds(explosionTime - 1.2f);

        ChangePlayerVelocity(Random.Range(0, 2));
    }

    private void ChangePlayerVelocity(int variety)
    {
        if (variety == 0)
        {
            Player.instance.ReactToFlashExplosion(effectOnPlayerTime, acceleratingSpeedChangeCoef, changedColor.Item1);
        }
        else
        {
            Player.instance.ReactToFlashExplosion(effectOnPlayerTime, slowingSpeedChangeCoef, changedColor.Item2);
        }
    }

    private void ReturnAfterExplosion()
    {
        anim.SetTrigger("return");
        currentAttackRadius = attackRadius;
    }

    protected override IEnumerator DamagePlayer()
    {
        Player.instance.DamagePlayer();
        yield return new WaitForSeconds(0f);
        attacksCount = 3;
    }

    private void TakeDamage()
    {
        isActive = false;
        state = EnemyState.Destroying;

        isExploding = false;
        currentAttackRadius = 0;
        explosionTimer = 0;

        ChangePlayerVelocity(Random.Range(0, 2));
        StartCoroutine(TurnOff());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + attackPos.x, transform.position.y + attackPos.y), attackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, checkPlayerAreaSize);
    }

    public override void Recover()
    {
        base.Recover();

        isAttacking = false;
        attacksCount = 0;
        currentAttackRadius = attackRadius;
    }
}
