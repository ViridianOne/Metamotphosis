using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AssemblyLine : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap map;
    [SerializeField] private GameObject grid;
    [SerializeField] private bool isVertical;
    [SerializeField] private int firstTilePos, lastTilePos;
    [SerializeField] private int firstFrame, lastFrame;
    [SerializeField] private AnimatedTile middleTile, firstTile, lastTile;
    [SerializeField] private int constTilePos; //x or y position that is same for every tile in current tilemap
    private bool isActive;
    //[SerializeField] private int animationLayer;
    // Start is called before the first frame update
    void Start()
    {
        //SetAnimationLayer(animationLayer);
        for (int i = firstTilePos; i <= lastTilePos; i++)
        {
            if (i == firstTilePos)
                map.SetTile(new Vector3Int(isVertical ? constTilePos : i, isVertical ? i : constTilePos, 0), firstTile);
            else if (i == lastTilePos)
                map.SetTile(new Vector3Int(isVertical ? constTilePos : i, isVertical ? i : constTilePos, 0), lastTile);
            else
            {
                //middleTile.m_AnimationStartFrame = Random.Range(firstFrame, lastFrame);
                map.SetTile(new Vector3Int(isVertical ? constTilePos : i, isVertical ? i : constTilePos, 0), middleTile);
            }
        }
        //grid.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        AudioManager.instance.Play(20);
    }
}
