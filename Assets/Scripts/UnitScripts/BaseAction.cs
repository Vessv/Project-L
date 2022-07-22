using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;


public abstract class BaseAction :  NetworkBehaviour, IUnitAction
{
    public abstract ActionType GetActionType();


    public event EventHandler OnActionStarted;
    public event EventHandler OnActionComplete;


    [SerializeField]
    protected BaseUnit unit;
    protected bool isActive;
    protected Action onActionComplete;

    public BaseUnit GetUnit() => unit;

    public bool IsActive() => isActive;

    public void Setup(BaseUnit unit)
    {
        this.unit = unit;
    }
}
