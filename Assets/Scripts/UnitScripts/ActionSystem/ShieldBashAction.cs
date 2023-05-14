using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBashAction : BaseAction
{
    [SerializeField]
    private int _damage;
    List<Vector3> _pathVectorList = new List<Vector3>();
    BaseUnit targetUnit;
    public void Attack()
    {
        if (!CanDoAction)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;
            Debug.Log("Not Enough actions points");
            return;
        }
        _pathVectorList = new List<Vector3>();
        _pathVectorList.Clear();
        _pathVectorList = Pathfinding.Instance.FindPathToNotWalkable(unit.transform.position, unit.TargetPosition.Value);
        _pathVectorList.RemoveAt(_pathVectorList.Count - 1);
        if (_pathVectorList == null)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;
            Debug.Log("obstacles in the way an path is not reachable");
            return;
        }
        _pathVectorList = Pathfinding.Instance.FindPath(unit.transform.position, _pathVectorList[_pathVectorList.Count - 1]);
        if (_pathVectorList == null)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;
            Debug.Log("no walkable near in the way an path is not reachable");
            return;
        }

        bool withinAttackRange = _pathVectorList.Count > 1 && _pathVectorList.Count <= (1 + 2 + (int)Mathf.Floor(unit.Stats.Value.Dexterity / 2));

        targetUnit = GameHandler.Instance.GetGrid().GetGridObject(unit.TargetPosition.Value).GetUnit();

        bool isAValidTarget = targetUnit != null && targetUnit != unit;
        if (!withinAttackRange)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;
            Debug.Log("Target is not withinAttackRange, distance: " + _pathVectorList.Count);
            return;
        }
        if (!isAValidTarget)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;
            Debug.Log("ShieldBash.cs error at Attack() no valid target at x,y =" + unit.TargetPosition.Value.x + "," + unit.TargetPosition.Value.y);
            return;
        }
        GameHandler.Instance.GetGrid().GetGridObject(unit.transform.position).RemoveUnit();
        StartCoroutine(ShieldBash(_pathVectorList));

        
        //unit.IsMyTurn.Value = false;

    }

    IEnumerator ShieldBash(List<Vector3> vectorList)
    {
        if (vectorList.Count > 0)
        {
            unit.transform.position = vectorList[0];
            vectorList.RemoveAt(0);
            yield return new WaitForSeconds(0.05f);
            StartCoroutine(ShieldBash(_pathVectorList));

            yield break;
        }
        GameHandler.Instance.GetGrid().GetGridObject(unit.transform.position).SetUnit(unit);
        _damage = (int)Mathf.Floor(unit.Stats.Value.Strength * 1.5f);
        PlaySound("shield");
        yield return new WaitForSeconds(0.05f);
        targetUnit.TakeDamageClientRpc(_damage);
        Debug.Log("Damaged: " + targetUnit.name);
        unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
        unit.SelectedAction.Value = UnitAction.Action.None;
        UseActionPoints();
        yield break;
    }

    public void ShowMoveTiles()
    {
        int distance = 2 + (int)Mathf.Floor(unit.Stats.Value.Dexterity / 2);
        Vector3 position = unit.transform.position - new Vector3(0.5f, 0.5f);

        for (int i = -distance; i <= distance; i++)
        {
            for (int j = -distance; j <= distance; j++)
            {
                // Determinar si la posición es parte del diamante
                if (Mathf.Abs(i) + Mathf.Abs(j) <= distance)
                {
                    PlayerUnit currentPlayer = (PlayerUnit)unit;
                    int x = (int)position.x + i;
                    int y = (int)position.y + j;
                    if (x < 0 || y < 0 || x >= 18 || y >= 16)
                    {
                        continue;
                    }
                    List<Vector3> vectorList = Pathfinding.Instance.FindPathToNotWalkable(position, new Vector3(x, y));

                    if (vectorList != null && vectorList.Count > 1 && vectorList.Count - 1 <= distance)
                    {
                        currentPlayer.SetMapVisualTileActiveClientRpc(x, y, new Color(0.5f, 0.25f, 0, 0.5f));
                    }
                }
            }
        }
    }
}
