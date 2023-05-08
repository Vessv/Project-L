using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class TurnHandler : NetworkBehaviour
{
    [SerializeField]
    public bool hasPlayersCycleBeenDone;

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
            //Player turn
            ulong[] turnIds = NetworkManager.Singleton.ConnectedClients.Keys.ToArray();
            CurrentTurnIndex++;
            if(CurrentTurnIndex >= turnIds.Length && GameHandler.Instance.EnemyList.Count != 0)
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

    public void OnUnitActionPointsChanged(int previous, int currentValue)
    {
        if (currentValue == 0) CurrentUnit.IsMyTurn.Value = false;
    }

    public void OnIsMyTurnValueChanged(bool previous, bool currentValue)
    {
        if (currentValue)
        {
            OnTurnStart();
            return;
        }
        OnTurnEnd();
    }

    void OnTurnStart()
    {
        CurrentUnit.Threat.Value = 0;
        if (CurrentUnit.ActionPoints.Value < 0)
        {
            CurrentUnit.ActionPoints.Value = 0;
            return;
        }
        if(CurrentUnit.ActionPoints.Value + CurrentUnit.Stats.Value.Stamina > 8)
        {
            CurrentUnit.ActionPoints.Value = 8;
            return;
        }
        CurrentUnit.ActionPoints.Value += CurrentUnit.Stats.Value.Stamina;
    }

    void OnTurnEnd()
    {
        CurrentUnit.SelectedAction.Value = UnitAction.Action.None;
        NextTurn();
    }
}
