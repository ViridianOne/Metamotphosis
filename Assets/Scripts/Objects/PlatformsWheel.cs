using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlatformsWheel : MonoBehaviour, IPoolObject
{
    [Header("Main")]
    private bool isLightsOn;
    private int mecroIndex;
    private int platformsCount;
    private Vector3[] platformsPositions;
    [SerializeField] private bool isSleeping;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private GameObject wheelObject;
    [SerializeField] private GameObject wheelBotObject;
    [SerializeField] private GameObject[] platforms;
 
    [Header("Anim")]
    private Animator wheelAnim;
    private SpriteRenderer wheelSprite;
    private SpriteRenderer wheelBotSprite;
    private SpriteRenderer[] platformsSprites;
    [SerializeField] private int animationLayer;

    [Header("Rotation")]
    private float[] rotationAngles;
    private float platformSpeed;
    private int clockwiseCoef;
    [SerializeField] private bool isClockwiseRotation;
    [SerializeField] private float radius;
    [SerializeField] private float rotationSpeed;

    [Header("Movement")]
    private float velocityCoef;
    private Vector3 nextPosition;
    private Vector3 position1, position2, startPosition;
    [SerializeField] private bool isMoving;
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform pos1, pos2, startPos;

    [Header("Rendering")]
    [SerializeField] private Light2D objectLight;


    private void Awake()
    {
        wheelAnim = wheelObject.GetComponent<Animator>();
        wheelSprite = wheelObject.GetComponent<SpriteRenderer>();
        wheelBotSprite = wheelBotObject.GetComponent<SpriteRenderer>();

        platformsCount = platforms.Length;
        platformSpeed = platformsCount == 2 ? 1.292f : 1.292f;
        platformsSprites = new SpriteRenderer[platformsCount];
        platformsPositions = new Vector3[platformsCount];
        rotationAngles = new float[platformsCount];
        for (var i = 0; i < platformsCount; i++)
        {
            platformsSprites[i] = platforms[i].GetComponentInChildren<SpriteRenderer>();
            platformsPositions[i] = platforms[i].transform.localPosition;
            rotationAngles[i] = CalculateStartAngle(platforms[i].transform.localPosition) / rotationSpeed / platformSpeed;
        }
        clockwiseCoef = isClockwiseRotation ? -1 : 1;

        position1 = pos1.position;
        position2 = pos2.position;
        nextPosition = startPos.position;
    }

    void Start()
    {
        isLightsOn = !isSleeping;
        velocityCoef = 1f;
        wheelAnim.SetBool("isSleeping", isSleeping);
        wheelAnim.SetFloat("platformsCount", platformsCount);
        SetAnimationLayer(animationLayer);
        SetAnimRotationSpeed(isSleeping ? 0 : rotationSpeed);

        // test
        // StartCoroutine(testVelocityChange());
    }

    private IEnumerator testVelocityChange()
    {
        yield return new WaitForSecondsRealtime(3f);
        StartCoroutine(SetVelocityCoef(0.5f, Color.red, 5f));
    }

    void Update()
    {
        mecroIndex = MecroSelectManager.instance.GetIndex();
        if (isSleeping && mecroIndex == (int)MecroStates.form161)
        {
            isLightsOn = MecroSelectManager.instance.instantiatedMecros[mecroIndex].isAbilityActivated;
            wheelAnim.SetBool("isSleeping", !isLightsOn);
            SetAnimRotationSpeed(isLightsOn ? rotationSpeed * velocityCoef : 0);
        }
        if (isLightsOn)
        {
            if (isMoving)
            {
                if (transform.position == position1)
                {
                    nextPosition = position2;
                }
                else if (transform.position == position2)
                {
                    nextPosition = position1;
                }
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, movementSpeed * velocityCoef * Time.deltaTime);
            }

            for (var i = 0; i < platformsCount; i++)
            {
                rotationAngles[i] += Time.deltaTime * clockwiseCoef;
                RotatePlatform(platforms[i], rotationAngles[i] * rotationSpeed * velocityCoef * platformSpeed);
            }
        }
        objectLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
    }

    private void OnDrawGizmos()
    {
        if (isMoving)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(pos1.position, pos2.position);
        }
    }

    private void SetAnimationLayer(int index)
    {
        for (var i = 1; i < wheelAnim.layerCount; i++)
        {
            wheelAnim.SetLayerWeight(i, index == i ? 100 : 0);
        }
    }

    private void SetAnimRotationSpeed(float newRotationSpeed)
    {
        wheelAnim.SetFloat("rotationSpeedCoef", newRotationSpeed * clockwiseCoef);
    }

    private float CalculateStartAngle(Vector2 platformPos)
    {
        return Mathf.Atan2(platformPos.y, platformPos.x);
    }

    private void RotatePlatform(GameObject platform, float angle)
    {
        platform.transform.localPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }

    public IEnumerator SetVelocityCoef(float newVelocityCoef, Color newSpriteColor, float duration)
    {
        for (var i = 0; i < rotationAngles.Length; i++)
        {
            rotationAngles[i] = rotationAngles[i] / newVelocityCoef;
        }
        velocityCoef = newVelocityCoef;
        SetAnimRotationSpeed(rotationSpeed * newVelocityCoef);
        SetColor(newSpriteColor);
        yield return new WaitForSecondsRealtime(duration);

        for (var i = 0; i < rotationAngles.Length; i++)
        {
            rotationAngles[i] = rotationAngles[i] * velocityCoef;
        }
        velocityCoef = 1f;
        SetAnimRotationSpeed(rotationSpeed);
        SetColor(new Color(1f, 1f, 1f, 1f));
    }

    private void SetColor(Color newSpriteColor)
    {
        wheelSprite.color = newSpriteColor;
        wheelBotSprite.color = newSpriteColor;
        foreach (var platformSprite in platformsSprites)
            platformSprite.color = newSpriteColor;
    }

    public PoolObjectData GetObjectData()
    {
        return new PoolPlatformsWheelData(isMoving, isSleeping, clockwiseCoef, rotationSpeed,
             movementSpeed, position1, position2, startPosition, platformsPositions, animationLayer);
    }

    public void SetObjectData(PoolObjectData objectData)
    {
        var platformsWheelData = objectData as PoolPlatformsWheelData;

        isMoving = platformsWheelData.isMoving;
        isSleeping = platformsWheelData.isSleeping;
        clockwiseCoef = platformsWheelData.clockwiseCoef;
        rotationSpeed = platformsWheelData.rotationSpeed;
        movementSpeed = platformsWheelData.movementSpeed;
        position1 = platformsWheelData.position1;
        position2 = platformsWheelData.position2;
        nextPosition = platformsWheelData.startPosition == platformsWheelData.position1 ?
            platformsWheelData.position2 : platformsWheelData.position1;
        animationLayer = platformsWheelData.animationLayer;

        isLightsOn = !isSleeping;
        velocityCoef = 1f;
        wheelAnim.SetBool("isSleeping", isSleeping);
        wheelAnim.SetFloat("platformsCount", platformsCount);
        SetAnimationLayer(animationLayer);
        SetAnimRotationSpeed(isSleeping ? 0 : rotationSpeed);
        SetColor(new Color(1f, 1f, 1f, 1f));

        rotationAngles = new float[platformsCount];
        for (var i = 0; i < platformsCount; i++)
        {
            platforms[i].transform.localPosition = platformsWheelData.platformsPositions[i];
            rotationAngles[i] = CalculateStartAngle(platforms[i].transform.localPosition) / rotationSpeed / platformSpeed;
        }
    }
}

