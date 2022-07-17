using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Shoot;

    [SerializeField]
    private int _damage;

    public void Attack(Unit enemy)
    {
        _damage = unit.GetDamage();
        enemy.GetHealthSystem().Damage(_damage);
        Debug.Log("Damaged:" + enemy.name);
        if (enemy.GetHealthSystem().IsDead())
        {
            enemy.gameObject.SetActive(false);
        }
        unit.ActionStatus.Value = Unit.ActionState.Normal;
        unit.IsMyTurn.Value = false;

    }
}
