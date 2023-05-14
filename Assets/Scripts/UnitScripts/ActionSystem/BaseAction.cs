using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;


public abstract class BaseAction :  NetworkBehaviour, IUnitAction
{

    [SerializeField]
    protected BaseUnit unit;
    protected Action onActionComplete;

    [SerializeField]
    int actionPointsCost;
    public bool CanDoAction => unit.ActionPoints.Value >= actionPointsCost;

    public BaseUnit GetUnit() => unit;

    public void Setup(BaseUnit unit)
    {
        this.unit = unit;
    }

    public bool UseActionPoints()
    {
        if(CanDoAction)
        {
            unit.ActionPoints.Value -= actionPointsCost;
            return true;
        }
        return false;
    }

    public void PlaySound(string name, bool instant = false)
    {
        GameHandler.Instance.PlaySoundToAllPlayers(name, instant);
    }
    public void StopSound(string name, bool instant = false)
    {
        GameHandler.Instance.StopSoundToAllPlayers(name, instant);
    }
}
