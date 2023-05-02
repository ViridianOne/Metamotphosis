using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_new : MonoBehaviour, IPoolObject
{
    private bool lightsOn;
    private bool isSleeping;
    private bool isActiveZone;
    private Vector3 nextPos;
    private Vector3 position1, position2;
    [SerializeField] private bool isMoving;
    [SerializeField] private Transform pos1, pos2;
    [SerializeField] private Transform startPos;
    [SerializeField] private Vector2 activeZonePos, activeZoneSize;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float speed;
    [HideInInspector] public float velocityChangeTime;
    [HideInInspector] public float velocityCoef;

    [SerializeField] private int animationLayer;
    [HideInInspector] public Animator anim;
    [HideInInspector] public SpriteRenderer sprite;

    private void Awake()
    {
        position1 = pos1.position;
        position2 = pos2.position;
        nextPos = startPos.position;
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetAnimationLayer(animationLayer);
        anim.SetFloat("sleepingCoef", isSleeping ? 1 : 0);
        anim.SetBool("isSleeping", isSleeping);
        lightsOn = !isSleeping;
    }

    void Update()
    {
        isActiveZone = Physics2D.OverlapBox(activeZonePos, activeZoneSize, 0, playerMask);
        if (isActiveZone && isSleeping && MecroSelectManager.instance.GetIndex() == 0)
        {
            lightsOn = MecroSelectManager.instance.instantiatedMecros[MecroSelectManager.instance.GetIndex()].isAbilityActivated;
            anim.SetFloat("sleepingCoef", lightsOn ? 0 : 1);
            anim.SetBool("isSleeping", !lightsOn);
        }
        if (isMoving && isActiveZone && lightsOn)
        {
            if (transform.position == position1 || nextPos == position2)
            {
                nextPos = position2;
            }
            if (transform.position == position2 || nextPos == position1)
            {
                nextPos = position1;
            }
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * velocityCoef * Time.deltaTime);
        }
        ChangeVelocity();
    }

    private void OnDrawGizmos()
    {
        if (isMoving)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(pos1.position, pos2.position);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(activeZonePos, activeZoneSize);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        anim.SetTrigger("impulse");
    //        anim.SetBool("isPlayerOnPlatform", lightsOn);
    //        //if (nextPos == pos1.position && pos1.position.y != pos2.position.y)
    //        //{
    //        //    Player.instance.isOnMovingPlatform = true;
    //        //    Player.instance.movingPlatDif = new Vector2(0, 0.005f);
    //        //}
    //        //else if (nextPos == pos2.position && pos2.position.y != pos1.position.y)
    //        //{
    //        //    Player.instance.isOnMovingPlatform = true;
    //        //    Player.instance.movingPlatDif = new Vector2(0, -0.005f);
    //        //}
    //        collision.transform.SetParent(transform);
    //    }
    //}
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Player" && (collision.gameObject.activeInHierarchy || MecroSelectManager.instance.isChanged))
    //    {
    //        anim.SetBool("isPlayerOnPlatform", false);
    //        collision.transform.SetParent(null);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetTrigger("impulse");
            anim.SetBool("isPlayerOnPlatform", lightsOn);
            //if (nextPos == pos1.position && pos1.position.y != pos2.position.y)
            //{
            //    Player.instance.isOnMovingPlatform = true;
            //    Player.instance.movingPlatDif = new Vector2(0, 0.005f);
            //}
            //else if (nextPos == pos2.position && pos2.position.y != pos1.position.y)
            //{
            //    Player.instance.isOnMovingPlatform = true;
            //    Player.instance.movingPlatDif = new Vector2(0, -0.005f);
            //}
            collision.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && (collision.gameObject.activeInHierarchy || MecroSelectManager.instance.isChanged))
        {
            anim.SetBool("isPlayerOnPlatform", false);
            collision.transform.SetParent(null);
        }
    }

    private void SetAnimationLayer(int index)
    {
        for (var i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, index == i ? 100 : 0);
        }
    }

    protected void ChangeVelocity()
    {
        if (velocityChangeTime > 0)
            velocityChangeTime -= Time.deltaTime;
        else if (velocityChangeTime <= 0 && velocityCoef != 1)
        {
            velocityCoef = 1;
            anim.speed = 1;
            sprite.color = new Color(1, 1, 1, 1);
        }
    }

    public PoolObjectData GetObjectData()
    {
        return new PoolPlatformData(position1, position2, startPos.position, isMoving, isSleeping, 
            animationLayer, activeZonePos, activeZoneSize);
    }

    public void SetObjectData(PoolObjectData objectData)
    {
        var platformData = objectData as PoolPlatformData;

        transform.position = platformData.startPosition;
        position1 = platformData.position1;
        position2 = platformData.position2;
        nextPos = platformData.startPosition == platformData.position1 ? 
            platformData.position2 : platformData.position1;
        isMoving = platformData.isMoving;
        isSleeping = platformData.isSleeping;
        animationLayer = platformData.animationLayer;
        activeZonePos = platformData.activeZonePos;
        activeZoneSize = platformData.activeZoneSize;

        SetAnimationLayer(animationLayer);
    }
}
