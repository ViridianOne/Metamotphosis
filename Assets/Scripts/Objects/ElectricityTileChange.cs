using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class ElectricityTileChange : MonoBehaviour, IPoolObject
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap map;
    [SerializeField] private GameObject grid;
    [SerializeField] private bool isVertical;
    [SerializeField] private int firstTilePos, lastTilePos;
    [SerializeField] private int firstFrame, lastFrame;
    [SerializeField] private AnimatedTile middleTile, firstTile, lastTile;
    [SerializeField] private int constTilePos; //x or y position that is same for every tile in current tilemap
    [SerializeField] private BoxCollider2D electricityCollider;

    [Header("Diodes")]
    [SerializeField] private Animator leftDiode;
    [SerializeField] private Animator rightDiode;
    [SerializeField] private float activeTime, disableTime, turningOnTime;
    private float activeTimer, disableTimer;
    private bool isActive;
    [SerializeField] private int animationLayer;
    [SerializeField] private Transform leftDroidPos, rightDroidPos;
    [SerializeField] private Light2D leftDroidLight, rightDroidLight;

    void Start()
    {
        activeTimer = 0;
        disableTimer = disableTime;
        SetAnimationLayer(animationLayer);
        for (int i = firstTilePos; i <= lastTilePos; i++)
        {
            if (i == firstTilePos)
                map.SetTile(new Vector3Int(i, constTilePos, 0), firstTile);
            else if (i == lastTilePos)
                map.SetTile(new Vector3Int(i, constTilePos, 0), lastTile);
            else
            {
                middleTile.m_AnimationStartFrame = Random.Range(firstFrame, lastFrame);
                map.SetTile(new Vector3Int(i, constTilePos, 0), middleTile);
            }
        }
        grid.SetActive(false);
    }

    
    void Update()
    {
        if (disableTimer > 0f)
        {
            disableTimer -= Time.deltaTime;
            isActive = false;
        }
        else if (disableTimer <= 0f && !isActive)
        {
            StartCoroutine(TurnOn());
        }

        if (activeTimer > 0f)
        {
            activeTimer -= Time.deltaTime;
            isActive = true;
        }
        else if (activeTimer <= 0f && isActive)
        {
            leftDiode.SetBool("isAttacking", false);
            rightDiode.SetBool("isAttacking", false);
            AudioManager.instance.Stop(16);
            grid.SetActive(false);
            disableTimer = disableTime;
        }
        leftDroidLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
        rightDroidLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
    }

    private IEnumerator TurnOn()
    {
        leftDiode.SetBool("isAttacking", true);
        rightDiode.SetBool("isAttacking", true);
        yield return new WaitForSeconds(turningOnTime);
        AudioManager.instance.Play(16);
        grid.SetActive(true);
        activeTimer = activeTime;
    }

    private void SetAnimationLayer(int index)
    {
        for (var i = 1; i < leftDiode.layerCount; i++)
        {
            leftDiode.SetLayerWeight(i, index == i ? 100 : 0);
            rightDiode.SetLayerWeight(i, index == i ? 100 : 0);
        }
    }

    public PoolObjectData GetObjectData()
    {
        return new PoolElectricityData(isVertical, middleTile, firstTile, lastTile, 
            firstTilePos, lastTilePos, constTilePos, animationLayer, 
            electricityCollider.size, electricityCollider.offset, leftDroidPos.position, rightDroidPos.position, 
            grid.transform.localRotation, leftDroidPos.localRotation, rightDroidPos.localRotation);
    }

    public void SetObjectData(PoolObjectData objectData)
    {
        var electricityData = objectData as PoolElectricityData;

        isVertical = electricityData.isVertical;
        middleTile = electricityData.middleTile;
        firstTile = electricityData.firstTile;
        lastTile = electricityData.lastTile;
        firstTilePos = electricityData.firstTilePos;
        lastTilePos = electricityData.lastTilePos;
        constTilePos = electricityData.constTilePos;
        animationLayer = electricityData.animationLayer;
        electricityCollider.size = electricityData.colliderSize;
        electricityCollider.offset = electricityData.colliderPos;
        leftDroidPos.position = electricityData.leftDroidPos;
        leftDroidPos.rotation = electricityData.leftDroidAngle;
        rightDroidPos.position = electricityData.rightDroidPos;
        rightDroidPos.rotation = electricityData.rightDroidAngle;
        grid.transform.localRotation = electricityData.gridRotationAngle;

        SetAnimationLayer(animationLayer);
    }
}
