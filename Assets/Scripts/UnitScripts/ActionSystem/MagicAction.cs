using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAction : BaseAction
{
    [SerializeField]
    private int _damage;
    List<Vector3> _pathVectorList = new List<Vector3>();
    public void Attack()
    {
        if (!CanDoAction)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;
            Debug.Log("Not Enough actions points");
            return;
        }
        _pathVectorList.Clear();
        _pathVectorList = Pathfinding.Instance.FindPathToNotWalkable(unit.transform.position, unit.TargetPosition.Value);
        if (_pathVectorList == null) return;

        bool withinAttackRange = _pathVectorList.Count > 1 && _pathVectorList.Count <= (1 + 2 + (int)Mathf.Floor(unit.Stats.Value.Dexterity / 2));

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
        unit.SpawnProjectileClientRpc(1);
        PlaySound("magic");


        _damage = (int)Mathf.Floor(unit.Stats.Value.Intelligence * 1.5f);
        targetUnit.TakeDamageClientRpc(_damage);
        Debug.Log("Damaged: " + targetUnit.name);

        StartCoroutine(EndAction());

    }

    IEnumerator EndAction()
    {
        yield return new WaitForSeconds(0.2f);
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
                        currentPlayer.SetMapVisualTileActiveClientRpc(x, y, new Color(0, 1, 0.5f, 0.5f));
                    }
                }
            }
        }
    }
}
