using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class LaserBot : MonoBehaviour, IPoolObject
{
    [Header("Components")]
    private BoxCollider2D laserCollider;
    [SerializeField] private GameObject engine;
    [SerializeField] private GameObject laserBody;
    [SerializeField] private Light2D laserLight;

    [Header("Anim")]
    private Animator anim;
    private SpriteRenderer sprite;
    private SpriteRenderer engineSprite;
    private SpriteRenderer[] laserBodySprites;
    [SerializeField] private int animationLayer;
    [SerializeField] private Texture2D laserTexture;

    [Header("Movement")]
    private float velocityCoef = 1f;
    private Vector3 position1, position2;
    private Vector3 nextPosition;
    [SerializeField] private bool isMoving;
    [SerializeField] private Transform pos1, pos2, startPos;
    [SerializeField] private float movementSpeed;

    [Header("Attacking")]
    private bool isTurnOn = false;
    private bool isPlayerDamaged = false;
    [SerializeField] private float laserDistance;
    private Vector2 laserDirection;
    private Vector3 laserEndPosition;
    private ContactFilter2D layersToAttackCF;
    private int otherCollidersCount;
    private Collider2D[] otherColliders = new Collider2D[3];
    private RaycastHit2D raycastHit;
    private Vector3 vectorToEnd;
    private Vector3 laserStartFixVector;
    [SerializeField] private LayerMask layersToAttack;
    [SerializeField] private LayerMask layersToInterrupt;
    [SerializeField] private float turningOnTime;
    [SerializeField] private float activeTime, disableTime;
    [SerializeField] private Transform laserStart, laserEnd;


    void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        engineSprite = engine.GetComponent<SpriteRenderer>();
        laserBodySprites = laserBody.GetComponentsInChildren<SpriteRenderer>();
        laserCollider = laserBody.GetComponent<BoxCollider2D>();

        (position1, position2) = (pos1.position, pos2.position);
        laserEndPosition = laserEnd.localPosition;
    }

    private void Start()
    {
        nextPosition = startPos.position;
        laserDirection = laserEnd.position - laserStart.position;
        laserStartFixVector = new Vector3(0, laserDirection.y > 0 ? 0.01f : -0.01f, 0);
        layersToAttackCF = new ContactFilter2D { layerMask = layersToAttack };

        SetAnimationLayer(animationLayer);
        SetLaserSprites();
        laserBody.SetActive(false);

        StartCoroutine(TurnOn());
    }

    void Update()
    {
        if (isTurnOn)
        {
            if (!isPlayerDamaged)
            {
                otherCollidersCount = laserCollider.OverlapCollider(layersToAttackCF, otherColliders);
                if (otherCollidersCount > 0)
                {
                    StartCoroutine(CheckPlayerTouch());
                }
            }

            PlaceLaserBody();
        }

        if (isMoving)
        {
            if (transform.position == position1)
            {
                nextPosition = position2;
            }
            else if (transform.position == position2)
            {
                nextPosition = position1;
            }
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, movementSpeed * velocityCoef * Time.deltaTime);
        }
        laserLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
    }

    private void OnDrawGizmos()
    {
        if (isMoving)
        {
            Gizmos.DrawLine(pos1.position, pos2.position);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(laserStart.position, laserEnd.position);
    }

    private void SetLaserSprites()
    {
        var row = Mathf.Abs(animationLayer - 4) * 22f + 2f;
        for (var column = 0; column < 3; column++)
        {
            laserBodySprites[column].sprite = Sprite.Create(laserTexture, new Rect(column * 22f, row, 22f, 18f), new Vector2(0.5f, 0.5f), 22f);
        }
    }

    private void SetAnimationLayer(int index)
    {
        anim.SetFloat("animationLayer", index);
        for (var i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, index == i ? 100 : 0);
        }
    }

    private void PlaceLaserBody()
    {
        raycastHit = Physics2D.Raycast(laserStart.position + laserStartFixVector, laserDirection, laserDistance, layersToInterrupt);
        vectorToEnd = (raycastHit.point == Vector2.zero ? laserEnd.position.AsVector2() : raycastHit.point).Add(-laserStart.position);
        laserBody.transform.localScale = new Vector3(vectorToEnd.magnitude / 3f, 1f, 1f);
        laserBody.transform.position = laserStart.position.Add(new Vector3(vectorToEnd.x / 2f, vectorToEnd.y / 2f));
    }

    private IEnumerator TurnOn()
    {
        isTurnOn = true;
        anim.SetBool("isAttacking", true);
        yield return new WaitForSecondsRealtime(turningOnTime);

        laserBody.SetActive(true);
        AudioManager.instance.Play(23);
        yield return new WaitForSecondsRealtime(activeTime);

        AudioManager.instance.Stop(23);
        isTurnOn = false;
        anim.SetBool("isAttacking", false);
        laserBody.SetActive(false);
        yield return new WaitForSecondsRealtime(disableTime);

        StartCoroutine(TurnOn());
    }

    private IEnumerator CheckPlayerTouch()
    {
        for (var i = 0; i < otherCollidersCount; i++)
        {
            if (otherColliders[i].CompareTag("Player") && !otherColliders[i].isTrigger
                && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
            {
                isPlayerDamaged = true;
                Player.instance.DamagePlayer();
                yield return new WaitForSecondsRealtime(4f);

                isPlayerDamaged = false;
                yield break;
            }
        }
    }

    public IEnumerator SetVelocityCoef(float newVelocityCoef, Color newSpriteColor, float duration)
    {
        velocityCoef = newVelocityCoef;
        SetColor(newSpriteColor);
        yield return new WaitForSecondsRealtime(duration);

        velocityCoef = 1f;
        SetColor(new Color(1f, 1f, 1f, 1f));
    }

    private void SetColor(Color newSpriteColor)
    {
        sprite.color = newSpriteColor;
        engineSprite.color = newSpriteColor;
    }

    public PoolObjectData GetObjectData()
    {
        return new PoolLaserBotData(isMoving, position1, position2, startPos.position, 
            movementSpeed, laserEndPosition, activeTime, disableTime, animationLayer);
    }

    public void SetObjectData(PoolObjectData objectData)
    {
        var laserBotData = objectData as PoolLaserBotData;

        isMoving = laserBotData.isMoving;
        position1 = laserBotData.position1;
        position2 = laserBotData.position2;
        startPos.position = laserBotData.startPosition;
        movementSpeed = laserBotData.movementSpeed;
        laserEnd.localPosition = laserBotData.laserEndLocPosition;
        activeTime = laserBotData.activeTime;
        disableTime = laserBotData.disableTime;
        animationLayer = laserBotData.animationLayer;

        nextPosition = laserBotData.startPosition;
        laserDirection = laserEnd.position - laserStart.position;
        laserStartFixVector = new Vector3(0, laserDirection.y > 0 ? 0.01f : -0.01f, 0);

        isPlayerDamaged = false;
        SetAnimationLayer(animationLayer);
        SetLaserSprites();
        laserBody.SetActive(false);

        StartCoroutine(TurnOn());
    }
}
