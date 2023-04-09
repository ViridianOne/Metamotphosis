using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTest : MonoBehaviour
{
    [SerializeField]Tilemap map;
    [SerializeField]TileBase tileBase;

    [SerializeField] Vector3Int[] positions;
    [SerializeField] TileBase[] tileBases;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
            map.SetTile(new Vector3Int(1, 1, 0), tileBase);
        if(Input.GetKeyDown(KeyCode.U))
            map.SetTiles(positions, tileBases);
    }
}
