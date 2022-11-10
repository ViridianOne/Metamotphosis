using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class Mecro71 : Player
{
    [Header("Jumping")]
    private bool isGrounded;
    [SerializeField] private Transform feetPos;
    [SerializeField] private float radius;
    [SerializeField] private Vector2 feetDetectorSize;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpForce;
    private bool isJumping = false;
    [SerializeField] private float jumpDelay = 0.25f;
    private float jumpTimer;
    public GameObject holder;
    private bool wasOnGround;
    private bool lightSwitcher = false;
    public float acceleration = 0.5f;
    public bool isFlying;
    public int jumpCount = 0;
    public float defaultMoveSpeed; //equal to starting MoveSpeed
    /*public bool movingRight = false;
    public bool movingLeft = false;
    public bool movingUp = false;
    public bool movingDown = false;*/
    public string directionArrow;
    private float rotationSpeed = 720;
    public bool directionChosen = false;

    private bool isCeilingHit;
    [SerializeField] private Transform headPos;                 //for collision detection
    [SerializeField] private Vector2 headDetectorSize;

    protected override void Move()
    {
        //if (moveSpeed > maxSpeed)
            //moveSpeed = maxSpeed;
        //while(moveSpeed<maxSpeed)
            //moveSpeed += acceleration * Time.deltaTime;
        //rigidBody.velocity = new Vector2(movementForce, rigidBody.velocity.y);
        //moveSpeed = Mathf.MoveTowards(moveSpeed, maxSpeed, acceleration);
        //if (movingRight == true && isFlying)
       //rigidBody.velocity = new Vector2(moveSpeed, rigidBody.velocity.y); neeeded maybe
        // else if (movingLeft == true && isFlying)
        //rigidBody.velocity = new Vector2(-moveSpeed, rigidBody.velocity.y);
        switch (directionArrow)
        {
            case "right":
                rigidBody.velocity = new Vector2(moveSpeed, rigidBody.velocity.y);
                break;
            case "left":
                rigidBody.velocity = new Vector2(-moveSpeed, rigidBody.velocity.y);
                break;
            case "up":
                transform.rotation = Quaternion.Euler(Vector3.forward * 90);
                rigidBody.velocity = new Vector2(0, moveSpeed);
                break;
            case "down":
                transform.rotation = Quaternion.Euler(Vector3.forward * -90);
                rigidBody.velocity = new Vector2(0, -moveSpeed);
                break;
        }
    }

    private void ChoosingDirection()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            directionArrow = "right";
            directionChosen = true;

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            directionArrow = "left";
            moveInput = Input.GetAxisRaw("Horizontal");
            directionChosen = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            directionArrow = "down";
            directionChosen = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            directionArrow = "up";
            directionChosen = true;
        }
    }


    private void Update()
    {
        //if (jumpCount == 0) with space
        if (directionChosen == false)
            ChoosingDirection();
        if (isActive)
        {
            if (isAbleToMove)
            {
                //if (Input.GetButtonDown("Jump"))
                //{ 
                // Move(); 
                //}
                //moveInput = Input.GetAxisRaw("Horizontal");
                UpdateMovementAnimation();
            }
            UpdateLedegGrabbing();
            if (isTouchingLedge)
                isGrounded = false;

            var damageCount = 0;
            isCeilingHit = Physics2D.OverlapBox(headPos.position, headDetectorSize, 0f, groundMask);     //for collision detection
            if (isCeilingHit)
            {
                DamagePlayer();
                //Player.instance.StartCoroutine(Respawn());
                directionArrow = "";
                damageCount = 1;                
            }
                //var jump_count = 0;
                // Starting flying with Space
                /*if (Input.GetButtonDown("Jump") && jumpCount == 0)  //bool and count for knowing if it's start of flying
                {
                    isFlying = true;
                    jumpCount = 1;
                    //Move();
                    //if (moveSpeed < maxSpeed)
                    //moveSpeed += acceleration * Time.deltaTime;
                }
                else if (Input.GetButtonDown("Jump") && jumpCount == 1)
                {
                    isFlying = false;
                    jumpCount = 0;
                }
                if (isFlying)
                {
                    Move();
                    if (moveSpeed < maxSpeed)
                        moveSpeed += acceleration * Time.deltaTime;
                    anim.SetBool("isFlying", true);
                }
                else
                {
                    anim.SetBool("isFlying", false);
                    moveSpeed = defaultMoveSpeed;
                    rigidBody.velocity = new Vector2(0, 0);
                }*/

                //Starting flying right after arrow (with directionChosen)
            if (directionChosen)
                isFlying = true;
            if (Input.GetButtonDown("Jump") || damageCount == 1)
            {
                isFlying = false;
                directionChosen = false;
            }
            if (isFlying)
            {
                Move();
                if (moveSpeed < maxSpeed)
                    moveSpeed += acceleration * Time.deltaTime;
                anim.SetBool("isFlying", true);
            }
            else
            {
                anim.SetBool("isFlying", false);
                moveSpeed = defaultMoveSpeed;
                rigidBody.velocity = new Vector2(0, 0);
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(ledgePos1, ledgePos2);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(feetPos.position, radius);
        Gizmos.color = Color.magenta;
        //Gizmos.DrawRay(transform.position, transform.right);
        Gizmos.DrawWireCube(feetPos.position, feetDetectorSize);
    }

    private void UpdateMovementAnimation()
    {
        if (Input.GetButtonDown("Jump"))
        {
            //transform.localScale = new Vector3(1f, 1f, 1f);
            anim.SetBool("isFlying", true);
        }
        else if (Input.GetButtonDown("Jump"))
        {
            //transform.localScale = new Vector3(-1f, 1f, 1f);
            anim.SetBool("isFlying", true);
        }
        else
        {
            anim.SetBool("isFlying", false);
        }
        Flip();
    }

    /*private void FixedUpdate()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Move();
        }
    }*/

            private void Flip()
    {
        if (moveInput > 0f)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            isFacingRight = true;
        }
        else if (moveInput < 0f)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            isFacingRight = false;
        }
    }
}
