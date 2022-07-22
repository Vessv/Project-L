using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Shoot;

    [SerializeField]
    private int _damage;

    public void Attack(BaseUnit enemy)
    {
        _damage = unit.Stats.Strength;
        enemy.TakeDamage(_damage);
        Debug.Log("Damaged:" + enemy.name);

        unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
        unit.IsMyTurn.Value = false;

    }
}
