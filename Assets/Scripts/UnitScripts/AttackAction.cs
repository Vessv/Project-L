using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Shoot;

    [SerializeField]
    private int _damage;

    TurnHandler _turnHandler;

    private void Start()
    {
        _turnHandler = GetComponent<TurnHandler>();

    }

    public void Attack()
    {
        BaseUnit targetUnit = GameHandler.Instance.GetGrid().GetGridObject(unit.TargetPosition.Value).GetUnit();

        bool isAValidTarget = targetUnit != null && targetUnit != _turnHandler.CurrentUnit;
        if (!isAValidTarget)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            Debug.Log("AttackAction.cs error at Attack() no valid target at x,y =" + targetUnit.transform.position.x + "," + targetUnit.transform.position.y);
            return;
        }

        _damage = unit.Stats.Strength;
        //AudioManager.Instance.Play("Hit");
        targetUnit.TakeDamage(_damage);
        Debug.Log("Damaged:" + targetUnit.name + "for: " + _damage);

        unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
        UseActionPoints();
        //unit.IsMyTurn.Value = false;

    }
}
