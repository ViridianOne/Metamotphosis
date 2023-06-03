using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    SlowDown = -1,
    SpeedUp = 1
}

public class Bullet251 : MonoBehaviour
{
    [Header("Physics")]
    private Rigidbody2D rigidBody;
    private Animator anim;
    [SerializeField] private float moveSpeed, maxSpeed;
    [SerializeField] private float runAcceleration;
    [SerializeField] private float runDecceleration;
    [SerializeField] private float runAccelerationAmount;
    [SerializeField] private float runDeccelerationAmount;
    private int directionCoef;
    private Vector2 forceDirection;

    [Header("Attack")]
    private bool isShooted;
    [SerializeField] private BulletType type;
    [SerializeField] private float explosionTime, effectTime;
    [SerializeField] private float speedChangeCoef;
    [SerializeField] private Color changedColor;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        
    }


    private void FixedUpdate()
    {
        if(isShooted)
        {
            Move();
        }
    }

    protected void Move()
    {
        float targetSpeed = directionCoef * moveSpeed;
        float accelerate = 0;

        accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount : runDeccelerationAmount;

        float moveForce = (targetSpeed - rigidBody.velocity.x) * accelerate;
        rigidBody.AddForce(moveForce * forceDirection, ForceMode2D.Force);
    }

    private void OnValidate()
    {
        runAccelerationAmount = (50 * runAcceleration) / maxSpeed;
        runDeccelerationAmount = (50 * runDecceleration) / maxSpeed;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, maxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, maxSpeed);
    }

    public void Shoot(int dirCoef, Directions direction)
    {
        directionCoef = dirCoef;
        Flip(direction);
        isShooted = true;
        anim.SetBool("isRecovered", true);
    }

    private void Flip(Directions direction)
    {
        switch (direction)
        {
            case Directions.up:
                transform.localRotation = Quaternion.Euler(0, 0, 90);
                forceDirection = Vector2.up;
                break;
            case Directions.right:
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                forceDirection = Vector2.right;
                break;
            case Directions.down:
                transform.localRotation = Quaternion.Euler(0, 0, -90);
                forceDirection = Vector2.up;
                break;
            case Directions.left:
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                forceDirection = Vector2.right;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.velocityChangeTime = effectTime;
                enemy.velocityCoef = speedChangeCoef;
                enemy.anim.speed = speedChangeCoef;
                enemy.holderSprite.color = changedColor;
                StartCoroutine(ExplodeBullet());
            }
        }
        else if (collision.CompareTag("Platform"))
        {
            var platfrom = collision.GetComponent<Platform_new>();
            if (platfrom != null)
            {
                platfrom.velocityChangeTime = effectTime;
                platfrom.velocityCoef = speedChangeCoef;
                platfrom.anim.speed = speedChangeCoef;
                platfrom.sprite.color = changedColor;
                StartCoroutine(ExplodeBullet());
            }
        }
        else if (collision.CompareTag("Ground"))
            StartCoroutine(ExplodeBullet());
    }

    private IEnumerator ExplodeBullet()
    {
        isShooted = false;
        rigidBody.velocity = Vector2.zero;
        anim.SetBool("isRecovered", false);
        anim.SetTrigger("explode");
        AudioManager.instance.Play(type == BulletType.SpeedUp ? 21 : 22);
        yield return new WaitForSeconds(explosionTime);
        gameObject.SetActive(false);
    }
}
