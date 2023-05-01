using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMoving : MonoBehaviour
{
    [SerializeField] bool isMoving;
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;
    private Vector3 nextPos;
    [SerializeField] bool isSmall;
    private Animator anim;
    private CircleCollider2D circleCollider;

    //[SerializeField] private SpriteRenderer holder;
    [SerializeField] private Sprite bigSawSprite, smallSawSprite;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        anim.SetBool("isSmall", isSmall);
        circleCollider.radius = isSmall ? 1.25f : 2.5f;
    }

    private void Update()
    {
        if (isMoving)
        {
            if (transform.position == pos1.position)
            {
                nextPos = pos2.position;
            }
            if (transform.position == pos2.position)
            {
                nextPos = pos1.position;
            }
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(pos1.position, pos2.position);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && MecroSelectManager.instance.GetIndex() != 7)
        {
            Player.instance.DamagePlayer();
        }
    }

    private void OnValidate()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = isSmall ? smallSawSprite : bigSawSprite;
    }
}
