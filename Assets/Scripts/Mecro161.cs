using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecro161 : Player
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
    //private bool lightSwitcher = false;
    //[SerializeField] private GameObject lightEffect;

    protected override void Move()
    {
        movementForce = moveInput * moveSpeed;
        rigidBody.velocity = new Vector2(movementForce, rigidBody.velocity.y);
        //rigidBody.AddForce(Vector2.right * movementForce);
        //rigidBody.AddForce(Vector2.right * direction.x * moveSpeed);
        //if (Mathf.Abs(rigidBody.velocity.x) > maxSpeed)
        //{
        //    rigidBody.velocity = new Vector2(Mathf.Sign(rigidBody.velocity.x) * maxSpeed, rigidBody.velocity.y);
        //}
    }

    private void Update()
    {
        if (isActive)
        {
            if (isAbleToMove)
            {
                moveInput = Input.GetAxisRaw("Horizontal");
                if (Input.GetButtonDown("Fire1"))
                {
                    lightSwitcher = !lightSwitcher;
                    if (lightSwitcher)
                    {
                        anim.SetLayerWeight(1, 0);
                        anim.SetLayerWeight(2, 100);
                        AudioManager.instance.Play(1);
                    }
                    else
                    {
                        anim.SetLayerWeight(1, 100);
                        anim.SetLayerWeight(2, 0);
                        AudioManager.instance.Play(2);
                    }
                }
                wasOnGround = isGrounded;
                //isGrounded = Physics2D.OverlapCircle(feetPos.position, radius, groundMask);
                isGrounded = Physics2D.OverlapBox(feetPos.position, feetDetectorSize, 0f, groundMask);
                if (!wasOnGround && isGrounded)
                {
                    StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f));
                }
                if (isGrounded && Input.GetButtonDown("Jump"))
                {
                    //isJumping = true;
                    jumpTimer = Time.time + jumpDelay;
                }
                if (!isGrounded)
                {
                    anim.SetBool("isJumping", true);
                    //runSound.Stop();
                }
                else
                {
                    anim.SetBool("isJumping", false);
                    anim.SetBool("landingMoment", false);
                }
                if (rigidBody.velocity.y <= 0 && !isGrounded)
                {
                    anim.SetBool("landingMoment", true);
                    AudioManager.instance.Play(8);
                }
                UpdateMovementAnimation();
            }
            UpdateLedegGrabbing();
            if (isTouchingLedge)
            {
                isGrounded = false;
            }
        }
        //else
        //{
        //    lightSwitcher = false;
        //    anim.SetLayerWeight(1, 100);
        //    anim.SetLayerWeight(2, 0);
        //}
        /*if (isActive && !runSound.isPlaying)
        {
            runSound.Play();
        }*/
        if ((moveInput == 0 || !isGrounded) && AudioManager.instance.sounds[9].source.isPlaying)
        {
            AudioManager.instance.Stop(9);
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
        if (moveInput > 0f)
        {
            //transform.localScale = new Vector3(1f, 1f, 1f);
            anim.SetBool("isMoving", true);
        }
        else if (moveInput < 0f)
        {
            //transform.localScale = new Vector3(-1f, 1f, 1f);
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
        if (moveInput!=0f && !AudioManager.instance.sounds[9].source.isPlaying)
        {
            AudioManager.instance.Play(9);
        }
        Flip();
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
            if (jumpTimer > Time.time)
                jumpTimer -= Time.deltaTime;
            if (jumpTimer <= Time.time && jumpTimer > 0f)
            {
                Jump();
            }
        }
    }

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

    public void Jump()
    {
        //if (isJumping)
        //{
        //    //rigidBody.velocity = Vector2.up * jumpForce;
        //    rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        //    rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //}
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //isJumping = false;
        jumpTimer = 0;
        StartCoroutine(JumpSqueeze(0.8f, 1.15f, 0.05f));
    }

    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originSize.z);
        float timer = 0f;
        while(timer <= 1f)
        {
            timer += Time.deltaTime / seconds;
            holder.transform.localScale = Vector3.Lerp(originSize, newSize, timer);
            yield return null;
        }
        timer = 0;
        while(timer <= 1f)
        {
            timer += Time.deltaTime / seconds;
            holder.transform.localScale = Vector3.Lerp(newSize, originSize, timer);
            yield return null;
        }
    }

    protected override void StopMoving()
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("landingMoment", false);
        anim.SetBool("isLedgeGrabbing", false);
        rigidBody.velocity = Vector2.zero;
        AudioManager.instance.Stop(9);
    }

    public override void DisableAbility()
    {
        lightSwitcher = false;
        anim.SetLayerWeight(1, 100);
        anim.SetLayerWeight(2, 0);
    }
}
