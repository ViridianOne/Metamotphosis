using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private float jumpAdditionForce;
    private Animator anim;
    public bool isSleeping;
    private bool lightsOn;
    [SerializeField] private Vector2 activeZonePos, activeZoneSize;
    [SerializeField] private LayerMask playerMask;
    private bool isActiveZone;
    private bool effectSwitch = true;
    [SerializeField] private int animationLayer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (animationLayer != 1)
        {
            anim.SetLayerWeight(1, 0);
            anim.SetLayerWeight(animationLayer, 100);
        }
        anim.SetBool("isSleeping", isSleeping);
        lightsOn = !isSleeping;
    }

    void Update()
    {
        isActiveZone = Physics2D.OverlapBox(activeZonePos, activeZoneSize, 0, playerMask);
        if (isActiveZone && isSleeping && MecroSelectManager.instance.GetIndex() == 0)
        {
            lightsOn = MecroSelectManager.instance.instantiatedMecros[MecroSelectManager.instance.GetIndex()].isAbilityActivated;
            anim.SetBool("isSleeping", !lightsOn);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(lightsOn && effectSwitch 
            && collision.gameObject.CompareTag("Player") && MecroSelectManager.instance.GetIndex() != 2)
        {
            effectSwitch = false;
            Player.instance.AddJumpForce(jumpAdditionForce);
            anim.SetTrigger("jump");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (lightsOn && collision.gameObject.CompareTag("Player"))
            effectSwitch = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(activeZonePos, activeZoneSize);
    }
}
