using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.InputSystem;

public class Unit : NetworkBehaviour
{

    public NetworkVariable<bool> IsMyTurn = new NetworkVariable<bool>();
    public NetworkVariable<ActionState> ActionStatus = new NetworkVariable<ActionState>();
    public NetworkVariable<Vector3> PlayerPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> TargetPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<UnitAction.Action> SelectedAction = new NetworkVariable<UnitAction.Action>();

    private HealthSystem _healthSystem;
    bool CanInteract => IsMyTurn.Value && IsLocalPlayer;
    public bool IsBusy => ActionStatus.Value == ActionState.Busy;
    public bool CanPlay => !IsBusy && SelectedAction.Value != UnitAction.Action.None;

    public bool CanMove => SelectedAction.Value == UnitAction.Action.Move;
    public bool CanShoot => SelectedAction.Value == UnitAction.Action.Shoot;


    void Awake()
    {
        //Application.targetFrameRate = 60;
        _healthSystem = new HealthSystem(1 * 10);

    }


    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer) return;
        SubmitActionStateServerRpc(ActionState.Normal);
        SubmitPositionServerRpc(new Vector3(0.5f, 0.5f));
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void Click(InputAction.CallbackContext context)
    {
        if (!CanInteract || !context.performed) return;

        if (CanPlay)
        {
            SubmitTargetPositionServerRpc(Utils.GetMouseWorldPosition());
        }
    }

    public enum ActionState
    {
        Normal,
        Busy,
    }

    public HealthSystem GetHealthSystem()
    {
        return _healthSystem;
    }

    [ServerRpc]
    public void SubmitUnitActionServerRpc(UnitAction.Action action)
    {
        SelectedAction.Value = action;
    }

    [ServerRpc]
    void SubmitTargetPositionServerRpc(Vector3 targetPosition)
    {
        TargetPosition.Value = targetPosition;
    }

    [ServerRpc]
    void SubmitActionStateServerRpc(ActionState state)
    {
        ActionStatus.Value = state;
    }
    [ServerRpc]
    void SubmitPositionServerRpc(Vector3 position)
    {
        PlayerPosition.Value = position;
    }
}
