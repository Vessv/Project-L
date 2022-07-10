using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class UnitO : NetworkBehaviour
{


    public NetworkVariable<bool> isMyTurn = new NetworkVariable<bool>();
    public NetworkVariable<State> state = new NetworkVariable<State>();
    public NetworkVariable<Vector3> playerPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> targetPositionRpc = new NetworkVariable<Vector3>();
    public string[] unitActions;
    public NetworkVariable<FixedString64Bytes> selectedAction = new NetworkVariable<FixedString64Bytes>();


    public HealthSystem healthSystem;



    private void Awake()
    {
        //Application.targetFrameRate = 60;
        healthSystem = new HealthSystem(1 * 10);

    }


    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer) SetStateServerRpc(State.Normal);
        if (IsLocalPlayer) SumbitPositionServerRpc(new Vector3(0.5f, 0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        ClientInput();
    }

    void ClientInput()
    {
        if (!isMyTurn.Value || !IsLocalPlayer) return;


        //if (state.Value == State.Moving) Move();

        if (Input.GetMouseButtonDown(0) && state.Value == State.Normal && selectedAction.Value == "Move")
        {
            SetStateServerRpc(State.Moving);
            SumbitTargetPositionServerRpc(Utils.GetMouseWorldPosition());

        } else if(Input.GetMouseButtonDown(0) && state.Value == State.Normal && selectedAction.Value == "Shoot")
        {
            SetStateServerRpc(State.Attacking);
            SumbitTargetPositionServerRpc(Utils.GetMouseWorldPosition());
        }
    }

    public enum State
    {
        Normal,
        Moving,
        Attacking
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }

    [ServerRpc]
    public void SetSelectedActionServerRpc(string action)
    {
        selectedAction.Value = action;
    }

    [ServerRpc]
    void SumbitTargetPositionServerRpc(Vector3 targetPos)
    {
        targetPositionRpc.Value = targetPos;
    }

    [ServerRpc]
    void SetStateServerRpc(State statev)
    {
        state.Value = statev;
    }
    [ServerRpc]
    void SumbitPositionServerRpc(Vector3 position)
    {
        playerPosition.Value = position;
    }
    [ServerRpc]
    void EndTurnServerRpc()
    {
        isMyTurn.Value = false;
    }
}
