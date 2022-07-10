using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Move;

    public int Damage;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(UnitO enemy)
    {
        //GameHandler.instance.GetGrid().GetGridObject(unit.targetPositionRpc.Value).GetUnit();
        enemy.GetHealthSystem().Damage(1);
        Debug.Log("Damaged:" + enemy.name);
        unit.state.Value = UnitO.State.Normal;
        unit.isMyTurn.Value = false;

    }
}
