using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    const float TILE_X_OFFSET = 1f;
    const float TILE_Z_OFFSET = 0.75f;

    [Header("Rules")]
    [SerializeField] private int mapWidth = 8;
    [SerializeField] private int mapHeight = 8;
    
    [SerializeField] private GameObject[] tileList;



    private void Start()
    {
        CreateTileMap();
    }

    private void CreateTileMap()
    {
        for(int x = 0; x < mapWidth; x++)
        {
            for(int z = 0; z < mapHeight; z++)
            {
                CreateTile(x, z);
            }
        }
    }

    private void CreateTile(int xCoordinate, int zCoordinate)
    {
        GameObject tile = Instantiate(tileList[0]);
        PlaceTile(tile, xCoordinate, zCoordinate);
        InitTile(tile, xCoordinate, zCoordinate);
    }

    private void PlaceTile(GameObject tile, int xCoordinate, int zCoordinate)
    {
        if (zCoordinate % 2 == 0)
        {
            tile.transform.position = new Vector3(xCoordinate * TILE_X_OFFSET, 0f, zCoordinate * TILE_Z_OFFSET);
        }
        else
        {
            tile.transform.position = new Vector3(xCoordinate * TILE_X_OFFSET + TILE_X_OFFSET / 2, 0f, zCoordinate * TILE_Z_OFFSET);
        }
    }

    private void InitTile(GameObject tile, int xCoordinate, int zCoordinate)
    {
        tile.GetComponent<TileController>().Init();
        tile.transform.parent = transform;
        tile.name = xCoordinate.ToString() + ", " + zCoordinate.ToString();
    }
}
