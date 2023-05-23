using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IPoolObject
{
    private Animator anim;
    private SpriteRenderer sprite;
    private Animator explosionAnim;
    private SpriteRenderer explosionSprite;
    private PointEffector2D pointEffector;
    private CircleCollider2D pointEffectorCollider;
    [SerializeField] private int animationLayer;
    [SerializeField] private GameObject explosionObject;

    private bool isExploding = false;
    [SerializeField] private LayerMask layersToAttack;
    [SerializeField] private float explosionDelay;
    [SerializeField] private float explosionTime;
    [SerializeField] private float explosionReclainForce;
    [SerializeField] private float lethalExplosionRadius, explosionReclainRadius;
    [SerializeField] private Vector2 triggerAreaPos, triggerAreaSize;


    void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        pointEffector = GetComponent<PointEffector2D>();
        pointEffectorCollider = GetComponent<CircleCollider2D>();

        SetAnimationLayer(animationLayer);
    }

    void Update()
    {
        if (!isExploding)
        {
            if (Physics2D.OverlapBox(transform.position.Add(triggerAreaPos), triggerAreaSize, 0f, layersToAttack))
            {
                StartCoroutine(Explode());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position.Add(triggerAreaPos), triggerAreaSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lethalExplosionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionReclainRadius);
    }

    private IEnumerator Explode()
    {
        isExploding = true;
        yield return new WaitForSecondsRealtime(explosionDelay);
        
        if (Physics2D.OverlapCircle(transform.position, lethalExplosionRadius, layersToAttack))
        {
            Player.instance.DamagePlayer();
        }
        else
        {
            var vectorToPlayer = Player.instance.transform.position - transform.position;
            Player.instance.AddForce(Mathf.Clamp(explosionReclainRadius - vectorToPlayer.magnitude, 0, explosionReclainRadius) 
                / explosionReclainRadius * explosionReclainForce * vectorToPlayer);
        }
        anim.SetTrigger("explode");
        anim.SetBool("isDisabled", true);
        explosionObject.SetActive(true);
        yield return new WaitForSecondsRealtime(explosionTime);

        explosionObject.SetActive(false);
        //gameObject.SetActive(false);
    }

    private void SetAnimationLayer(int index)
    {
        for (var i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, index == i ? 100 : 0);
        }
    }

    public PoolObjectData GetObjectData()
    {
        return new PoolBombData(animationLayer, explosionDelay, explosionReclainForce,
            lethalExplosionRadius, explosionReclainRadius, triggerAreaPos, triggerAreaSize);
    }

    public void SetObjectData(PoolObjectData objectData)
    {
        var bombData = objectData as PoolBombData;

        animationLayer = bombData.animationLayer;
        explosionDelay = bombData.timeBeforeExplode;
        explosionReclainForce = bombData.explosionReclainForce;
        lethalExplosionRadius = bombData.lethalExplosionRadius;
        explosionReclainRadius = bombData.explosionReclainRadius;
        triggerAreaPos = bombData.triggerAreaPos;
        triggerAreaSize = bombData.triggerAreaSize;

        isExploding = false;
        anim.SetBool("isDisabled", false);
        explosionObject.SetActive(false);
        SetAnimationLayer(animationLayer);
    }
}
