using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public EnemyType type;
    protected EnemyState state;

    [Header("Anim")]
    [HideInInspector] public Animator anim;
    [SerializeField] protected GameObject holder;
    [HideInInspector] public SpriteRenderer holderSprite;
    private Color spriteColor;
    private bool isFaded;
    [SerializeField] private float fadingTime;
    [SerializeField] private float fadingFrameTime;
    private float fadingTimer;
    private float fadingFrameTimer;
    [SerializeField] protected int animationLayer;

    [Header("Psycics")]
    protected Rigidbody2D rigidBody;
    protected Collider2D enemyCollider;
    [SerializeField] protected float gravityScale = 4f;
    [HideInInspector] public float velocityCoef;
    [HideInInspector] public float velocityChangeTime;

    [Header("Damage")]
    protected bool isActive = true;
    [HideInInspector] public bool isDefeated = false;
    [SerializeField] protected Transform respawnPoint;
    [SerializeField] private float damageTime;
    [SerializeField] protected LayerMask masksAbleToDamage, masksToDamage;

    [Header("Attack")]
    protected bool canDamagePlayer;
    [SerializeField] protected Vector2 attackPos, attackSize;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        holderSprite = holder.GetComponent<SpriteRenderer>();
        spriteColor = holderSprite.color;
    }

    protected virtual void Start()
    {
        state = EnemyState.Idle;
        if (animationLayer == 0)
            animationLayer = 1;
        for (int i = 1; i < anim.layerCount; i++)
        {
            if (i == animationLayer)
                anim.SetLayerWeight(i, 1);
            else
                anim.SetLayerWeight(i, 0);
        }
    }

    protected abstract void Move();

    protected abstract IEnumerator DamagePlayer();

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    protected void ChangeVelocity()
    {
        if (velocityChangeTime > 0)
            velocityChangeTime -= Time.deltaTime;
        else if (velocityChangeTime <= 0 && velocityCoef != 1)
        {
            velocityCoef = 1;
            anim.speed = 1;
            holderSprite.color = new Color(1, 1, 1, 1);
        }
    }

    //protected abstract void TakeDamage();
    protected IEnumerator TurnOff()
    {
        anim.SetTrigger("damage");
        rigidBody.velocity = Vector2.zero;
        rigidBody.gravityScale = 0;
        enemyCollider.enabled = false;
        yield return new WaitForSeconds(damageTime);
        if (type != EnemyType.Spark && type != EnemyType.Flash)
        {
            fadingTimer = fadingTime;
            fadingFrameTimer = 0;
            while (fadingTimer > 0)
            {
                spriteColor = holderSprite.color;
                if (fadingFrameTimer <= 0)
                {
                    isFaded = !isFaded;
                    fadingFrameTimer = fadingFrameTime;
                }
                spriteColor.a = isFaded ? 0 : 1;
                holderSprite.color = spriteColor;
                fadingTimer -= Time.deltaTime;
                fadingFrameTimer -= Time.deltaTime;
                yield return null;
            }
        }
        isDefeated = true;
        SetActive(false);
    }

    public virtual void Recover()
    {
        anim.SetTrigger("recover");

        isActive = true;
        isDefeated = false;
        transform.position = respawnPoint.position;
        rigidBody.gravityScale = gravityScale;
        enemyCollider.enabled = true;
        
        state = EnemyState.Idle;
        SetActive(true);
    }
}
