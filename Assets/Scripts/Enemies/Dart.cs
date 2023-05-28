using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : Enemy
{
    [Header("Physics")]
    private bool isApproachingGround;
    private bool wasApproachingGround;
    private bool wasInvertedAtStart;
    [SerializeField] private bool isInverted;
    [SerializeField] private Vector2 groundDetecrotPos, groundDetectorSize;
    [SerializeField] private LayerMask groundMask;

    [Header("Attack")]
    private bool isAttacking = false;
    [SerializeField] private Transform topTriggerPos, bottomTriggerPos;
    [SerializeField] private Vector2 triggerDetectorSize;

    [Header("Damage")]
    [SerializeField] private Vector2 damagePos, damageSize;


    protected override void Start()
    {
        base.Start();

        wasInvertedAtStart = isInverted;
        rigidBody.gravityScale = gravityScale * (isInverted ? -1 : 1);
    }

    private void Update()
    {
        if (isActive)
        {
            if (!isAttacking
                && (isInverted && Physics2D.OverlapBox(bottomTriggerPos.position, triggerDetectorSize, 0f, masksAbleToDamage)
                || !isInverted && Physics2D.OverlapBox(topTriggerPos.position, triggerDetectorSize, 0f, masksAbleToDamage)) 
                && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
            {
                Attack();
            }

            wasApproachingGround = isApproachingGround;
            isApproachingGround = Physics2D.OverlapBox(new Vector2(transform.position.x + groundDetecrotPos.x,
                transform.position.y + groundDetecrotPos.y), groundDetectorSize, 0f, groundMask);
            if (isAttacking && !wasApproachingGround && isApproachingGround)
            {
                StartCoroutine(Land());
            }

            if (!isAttacking
                && Physics2D.OverlapBox(new Vector2(transform.position.x + damagePos.x, transform.position.y + damagePos.y),
                    damageSize, 0f, masksAbleToDamage)
                && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
            {
                TakeDamage();
            }

            if (isAttacking 
                && Physics2D.OverlapBox(new Vector2(transform.position.x + attackPos.x, transform.position.y + attackPos.y),
                    attackSize, 0f, masksToDamage)
                && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
            {
                StartCoroutine(DamagePlayer());
            }
            ChangeVelocity();
        }
        enemyLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
    }

    protected override void Move() {}

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

    private void Flip()
    {
        transform.localRotation = Quaternion.Euler(isInverted ? 0 : 180, 0, 0);
    }

    private void Attack()
    {
        isAttacking = true;
        isInverted = !isInverted;
        anim.SetTrigger("attack");
        rigidBody.gravityScale = gravityScale * (isInverted ? -1 : 1) * velocityCoef;
        if(!AudioManager.instance.sounds[22].source.isPlaying)
            AudioManager.instance.Play(22);
        StartCoroutine(JumpSqueeze(0.8f, 1.15f, 0.05f));
    }

    private IEnumerator Land()
    {
        anim.SetBool("landingMoment", true);
        AudioManager.instance.Play(30);
        yield return new WaitForSeconds(0.35f);  // длительность анимации gravity change
        anim.SetBool("landingMoment", false);
        isAttacking = false;
        Flip();
        InvertPlayerGravity();
        StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f));
    }

    private void TakeDamage()
    {
        isActive = false;
        anim.SetTrigger("damage");
        Player.instance.MiniJump(12f);
        AudioManager.instance.Play(6);
        StartCoroutine(TurnOff());
    }

    protected override IEnumerator DamagePlayer()
    {
        isActive = false;
        Player.instance.DamagePlayer();
        yield return new WaitForSeconds(1.5f);
        isActive = true;
    }

    private void InvertPlayerGravity()
    {
        Player.instance.InvertGravity();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(damagePos, damageSize);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(attackPos, attackSize);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireCube(feetPos, feetDetectorSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(topTriggerPos.position, triggerDetectorSize);
        Gizmos.DrawWireCube(bottomTriggerPos.position, triggerDetectorSize);
    }

    public override void Recover()
    {
        base.Recover();

        isInverted = wasInvertedAtStart;
        rigidBody.gravityScale = gravityScale * (wasInvertedAtStart ? -1 : 1);
        Flip();
    }
}
