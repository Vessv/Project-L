using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class TurnHandler : NetworkBehaviour
{
    [SerializeField]
    bool hasPlayersCycleBeenDone;

    [SerializeField]
    public BaseUnit CurrentUnit;

    [SerializeField]
    public int CurrentTurnIndex;

    private void Awake()
    {
        if (!IsServer) return;
        hasPlayersCycleBeenDone = false;
        CurrentTurnIndex = 0;
        HandleDisconnected();
    }

    private void Start()
    {
        if (!IsServer) return;
        CurrentUnit = NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.GetComponent<PlayerUnit>();
        CurrentUnit.IsMyTurn.Value = true;
    }

    void HandleDisconnected()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (!IsServer) return;
            CurrentTurnIndex--;
            if (CurrentTurnIndex < 0) CurrentTurnIndex = 0;
        };
    }

    public void NextTurn()
    {
        if (hasPlayersCycleBeenDone)
        {
            //Enemy turn
            //Gets the enemies from a enemy holder does one turn for each enemy ends enemycycle
            //On cycle done, player cycle goes
            //add speed stat? would need to put all units on an array and do turns order depending on the speed stat
            //add actions points and actions consumes certain amounts of them, turns ends when u run out of it or can't do anything more.

            CurrentTurnIndex++;
            if (CurrentTurnIndex >= GameHandler.Instance.EnemyList.Count)
            {
                hasPlayersCycleBeenDone = false;
                Debug.Log("Players Turn");
                CurrentTurnIndex = 0;
                CurrentUnit = NetworkManager.Singleton.ConnectedClientsList[CurrentTurnIndex].PlayerObject.GetComponent<PlayerUnit>();
                CurrentUnit.IsMyTurn.Value = true;
                Debug.Log("current turn: 0");
                return;
            }

            Debug.Log("current turn: " + CurrentTurnIndex + " maxTurn: " + GameHandler.Instance.EnemyList.Count);
            CurrentUnit = GameHandler.Instance.EnemyList[CurrentTurnIndex];
            CurrentUnit.IsMyTurn.Value = true;
        }
        else
        {
            ulong[] turnIds = NetworkManager.Singleton.ConnectedClients.Keys.ToArray();
            CurrentTurnIndex++;
            if(CurrentTurnIndex >= turnIds.Length)
            {
                hasPlayersCycleBeenDone = true;
                Debug.Log("Enemy Turn");
                CurrentTurnIndex = 0;
                CurrentUnit = GameHandler.Instance.EnemyList[CurrentTurnIndex];
                CurrentUnit.IsMyTurn.Value = true;
                return;
            }
            Debug.Log("current turn: " + CurrentTurnIndex + " maxTurn: " + turnIds.Length);
            CurrentUnit = NetworkManager.Singleton.ConnectedClientsList[CurrentTurnIndex].PlayerObject.GetComponent<PlayerUnit>();
            CurrentUnit.IsMyTurn.Value = true;
        }

    }

    public void OnTurnEnd(bool previous, bool current)
    {
        if (!current)
        {
            CurrentUnit.SelectedAction.Value = UnitAction.Action.None;
            NextTurn();
        }
    }
}
