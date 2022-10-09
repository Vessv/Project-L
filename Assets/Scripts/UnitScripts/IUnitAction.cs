using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitAction
{

    ActionType GetActionType();
    void Setup(BaseUnit unit);
    BaseUnit GetUnit();
    bool UseActionPoints();
}

public enum ActionType
{
    Move,
    Spin,
    Shoot,
    Grenade,
    Overwatch,
}
