using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipAction : BaseAction
{
    public void Skip()
    {
        PlaySound("hit");
        Debug.Log("skiped: " + unit.name);


        unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
        unit.SelectedAction.Value = UnitAction.Action.None;
        unit.IsMyTurn.Value = false;

    }
}
