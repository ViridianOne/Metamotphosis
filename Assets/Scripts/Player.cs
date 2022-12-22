using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Player : MonoBehaviour
{
    public static Player instance;

    protected Animator anim;
    protected bool isFacingRight;

    protected Collider2D playerCollider;
    [SerializeField] float respawnTime;
    private float respawnTimer;
    [HideInInspector] public Transform respawnPoint;
    protected bool isActive;
    [HideInInspector] public bool lightSwitcher = false;

    [Header("Physics")]
    protected Rigidbody2D rigidBody;
    protected float gravity;
    [SerializeField] protected float moveSpeed;
    protected float movementForce;
    protected float moveInput;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float linearDrag;
    protected Vector2 direction;
    protected bool isAbleToMove = true;

    [Header("Ledge Grabbing")]
    [HideInInspector] public bool isTouchingLedge;
    private bool canClimbLedge;
    protected bool ledgeDetected;
    private Vector2 ledgePos1, ledgePos2;
    private float ledgeGrabbingTimer;
    [SerializeField] private float ledgeGrabbingTime;
    [SerializeField] private Vector3 difference1, difference2, difference3, difference4;

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        gravity = rigidBody.gravityScale;
        isActive = true;
        anim.SetBool("isLedgeGrabbing", false);
        anim.SetBool("isMoving", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("landingMoment", false);
        anim.SetBool("isFlying", false);
    }

    protected abstract void Move();

    protected void UpdateLedegGrabbing() 
    {
        if (isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
        }
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;
            anim.SetBool("isLedgeGrabbing", true);
        }
        if (canClimbLedge)
        {
            transform.position = ledgePos1;
            if (ledgeGrabbingTimer <= 0)
                ledgeGrabbingTimer = ledgeGrabbingTime;
            else
            {
                ledgeGrabbingTimer -= Time.deltaTime;
            }
            if (ledgeGrabbingTimer <= 0)
                FinishLedgeGrabbing();
        }
    }

    private void FinishLedgeGrabbing()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        ledgeDetected = false;
        isAbleToMove = true;
        isTouchingLedge = false;
        anim.SetBool("isLedgeGrabbing", false);
    }

    public void GrabLedge(Vector3 grabPos, bool isRight, float coefficient)
    {
        isAbleToMove = false;
        if (!isRight && !isFacingRight || isRight && isFacingRight)
        {
            if (!isRight)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
        isTouchingLedge = true;
        if (!isRight)
        {
            ledgePos1 = grabPos - difference1;
            ledgePos2 = grabPos + difference2;
        }
        else
        {
            ledgePos1 = grabPos - difference3;
            ledgePos2 = grabPos + difference4;
        }
    }

    public void MiniJump(float miniJumpForce) 
    {
        rigidBody.velocity = new Vector2(0, 0);
        rigidBody.AddForce(Vector2.up * miniJumpForce, ForceMode2D.Impulse);
    }

    public void DamagePlayer()
    {
        AudioManager.instance.Play(3);
        anim.SetBool("isDamaged", true);
        anim.SetTrigger("damage");
        isActive = false;
        playerCollider.enabled = false;
        MiniJump(12f);
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        rigidBody.gravityScale = 4;
        yield return new WaitForSeconds(0.3f);
        rigidBody.gravityScale = 0;
        yield return new WaitForSeconds(respawnTime - 0.3f);
        if (!isActive)
        {
            rigidBody.gravityScale = gravity;
            transform.position = respawnPoint.position;
            isActive = true;
            playerCollider.enabled = true;
            anim.SetBool("isDamaged", false);
        }
    }

    protected abstract void StopMoving();

    public void ToggleActive(bool state)
    {
        isActive = state;
        /*anim.SetBool("isMoving", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("landingMoment", false);
        anim.SetBool("isFlying", false);*/
        if (!state)
        {
            StopMoving();
        }
    }

    public abstract void DisableAbility();
}
