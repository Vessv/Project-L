using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Shoot;

    [SerializeField]
    private int _damage;
    List<Vector3> _pathVectorList = new List<Vector3>();

    TurnHandler _turnHandler;

    private void Start()
    {
        _turnHandler = GetComponent<TurnHandler>();

    }

    public void Attack()
    {
        _pathVectorList.Clear();
        _pathVectorList = Pathfinding.Instance.FindPathToNotWalkable(unit.transform.position, unit.TargetPosition.Value);
        if(_pathVectorList == null) return;

        bool withinAttackRange = _pathVectorList.Count > 1 && _pathVectorList.Count <= (unit.Stats.Dexterity + 2);

        BaseUnit targetUnit = GameHandler.Instance.GetGrid().GetGridObject(unit.TargetPosition.Value).GetUnit();

        bool isAValidTarget = targetUnit != null && targetUnit != _turnHandler.CurrentUnit;
        if (!withinAttackRange)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            Debug.Log("Target is not withinAttackRange, distance: " + _pathVectorList.Count);
            return;
        }
        if (!isAValidTarget)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            Debug.Log("AttackAction.cs error at Attack() no valid target at x,y =" + targetUnit.transform.position.x + "," + targetUnit.transform.position.y);
            return;
        }

        _damage = unit.Stats.Strength;
        //AudioManager.Instance.Play("Hit");
        targetUnit.TakeDamage(_damage);
        Debug.Log("Damaged: " + targetUnit.name);
        unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
        unit.SelectedAction.Value = UnitAction.Action.None;
        UseActionPoints();
        //unit.IsMyTurn.Value = false;

    }
}
