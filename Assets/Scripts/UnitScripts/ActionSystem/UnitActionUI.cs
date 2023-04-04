using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class UnitActionUI : MonoBehaviour
{
    PlayerUnit _unit;
    [SerializeField]
    int actionIndex;
    public void UseAction(PlayerUnit unit)
    {
        _unit = unit;
        if (!_unit.IsBusy && _unit.IsMyTurn.Value) _unit.SubmitUnitActionServerRpc((UnitAction.Action)actionIndex);

    }
    public void SetActionIndex(int index)
    {
        if (index < 0) return;
        actionIndex = index;
    }
}
