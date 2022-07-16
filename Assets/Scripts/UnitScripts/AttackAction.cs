using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Shoot;

    public int Damage;

    public void Attack(Unit enemy)
    {
        enemy.GetHealthSystem().Damage(1);
        Debug.Log("Damaged:" + enemy.name);
        unit.ActionStatus.Value = Unit.ActionState.Normal;
        unit.IsMyTurn.Value = false;

    }
}
