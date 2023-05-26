using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PressingBlock : MonoBehaviour, IPoolObject
{
    [Header("Physics")]
    [SerializeField] private float gravity;
    [SerializeField] private float climbSpeed;
    private Rigidbody2D rigidBody;

    [Header("Attack")]
    [SerializeField] Transform blockIdlePos, detectorPos, damagePos;
    [SerializeField] Vector3 detectorSize, damageSize;
    [SerializeField] LayerMask playerMask;
    [SerializeField] float triggerTime, landingTime;
    private bool isPlayerUnderneath, canDamagePlayer, canAttack, canClimb, isClimbing, ableToDamege;

    [Header("Animation")]
    [SerializeField] private int animationLayer;
    private Animator anim;

    [Header("Rendering")]
    [SerializeField] private Light2D objectLight;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if(animationLayer != 1)
        {
            anim.SetLayerWeight(animationLayer, 100);
            anim.SetLayerWeight(1, 0);
        }
        else
            anim.SetLayerWeight(1, 100);
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        canAttack = true;
        ableToDamege = true;
    }

    
    void Update()
    {
        isPlayerUnderneath = Physics2D.OverlapBox(detectorPos.position, detectorSize, 0, playerMask);

        canDamagePlayer = ableToDamege && Physics2D.OverlapBox(damagePos.position, damageSize, 0, playerMask);

        if(canAttack && isPlayerUnderneath)
        {
            StartCoroutine(Attack());
        }

        if(canClimb)
        {
            StartCoroutine(Climb());
        }

        if(isClimbing)
        {
            anim.SetBool("isLanded", false);
            transform.position = Vector3.MoveTowards(transform.position, blockIdlePos.position, climbSpeed * Time.deltaTime);
        }

        if(isClimbing && blockIdlePos.position.y - transform.position.y < 0.1f && blockIdlePos.position.y - transform.position.y > -0.1f)
        {
            rigidBody.gravityScale = 0;
            rigidBody.velocity = Vector2.zero;
            transform.position = blockIdlePos.position;
            anim.SetBool("isAttacking", false);
            isClimbing = false;
            canAttack = true;
            ableToDamege = true;
        }

        if (canDamagePlayer)
        {
            ableToDamege = false;
            Physics2D.IgnoreLayerCollision(6, 7, true);
            Player.instance.DamagePlayer();
            rigidBody.velocity = Vector2.zero;
        }
        objectLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
    }

    private IEnumerator Attack()
    {
        anim.SetBool("isAttacking", true);

        yield return new WaitForSeconds(triggerTime);

        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        rigidBody.gravityScale = gravity;
        canAttack = false;
        canClimb = false;
    }

    private IEnumerator Climb()
    {
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        rigidBody.velocity = Vector2.zero;
        anim.SetBool("isLanded", true);

        yield return new WaitForSeconds(landingTime);

        canClimb = false;
        isClimbing = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(detectorPos.position, detectorSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(damagePos.position, damageSize);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform")
        {
            canClimb = true;
            landingTime = 2f;
        }
        else if(collision.gameObject.tag == "Player" && MecroSelectManager.instance.GetIndex() == 7)
        {
            canClimb = true;
            landingTime = 0;
        }
    }

    public PoolObjectData GetObjectData()
    {
        return new PoolPressingBlock(detectorSize, transform.position, detectorPos.position, gravity, 
            climbSpeed, triggerTime, landingTime, animationLayer);
    }

    public void SetObjectData(PoolObjectData objectData)
    {
        var pressingBlockData = objectData as PoolPressingBlock;

        detectorSize = pressingBlockData.detectorSize;
        transform.position = pressingBlockData.blockPos;
        detectorPos.position = pressingBlockData.detectorPos;
        gravity = pressingBlockData.gravity;
        climbSpeed = pressingBlockData.climbSpeed;
        triggerTime = pressingBlockData.triggerTime;
        landingTime = pressingBlockData.landingTime;
        animationLayer = pressingBlockData.animationLayer;
    }
}
