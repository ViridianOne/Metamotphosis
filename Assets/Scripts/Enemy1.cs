using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    [Header("Animation")]
    private Animator anim;
    private bool isActive;
    [SerializeField] private GameObject holder;

    [Header("Physics")]
    private Collider2D enemyCollider;
    private Rigidbody2D rigidBody;
    private float gravity;
    [SerializeField] private float moveSpeed;
    private float movementForce;
    [SerializeField] private int moveDirection;
    private int lastMoveDirection;

    [Header("Electricity")]
    [SerializeField] private float creatingElectricityTime;
    [SerializeField] private GameObject electricity;
    [SerializeField] private Transform electricityPosition;

    [SerializeField] private float vanishTime;
    [SerializeField] private float vanishSpeed;


    void Start()
    {
        enemyCollider = GetComponent<Collider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        gravity = rigidBody.gravityScale;
        anim = GetComponent<Animator>();
        isActive = true;
        anim.SetBool("isMoving", false);
    }

    void Update()
    {
        if (isActive)
        {
            UpdateMovementAnimation();
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
        }
    }

    private void UpdateMovementAnimation()
    {
        anim.SetBool("isMoving", moveDirection != 0);
        Flip();
    }

    private void Flip()
    {
        if (moveDirection > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveDirection < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void Move()
    {
        movementForce = moveDirection * moveSpeed;
        rigidBody.velocity = new Vector2(movementForce, rigidBody.velocity.y);
    }

    public void ChangeMoveDirection()
    {
        moveDirection = -1 * moveDirection;
    }

    public void CreateElectricity()
    {
        if (isActive)
        {
            lastMoveDirection = moveDirection;
            moveDirection = 0;
            Instantiate(electricity, electricityPosition.position, electricityPosition.rotation);
            Invoke("CreatingElectricity", creatingElectricityTime);
        }
    }

    private void CreatingElectricity()
    {
        moveDirection = lastMoveDirection;
    }

    public void TakeDamage()
    {
        Player.instance.MiniJump(6f);
        if (isActive)
        {
            moveDirection = 0;
            isActive = false;
            anim.SetTrigger("damage");
            StartCoroutine(Vanishing());
        }
    }

    private IEnumerator Vanishing()
    {
        //yield return new WaitForSeconds(vanishTime);        
        //var sprite = holder.GetComponent<SpriteRenderer>();
        //var spriteColor = sprite.color;
        //var timer = 0f;
        //while (spriteColor.a >= 0) 
        //{
        //    timer += Time.deltaTime;
        //    spriteColor.a -= vanishSpeed * Time.deltaTime;
        //    sprite.color = spriteColor;
        //    yield return null;
        //}
        var isTakingDown = true;
        var sprite = holder.GetComponent<SpriteRenderer>();
        var timer = 0f;
        while (timer <= vanishTime)
        {
            var spriteColor = sprite.color;
            timer += Time.deltaTime;
            spriteColor.a += (isTakingDown ? -1 : 1) * vanishSpeed * Time.deltaTime;
            spriteColor.a = Mathf.Clamp(spriteColor.a, 0, 1);
            if (spriteColor.a == 0 || spriteColor.a == 1)
                isTakingDown = !isTakingDown;
            sprite.color = spriteColor;
            yield return null;
        }
        Destroy(gameObject);
    }
}
