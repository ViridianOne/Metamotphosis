using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DestroyingPlatform : MonoBehaviour, IPoolObject
{
    [Header("Components")]
    [SerializeField] private Collider2D[] colliders;
    private PlatformEffector2D effector;
 
    [Header("Anim")]
    private Animator anim;
    private SpriteRenderer sprite;
    [SerializeField] private int animationLayer;

    [Header("Destroying")]
    private bool isPlayerTouchedPlatform = false;
    private bool isRecovering = false;
    private int currentPhase = 0;
    private float phaseTimer;
    private float recoverTimer;
    [SerializeField] private float phaseTime;
    [SerializeField] private float timeToRecover;

    [SerializeField] private Light2D objectLight;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        effector = GetComponent<PlatformEffector2D>();
    }

    private void Start()
    {
        SetAnimationLayer(animationLayer);
        Recover();
    }

    void Update()
    {
        if (isPlayerTouchedPlatform)
        {
            if (isRecovering)
            {
                if (recoverTimer > 0)
                    recoverTimer -= Time.deltaTime;
                else if (recoverTimer <= 0)
                    Recover();
            }
            else
            {
                if (phaseTimer > 0)
                    phaseTimer -= Time.deltaTime;
                else if (phaseTimer <= 0)
                {
                    currentPhase += 1;
                    anim.SetTrigger("impulse");
                    anim.SetFloat("phase", currentPhase);

                    if (currentPhase < 3)
                        phaseTimer = phaseTime;
                    else
                        Destroy();
                }
            }
        }
        objectLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerTouchedPlatform = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            anim.SetTrigger("impulse");
        }
    }

    private void SetAnimationLayer(int index)
    {
        for (var i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, index == i ? 100 : 0);
        }
    }

    private void Recover()
    {
        if (isRecovering)
            anim.SetTrigger("recover");
        isRecovering = false;
        isPlayerTouchedPlatform = false;
        foreach (var collider in colliders)
            collider.enabled = true;
        effector.enabled = true;
        anim.SetFloat("phase", 0);
        currentPhase = 0;
        phaseTimer = phaseTime;
        recoverTimer = timeToRecover;
    }

    private void Destroy()
    {
        isRecovering = true;
        foreach (var collider in colliders)
            collider.enabled = false;
        effector.enabled = false;
        recoverTimer = timeToRecover;
    }

    public PoolObjectData GetObjectData()
    {
        return new PoolDestroyingPlatformData(phaseTime, timeToRecover, animationLayer);
    }

    public void SetObjectData(PoolObjectData objectData)
    {
        var destroyingPlatformData = objectData as PoolDestroyingPlatformData;

        phaseTime = destroyingPlatformData.phaseTime;
        timeToRecover = destroyingPlatformData.timeToRecover;
        animationLayer = destroyingPlatformData.animationLayer;

        Recover();
        SetAnimationLayer(animationLayer);
    }
}
