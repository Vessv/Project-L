using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class GameHandler_GridCombatSystem : NetworkBehaviour
{
    public static GameHandler_GridCombatSystem instance { get; private set; }

    private Grid<GridObject> grid;
    private Pathfinding pathfindingGrid;
    private Tilemap tilemapGrid;

    public Transform[] enviromentPrefabs;
    public GameObject enviromentPrefabHolder;

    private void Awake()
    {
        instance = this;
        enviromentPrefabs = enviromentPrefabHolder.GetComponent<EnviromentHolder>().enviromentPrefabs;
    }

    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid<GridObject>(18, 10, 1f, new Vector3(0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(grid, x, y));
        tilemapGrid = new Tilemap(18, 10, 1f, new Vector3(0, 0));
        pathfindingGrid = new Pathfinding(18, 10, 1f, new Vector3(0, 0));
        CreateWorld();
        InstantiateWorld();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateWorld()
    {
        for (int x = 0; x < tilemapGrid.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < tilemapGrid.GetGrid().GetHeight(); y++)
            {
                tilemapGrid.spriteRaycast(x, y);
                //tilemap.SetTilemapSprite(new Vector3(x, y), Tilemap.TilemapObject.TilemapSprite.Ground);
            }
        }
    }

    public void InstantiateWorld()
    {
        for (int x = 0; x < tilemapGrid.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < tilemapGrid.GetGrid().GetHeight(); y++)
            {
                if ((int)tilemapGrid.GetGrid().GetGridObject(x, y).GetTilemapSprite() != 0)
                    Instantiate(enviromentPrefabs[(int)tilemapGrid.GetGrid().GetGridObject(x, y).GetTilemapSprite() - 1], tilemapGrid.GetGrid().GetWorldPosition(x, y) + new Vector3(1f, 1f) * .5f, Quaternion.identity, this.transform);
                if((int)tilemapGrid.GetGrid().GetGridObject(x, y).GetTilemapSprite() != 1)
                {
                    pathfindingGrid.GetGrid().GetGridObject(x,y).SetWalkable(false);
                }
            }
        }
    }

    public Grid<GridObject> GetGrid()
    {
        return grid;
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        UnitCombatGrid unit;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetUnit(UnitCombatGrid unit)
        {
            this.unit = unit;
        }

        public UnitCombatGrid GetUnit()
        {
            return this.unit;
        }

        public void RemoveUnit()
        {
            this.unit = null;
        }

        public override string ToString()
        {
            return x+":"+y;
        }

    }
}
