using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    private Tilemap tilemap;
    private Tilemap.TilemapObject.TilemapSprite tilemapSprite;

    public Transform[] enviromentPrefabs;
    public GameObject enviromentPrefabHolder;

    private Pathfinding pathfinding;


    private void Awake()
    {
        enviromentPrefabs = enviromentPrefabHolder.GetComponent<EnviromentHolder>().enviromentPrefabs;
    }

    // Start is called before the first frame update
    void Start()
    {
        tilemap = new Tilemap(18, 10, 1f, new Vector3(0,0));
        pathfinding = new Pathfinding(18, 10, 1f, new Vector3(0, 0));
        CreateWorld();
        InstantiateWorld();
        /*CreateWorld();

        for (int x = 0; x < tilemap.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < tilemap.GetGrid().GetHeight(); y++)
            {
                Tilemap.TilemapObject tilemapObject = tilemap.GetGrid().GetGridObject(x, y);
                if (tilemapObject.GetTilemapSprite() == Tilemap.TilemapObject.TilemapSprite.Ground)
                {
                    Instantiate(enviromentPrefabs[0], tilemap.GetGrid().GetWorldPosition(x, y) + new Vector3(1f, 1f) * .5f, Quaternion.identity);
                }
            }
        }*/
    }

    public void CreateWorld()
    {
        for (int x = 0; x < tilemap.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < tilemap.GetGrid().GetHeight(); y++)
            {
                tilemap.spriteRaycast(x, y);
                //tilemap.SetTilemapSprite(new Vector3(x, y), Tilemap.TilemapObject.TilemapSprite.Ground);
            }
        }
    }

    public void InstantiateWorld()
    {
        for (int x = 0; x < tilemap.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < tilemap.GetGrid().GetHeight(); y++)
            {
                if((int)tilemap.GetGrid().GetGridObject(x, y).GetTilemapSprite() != 0)
                    Instantiate(enviromentPrefabs[(int)tilemap.GetGrid().GetGridObject(x, y).GetTilemapSprite() - 1], tilemap.GetGrid().GetWorldPosition(x, y) + new Vector3(1f, 1f) * .5f, Quaternion.identity, this.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        /*if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
            Debug.Log(tilemap.GetGrid().GetGridObject(mouseWorldPosition));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Ground;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Wall;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            tilemap.Save();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            tilemap.Load();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            healthSystem.Damage(10);
        }*/
    }

    public void instantiateSprite()
    {
        Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
        tilemap.SetTilemapSprite(mouseWorldPosition, tilemapSprite);
        Instantiate(enviromentPrefabs[(int)tilemapSprite-1], tilemap.GetGrid().GetWorldPosition(mouseWorldPosition) + new Vector3(1f, 1f) * .5f, Quaternion.identity, this.transform);

    }
}
