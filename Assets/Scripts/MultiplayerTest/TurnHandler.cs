using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using System.Linq;



public class TurnHandler : NetworkBehaviour
{
    private NetworkVariable<ulong> currentTurn = new NetworkVariable<ulong>();
    int turnIndex;

    // Start is called before the first frame update
    void Start()
    {
        turnIndex = 0;
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (!IsServer) return;
            turnIndex--;
            if (turnIndex < 0) turnIndex = 0;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer || turnIndex < 0) return;
        Unit unit = NetworkManager.Singleton.ConnectedClientsList[turnIndex].PlayerObject.GetComponent<Unit>();
        if(!unit.isMyTurn.Value) unit.isMyTurn.OnValueChanged += OnTurnEnd;
        unit.isMyTurn.Value = true;
    }

    void OnTurnEnd(bool previous, bool current)
    {
        if (!current)
        {
            Unit unit = NetworkManager.Singleton.ConnectedClientsList[turnIndex].PlayerObject.GetComponent<Unit>();
            unit.isMyTurn.OnValueChanged -= OnTurnEnd;
            NextTurn();
        }
    }

    void NextTurn()
    {
        ulong[] turnIds = NetworkManager.Singleton.ConnectedClients.Keys.ToArray();
        turnIndex++;
        if(turnIndex >= turnIds.Length) turnIndex = 0;
        currentTurn.Value = turnIds[turnIndex];
        Debug.Log("current turn: " + turnIndex + " maxTurn: " + turnIds.Length);

    }
}
