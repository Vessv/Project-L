using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyTreeAction : BaseAction
{

    List<Vector3> _pathVectorList = new List<Vector3>();

    [SerializeField]
    UnitSO.UnitStats _buffedStats;

    public void Heal()
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
        if (_pathVectorList == null)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;
            Debug.Log("obstacles in the way an path is not reachable");
            return;
        }

        bool withinAttackRange = _pathVectorList.Count > 1 && _pathVectorList.Count <= (1 + 2 + (int)Mathf.Floor(unit.Stats.Value.Dexterity / 2));


        if (!withinAttackRange)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;
            Debug.Log("Target is not withinAttackRange, distance: " + _pathVectorList.Count);
            return;
        }

        unit.SpawnObjectClientRpc(unit.TargetPosition.Value, 5);
        PlaySound("tree_leaves");
        PlaySound("tree_birds");

        StartCoroutine(DoHeal());


    }

    IEnumerator DoHeal()
    {
        yield return new WaitForSeconds(1.3f);
        HitUnits();

        yield return new WaitForSeconds(2f);
        StopSound("tree_leaves");
        StopSound("tree_birds");

        unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
        unit.SelectedAction.Value = UnitAction.Action.None;
        UseActionPoints();
    }

    void HitUnits()
    {
        int distance = 3;
        int healAmount = (int)Mathf.Floor(unit.Stats.Value.Vitality);
        for (int i = -distance; i <= distance; i++)
        {
            for (int j = -distance; j <= distance; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) <= distance)
                {
                    Vector3 targetAnOffset = unit.TargetPosition.Value + new Vector3(i, j);
                    bool isOutOfBounds = targetAnOffset.x < 0 || targetAnOffset.y < 0 || targetAnOffset.x >= 18 || targetAnOffset.y >= 16;

                    if (isOutOfBounds) continue;
                    BaseUnit targetUnit = GameHandler.Instance.GetGrid().GetGridObject(unit.TargetPosition.Value + new Vector3(i, j)).GetUnit();

                    bool isAValidTarget = targetUnit != null && targetUnit.UnitScriptableObject.UnitFaction == UnitSO.Faction.Hero;
                    if (isAValidTarget)
                    {
                        targetUnit.CurrentHealth.Value += healAmount;
                        targetUnit.Stats.Value += _buffedStats;
                        Debug.Log("Healed: " + targetUnit.name);
                    }
                }
            }
        }
    }

    public void ShowMoveTiles()
    {
        int distance = 2 + (int)Mathf.Floor(unit.Stats.Value.Dexterity / 2);
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
                        currentPlayer.SetMapVisualTileActiveClientRpc(x, y, new Color(0.1f, 0.8f, 0.1f, 0.5f));
                    }
                }
            }
        }
    }
}
