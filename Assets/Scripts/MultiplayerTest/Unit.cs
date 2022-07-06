using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Unit : NetworkBehaviour
{
    public UnitSO unit;
    public HealthSystem healthSystem;
    public int vitality;

    private NetworkVariable<Vector3> playerPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<bool> isMyTurn = new NetworkVariable<bool>();


    private void Awake()
    {
        vitality = unit.vitality;
        healthSystem = new HealthSystem(vitality*10);
    }

    private void Update()
    {
        transform.position = playerPosition.Value;
        if (!isMyTurn.Value) return;
        if (Input.GetMouseButtonDown(0) && IsLocalPlayer)
        {
            MoveTo(Utils.GetMouseWorldPosition());
            EndTurnServerRpc();
        }

    }

    [ServerRpc]
    void EndTurnServerRpc()
    {
        isMyTurn.Value = false;
    }

    public void MoveTo(Vector3 position)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            transform.position = position;
            playerPosition.Value = position;

        } else
        {
            MoveToServerRpc(position);
        }
    }

    public void Damage(int value)
    {
        healthSystem.Damage(value);
    }

    [ServerRpc]
    void MoveToServerRpc(Vector3 position, ServerRpcParams rpcParams = default)
    {
        playerPosition.Value = position;
    }
}
