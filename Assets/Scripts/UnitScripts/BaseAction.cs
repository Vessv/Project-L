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
    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    public Unit GetUnit() => unit;

    public bool IsActive() => isActive;

    public void Setup(Unit unit)
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
