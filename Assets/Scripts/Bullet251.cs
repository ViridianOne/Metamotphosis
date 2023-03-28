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
    private bool isShooted;
    [SerializeField] private BulletType type;

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
            print(directionCoef);
        }
    }

    protected void Move()
    {
        float targetSpeed = directionCoef * moveSpeed;
        float accelerate = 0;

        accelerate = Mathf.Abs(targetSpeed) > 0.01f ? runAccelerationAmount : runDeccelerationAmount;

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

    public void Shoot(int dirCoef)
    {
        directionCoef = dirCoef;
        isShooted = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") && !collision.CompareTag("Untagged"))
        {
            var rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                print("u did it");
            }
            isShooted = false;
            rigidBody.velocity = Vector2.zero;
            anim.SetTrigger("explode");
        }
    }
}
