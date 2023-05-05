using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{

    //Pathfinding vars
    List<Vector3> _pathVectorList = new List<Vector3>();
    public float Speed = 4f;
    int currentPathIndex;

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        if (unit == null) return;
        if (unit.ActionStatus.Value != BaseUnit.ActionState.Busy || unit.SelectedAction.Value != UnitAction.Action.Move) return;

        if (_pathVectorList != null && _pathVectorList.Count > 0)
        {
            Vector3 targetPosition = _pathVectorList[currentPathIndex];
            if (Vector3.Distance(unit.transform.position, targetPosition) > 0.05f)
            {
                Vector3 moveDir = (targetPosition - unit.transform.position).normalized;

                unit.transform.position = unit.transform.position + moveDir * Speed * Time.deltaTime;
                if (Vector3.Distance(unit.transform.position, targetPosition) < 0.05f)
                {
                    unit.transform.position = targetPosition;
                }
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= _pathVectorList.Count || currentPathIndex >= unit.Stats.Value.Speed)
                {
                    _pathVectorList.Clear();
                    GameHandler.Instance.GetGrid().GetGridObject(unit.transform.position).SetUnit(unit);
                    //unit.SetStateServerRpc(State.Normal);
                    unit.ActionStatus.Value = BaseUnit.ActionState.Normal;

                    //EndTurnServerRpc();

                    UseActionPoints();
                    //unit.IsMyTurn.Value = false;

                    //onPositionReached?.Invoke();
                    unit.SelectedAction.Value = UnitAction.Action.None;
                    unit.UpdateWalkVariableClientRpc();
                }
            }

        }
        else
        {
            //notReachable?.Invoke();
            Debug.Log("Not rechable");
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;

        }
    }

    public void Move()
    {
        Debug.Log("Used move action: " + unit.name);
        _pathVectorList = new List<Vector3>();
        currentPathIndex = 0;

        _pathVectorList.Clear();
        _pathVectorList = Pathfinding.Instance.FindPath(unit.transform.position, unit.TargetPosition.Value);


        if (_pathVectorList != null && _pathVectorList.Count > 1)
        {
            _pathVectorList.RemoveAt(0);
            GameHandler.Instance.GetGrid().GetGridObject(unit.transform.position).RemoveUnit();
            //OnPositionReachedServerRpc();
        }
        else
        {
            //notReachableServerRpc();
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;

            Debug.Log("path vector list is lower than 1 or target is occupied or path is longer than distance");
        }
    }

    public void ShowMoveTiles()
    {
        int distance = unit.Stats.Value.Speed;
        Vector3 position = unit.transform.position - new Vector3(0.5f, 0.5f);

        for (int i = -distance; i <= distance; i++)
        {
            for (int j = -distance; j <= distance; j++)
            {
                // Determinar si la posición es parte del diamante
                if (Mathf.Abs(i) + Mathf.Abs(j) <= distance)
                {
                    int x = (int)position.x + i;
                    int y = (int)position.y + j;
                    if (x < 0 || y < 0 || x >= 18 || y >= 16)
                    {
                        continue;
                    }
                    List<Vector3> vectorList = Pathfinding.Instance.FindPath(position, new Vector3(x,y));
                    if(vectorList != null && vectorList.Count > 1 && vectorList.Count-1 <= distance)
                    {
                        if (Pathfinding.Instance.GetGrid().GetGridObject(x, y).isWalkable && !Pathfinding.Instance.GetGrid().GetGridObject(x, y).isObstacle)
                        {
                            PlayerUnit currentPlayer = (PlayerUnit)unit;

                            currentPlayer.SetMapVisualTileActiveClientRpc(x, y, new Color(0, 1, 0, 0.5f));
                        }
                    }
                    
                }
            }
        }
    }
}
