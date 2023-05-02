using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_light_new : MonoBehaviour
{
    [SerializeField] private bool isMoving;
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;
    private Vector3 nextPos;
    [HideInInspector] public Animator anim;
    public bool isSleeping;
    [SerializeField] private bool lightsOn = false;
    [SerializeField] private Vector2 activeZonePos, activeZoneSize;
    [SerializeField] private LayerMask playerMask;
    private bool isActiveZone;
    [SerializeField] private int animationLayer;
    [HideInInspector] public float velocityChangeTime;
    [HideInInspector] public float velocityCoef;
    [HideInInspector] public SpriteRenderer sprite;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        nextPos = pos1.position;
        if (animationLayer != 1)
        {
            anim.SetLayerWeight(1, 0);
            anim.SetLayerWeight(animationLayer, 100);
        }
        anim.SetFloat("sleepingCoef", isSleeping ? 1 : 0);
        anim.SetBool("isSleeping", isSleeping);
        //lightsOn = !isSleeping;
    }

    void Update()
    {
        isActiveZone = Physics2D.OverlapBox(activeZonePos, activeZoneSize, 0, playerMask);
        if (MecroSelectManager.instance.GetIndex() == 0)
        {
            lightsOn = MecroSelectManager.instance.instantiatedMecros[MecroSelectManager.instance.GetIndex()].isAbilityActivated;
        }
        if (isActiveZone && isSleeping)
        {
            //lightsOn = MecroSelectManager.instance.instantiatedMecros[MecroSelectManager.instance.GetIndex()].isAbilityActivated;
            anim.SetFloat("sleepingCoef", lightsOn ? 0 : 1);
            anim.SetBool("isSleeping", !lightsOn);
            //anim.SetBool("isAwakening", lightsOn);
        }
        if (isMoving && isActiveZone && lightsOn)
        {
            if (transform.position == pos1.position || nextPos == pos2.position)
            {
                nextPos = pos2.position;
            }
            if (transform.position == pos2.position || nextPos == pos1.position)
            {
                nextPos = pos1.position;
            }
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * velocityCoef * Time.deltaTime);
        }
        ChangeVelocity();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(pos1.position, pos2.position);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(activeZonePos, activeZoneSize);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetTrigger("impulse");
            anim.SetBool("isPlayerOnPlatform", lightsOn);
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
}
