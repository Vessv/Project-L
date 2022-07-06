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



    protected UnitO unit;
    protected bool isActive;
    protected Action onActionComplete;

    public UnitO GetUnit() => unit;

    public bool IsActive() => isActive;

    public void Setup(UnitO unit)
    {
        this.unit = unit;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
