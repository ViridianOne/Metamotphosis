using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_new : MonoBehaviour
{
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;
    private Vector3 nextPos;
    [HideInInspector] public Animator anim;
    public bool isSleeping;
    private bool lightsOn;
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
        lightsOn = !isSleeping;
    }

    void Update()
    {
        isActiveZone = Physics2D.OverlapBox(activeZonePos, activeZoneSize, 0, playerMask);
        if(isActiveZone && isSleeping && MecroSelectManager.instance.GetIndex() == 0)
        {
            lightsOn = MecroSelectManager.instance.instantiatedMecros[MecroSelectManager.instance.GetIndex()].isAbilityActivated;
            anim.SetFloat("sleepingCoef", lightsOn ? 0 : 1);
            anim.SetBool("isSleeping", !lightsOn);
        }
        if (isActiveZone && lightsOn)
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
        if(velocityCoef != 1)
            StartCoroutine(ChangeVelocity(velocityChangeTime));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(pos1.position, pos2.position);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(activeZonePos, activeZoneSize);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetTrigger("impulse");
            anim.SetBool("isPlayerOnPlatform", lightsOn);
            if (nextPos == pos1.position && pos1.position.y != pos2.position.y)
            {
                Player.instance.isOnMovingPlatform = true;
                Player.instance.movingPlatDif = new Vector2(0, 0.005f);
            }
            else if (nextPos == pos2.position && pos2.position.y != pos1.position.y)
            {
                Player.instance.isOnMovingPlatform = true;
                Player.instance.movingPlatDif = new Vector2(0, -0.005f);
            }
            collision.collider.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && (collision.gameObject.activeInHierarchy || MecroSelectManager.instance.isChanged))
        {
            anim.SetBool("isPlayerOnPlatform", false);
            collision.collider.transform.SetParent(null);
        }
    }

    public IEnumerator ChangeVelocity(float effectTime)
    {
        yield return new WaitForSeconds(effectTime);
        velocityCoef = 1;
        anim.speed = 1;
        sprite.color = new Color(1, 1, 1, 1);
    }
}
