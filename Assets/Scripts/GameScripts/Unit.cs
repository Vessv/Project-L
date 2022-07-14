using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.InputSystem;

public class Unit : NetworkBehaviour
{

    //NetworkVariables
    public NetworkVariable<bool> IsMyTurn = new NetworkVariable<bool>();
    public NetworkVariable<ActionState> ActionStatus = new NetworkVariable<ActionState>();
    public NetworkVariable<Vector3> PlayerPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> TargetPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<UnitAction.Action> SelectedAction = new NetworkVariable<UnitAction.Action>();



    public HealthSystem healthSystem;



    private void Awake()
    {
        //Application.targetFrameRate = 60;
        healthSystem = new HealthSystem(1 * 10);

    }


    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer) SubmitActionStateServerRpc(ActionState.Normal);
        if (IsLocalPlayer) SubmitPositionServerRpc(new Vector3(0.5f, 0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && IsLocalPlayer)
        {
            //Click2();
        }
    }


    bool CanInteract => IsMyTurn.Value && IsLocalPlayer;
    public bool IsBusy => ActionStatus.Value == ActionState.Busy;
    public bool CanPlay => !IsBusy && SelectedAction.Value != UnitAction.Action.None;

    public bool CanMove => SelectedAction.Value == UnitAction.Action.Move;
    public bool CanShoot => SelectedAction.Value == UnitAction.Action.Shoot;


    public void Click(InputAction.CallbackContext context)
    {
        if (!CanInteract || !context.performed) return;

        if (CanPlay)
        {
            SubmitActionStateServerRpc(ActionState.Busy);
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
        return healthSystem;
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
