using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Trampoline : MonoBehaviour, IPoolObject
{
    private bool lightsOn;
    private bool isSleeping;
    private bool isActiveZone;
    private bool effectSwitch = true;
    [SerializeField] private float jumpAdditionForce;
    [SerializeField] private Vector2 activeZonePos, activeZoneSize;
    [SerializeField] private LayerMask playerMask;
    
    private Animator anim;
    [SerializeField] private int animationLayer;

    [SerializeField] private Light2D objectLight;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        SetAnimationLayer(animationLayer);
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
        objectLight.intensity = LevelManager.instance.isDarknessOn && !isSleeping ? 1 : 0;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(lightsOn && effectSwitch 
            && collision.gameObject.CompareTag("Player") && MecroSelectManager.instance.GetIndex() != 2)
        {
            effectSwitch = false;
            Player.instance.AddJumpForce(jumpAdditionForce);
            anim.SetTrigger("jump");
            AudioManager.instance.Play(4);
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

    private void SetAnimationLayer(int index)
    {
        for (var i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, index == i ? 100 : 0);
        }
    }

    public PoolObjectData GetObjectData()
    {
        return new PoolTrampolineData(isSleeping, jumpAdditionForce, activeZonePos, activeZoneSize, animationLayer);
    }

    public void SetObjectData(PoolObjectData objectData)
    {
        var trampolineData = objectData as PoolTrampolineData;

        isSleeping = trampolineData.isSleeping;
        jumpAdditionForce = trampolineData.jumpAdditionForce;
        activeZonePos = trampolineData.activeZonePos;
        activeZoneSize = trampolineData.activeZoneSize;
        animationLayer = trampolineData.animationLayer;

        SetAnimationLayer(animationLayer);
    }
}
