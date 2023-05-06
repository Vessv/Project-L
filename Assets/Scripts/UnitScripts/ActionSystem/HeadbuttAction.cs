using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadbuttAction : BaseAction
{
    [SerializeField]
    private int _damage;
    List<Vector3> _pathVectorList = new List<Vector3>();

    public void Attack()
    {
        _pathVectorList = new List<Vector3>();
        _pathVectorList.Clear();
        _pathVectorList = Pathfinding.Instance.FindPathToNotWalkable(unit.transform.position, unit.TargetPosition.Value);
        if (_pathVectorList == null)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;
            Debug.Log("obstacles in the way an path is not reachable");
            return;
        }

        bool withinAttackRange = _pathVectorList.Count > 1 && _pathVectorList.Count <= (1 + 1);

        BaseUnit targetUnit = GameHandler.Instance.GetGrid().GetGridObject(unit.TargetPosition.Value).GetUnit();

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
            Debug.Log("AttackAction.cs error at Attack() no valid target at x,y =" + unit.TargetPosition.Value.x + "," + unit.TargetPosition.Value.y);
            return;
        }

        _damage = (int)Mathf.Floor(unit.Stats.Value.Strength * 1.5f);
        //AudioManager.Instance.Play("Hit");
        targetUnit.TakeDamageClientRpc(_damage);
        unit.TakeDamageClientRpc((int)Mathf.Floor(unit.Stats.Value.Vitality * 0.1f));
        Debug.Log("Damaged: " + targetUnit.name);
        unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
        unit.SelectedAction.Value = UnitAction.Action.None;
        UseActionPoints();
        //unit.IsMyTurn.Value = false;

    }

    public void ShowMoveTiles()
    {
        int distance = 1;
        Vector3 position = unit.transform.position - new Vector3(0.5f, 0.5f);

        for (int i = -distance; i <= distance; i++)
        {
            for (int j = -distance; j <= distance; j++)
            {
                // Determinar si la posici�n es parte del diamante
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
                        currentPlayer.SetMapVisualTileActiveClientRpc(x, y, new Color(0.7f, 0.6f, 0, 0.5f));
                    }
                }
            }
        }
    }
}
