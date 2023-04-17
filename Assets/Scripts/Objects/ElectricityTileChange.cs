using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ElectricityTileChange : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap map;
    [SerializeField] private GameObject grid;
    [SerializeField] private bool isVertical;
    [SerializeField] private int firstMiddleTilePos, lastMiddleTilePos;
    [SerializeField] private int firstFrame, lastFrame;
    [SerializeField] private AnimatedTile middleTile;
    [SerializeField] private int constTilePos; //x or y position that is same for every tile in current tilemap 

    [Header("Diodes")]
    [SerializeField] private Animator leftDiode;
    [SerializeField] private Animator rightDiode;
    [SerializeField] private float activeTime, disableTime, turningOnTime;
    private float activeTimer, disableTimer;
    private bool isActive;
    [SerializeField] private int animationLayer;

    void Start()
    {
        activeTimer = 0;
        disableTimer = disableTime;
        if (animationLayer != 1)
        {
            leftDiode.SetLayerWeight(animationLayer, 100);
            leftDiode.SetLayerWeight(1, 0);
            rightDiode.SetLayerWeight(animationLayer, 100);
            rightDiode.SetLayerWeight(1, 0);
        }
        for (int i = firstMiddleTilePos; i <= lastMiddleTilePos; i++)
        {
            middleTile.m_AnimationStartFrame = Random.Range(firstFrame, lastFrame);
            map.SetTile(new Vector3Int(isVertical ? constTilePos : i, isVertical ? i : constTilePos, 0), middleTile);
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
            grid.SetActive(false);
            disableTimer = disableTime;
        }
    }

    private IEnumerator TurnOn()
    {
        leftDiode.SetBool("isAttacking", true);
        rightDiode.SetBool("isAttacking", true);
        yield return new WaitForSeconds(turningOnTime);
        grid.SetActive(true);
        activeTimer = activeTime;
    }
}
