using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public bool isObstacle;
    public PathNode cameFromNode;

    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
        isObstacle = false;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    public void SetObstacle(bool isObstacle)
    {
        this.isObstacle = isObstacle;
    }

    public override string ToString()
    {
        return x+":"+y;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(x, y);
    }
}
