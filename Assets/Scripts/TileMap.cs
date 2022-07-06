using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilemap
{
    private Grid<TilemapObject> grid;

    public Tilemap(int widht, int height, float cellSize, Vector3 originPosition)
    {
        grid = new Grid<TilemapObject>(widht, height, cellSize, originPosition, (Grid<TilemapObject> g, int x, int y) => new TilemapObject(grid,x,y));
    }
    public void SetTilemapSprite(Vector3 worldPosition, TilemapObject.TilemapSprite tilemapSprite)
    {
        TilemapObject tilemapObject = grid.GetGridObject(worldPosition);
        if (tilemapObject != null)
        {
            tilemapObject.SetTilemapSprite(tilemapSprite);
        }
    }

    public void spriteRaycast(int x, int y)
    {
        RaycastHit2D ray = Physics2D.Raycast(new Vector3(x, y, -1) + new Vector3(1, 1) * .5f, Vector3.forward * 2);
        
        if (ray.collider != null)
        {
            if(ray.collider.tag == "Ground")
            {
                SetTilemapSprite(new Vector3(x, y) + new Vector3(1, 1) * .5f, TilemapObject.TilemapSprite.Ground);
            }
        }
    }

    public void Save()
    {
        List<TilemapObject.SaveObject> tilemapObjectSaveObjectList = new List<TilemapObject.SaveObject>();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TilemapObject tilemapObject = grid.GetGridObject(x, y);
                tilemapObjectSaveObjectList.Add( tilemapObject.Save());
            }
        }
         SaveObject saveObject = new SaveObject { tileObjectSaveObjectArray = tilemapObjectSaveObjectList.ToArray() };
        SaveSystem.SaveObject(saveObject);
    }

    public void Load()
    {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
        foreach (TilemapObject.SaveObject tilemapObjectSaveObject in saveObject.tileObjectSaveObjectArray)
        {
            TilemapObject tilemapObject = grid.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
        }
    }

    public class SaveObject
    {
        public TilemapObject.SaveObject[] tileObjectSaveObjectArray;
    }

    public Grid<TilemapObject> GetGrid()
    {
        return grid;
    }

    public class TilemapObject
    {

        public enum TilemapSprite
        {
            None,
            Ground,
            Wall,
        }
        private Grid<TilemapObject> grid;
        private int x;
        private int y;
        private TilemapSprite tilemapSprite;

        public TilemapObject(Grid<TilemapObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetTilemapSprite(TilemapSprite tilemapSprite)
        {
            this.tilemapSprite = tilemapSprite;
            //grid.triggerobject hacer?
        }

        public override string ToString()
        {
            return tilemapSprite.ToString();
        }

        public TilemapSprite GetTilemapSprite()
        {
            return tilemapSprite;
        }

        [System.Serializable]
        public class SaveObject
        {
            public TilemapSprite tilemapSprite;
            public int x;
            public int y;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                tilemapSprite = this.tilemapSprite,
                x = this.x, y = this.y 
            };
        }

        public void Load(SaveObject saveObject)
        {
            tilemapSprite = saveObject.tilemapSprite;
        }
    }

}
