using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMoving : MonoBehaviour, IPoolObject
{
    private Vector3 nextPos;
    private Vector3 position1, position2;
    private CircleCollider2D circleCollider;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isSmall;
    [SerializeField] private Transform pos1, pos2;
    [SerializeField] private Transform startPos;
    [SerializeField] private float speed;

    private Animator anim;
    private SpriteRenderer spriteRender;
    [SerializeField] private Sprite bigSawSprite, smallSawSprite;

    private void Awake()
    {
        position1 = pos1.position;
        position2 = pos2.position;
        nextPos = startPos.position;
        anim = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
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
            if (transform.position == position1)
            {
                nextPos = position2;
            }
            if (transform.position == position2)
            {
                nextPos = position1;
            }
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (isMoving)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(pos1.position, pos2.position);
        }
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
        GetComponent<SpriteRenderer>().sprite = isSmall ? smallSawSprite : bigSawSprite;
    }

    public PoolObjectData GetObjectData()
    {
        return new PoolSawData(position1, position2, startPos.position, isMoving, isSmall);
    }

    public void SetObjectData(PoolObjectData objectData)
    {
        var sawData = objectData as PoolSawData;

        transform.position = sawData.startPosition;
        position1 = sawData.position1;
        position2 = sawData.position2;
        nextPos = sawData.startPosition == sawData.position1 ?
            sawData.position2 : sawData.position1;
        isMoving = sawData.isMoving;
        isSmall = sawData.isSmall;

        UpdateSize();
    }

    private void UpdateSize()
    {
        spriteRender.sprite = isSmall ? smallSawSprite : bigSawSprite;
        anim.SetBool("isSmall", isSmall);
        circleCollider.radius = isSmall ? 1.25f : 2.5f;
    }
}
