using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChemicalDrop : MonoBehaviour, IPoolObject
{
    [Header("Animation")]
    [SerializeField] float animationLayer;
    private Animator anim;

    [Header("Drop")]
    [SerializeField] private GameObject drop;
    [SerializeField] private Rigidbody2D dropRB;
    [SerializeField] private float dropRadius;
    [SerializeField] private Vector3 dropOffset;
    [SerializeField] private float disableTime, splashTime, turnOnTime;
    private float disableTimer;
    private bool isActive;

    [Header("Effect")]
    [SerializeField] LayerMask playerMask;
    [SerializeField] private Transform upper, down;
    [SerializeField] Vector2 upperDetectorSize, downDetectorSize;
    [SerializeField] Vector3 upperDetectorOffset, downDetectorOffset;
    [SerializeField] Color newPlayerColor;
    [SerializeField] float effectTime;
    private bool isTouchingChemical, isTouchingDrop;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        anim.SetFloat("animationLayer", animationLayer);
        isActive = false;
        drop.SetActive(false);
        drop.transform.position = upper.position;
    }

    void Update()
    {
        isTouchingChemical = Physics2D.OverlapBox(upper.position + upperDetectorOffset, upperDetectorSize, 0, playerMask)
            || Physics2D.OverlapBox(down.position + downDetectorOffset, downDetectorSize, 0, playerMask);
        isTouchingDrop = Physics2D.OverlapCircle(drop.transform.position + dropOffset, dropRadius, playerMask) && isActive;
        if (drop.transform.position.y <= down.position.y || isTouchingDrop)
            StartCoroutine(Splash());

        if (!isActive)
        {
            if (disableTimer <= 0)
                StartCoroutine(TurnOn());
            else
                disableTimer -= Time.deltaTime;
        }

        if(isTouchingChemical || isTouchingDrop)
            StartCoroutine(Player.instance.ReactToChemical(effectTime, newPlayerColor));
    }

    private IEnumerator Splash()
    {
        dropRB.gravityScale = 0;
        dropRB.velocity = Vector2.zero;
        anim.SetBool("isSplashing", true);
        yield return new WaitForSeconds(splashTime);
        if (isActive)
        {
            AudioManager.instance.Play(31);
            drop.SetActive(false);
            anim.SetBool("isSplashing", false);
            drop.transform.position = upper.position;
            isActive = false;
            disableTimer = disableTime;
        }
    }

    private IEnumerator TurnOn()
    {
        if (!isActive)
        {
            AudioManager.instance.Play(17);
            dropRB.gravityScale = 2;
            drop.SetActive(true);
            anim.SetBool("isFalling", true);
            isActive = true;
        }
        yield return new WaitForSeconds(turnOnTime);
        anim.SetBool("isFalling", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(down.position + downDetectorOffset, downDetectorSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(upper.position + upperDetectorOffset, upperDetectorSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(drop.transform.position + dropOffset, dropRadius);
    }

    public PoolObjectData GetObjectData()
    {
        return new PoolChemical(animationLayer, newPlayerColor, upper.position, down.position);
    }

    public void SetObjectData(PoolObjectData objectData)
    {
        var chemical = objectData as PoolChemical;

        animationLayer = chemical.animationLayer;
        newPlayerColor = chemical.changedColor;
        upper.position = chemical.upperPos;
        down.position = chemical.downPos;

        anim.SetFloat("animationLayer", animationLayer);
    }
}
