using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NPCUnit : BaseUnit
{
    // Start is called before the first frame update
    void Start()
    {
        //Test starting positions remember to remove
        if (UnitScriptableObject.UnitFaction == UnitSO.Faction.Demon && IsOwnedByServer)
        {
            Debug.Log("Spawneado enemigo");
            SubmitPositionServerRpc(new Vector3(2.5f, 2.5f));
            transform.position = new Vector3(2.5f, 2.5f);
            GameHandler.Instance.GetGrid().GetGridObject(new Vector3(2.5f, 2.5f)).SetUnit(this);
            IsMyTurn.OnValueChanged += GameHandler.Instance.TurnHandler.OnTurnEnd;
            TargetPosition.OnValueChanged += DoAction;

            return;
        }
    }

    private void Update()
    {
        if (!IsMyTurn.Value || !IsServer) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            SubmitUnitActionServerRpc(UnitAction.Action.Move);
            TargetPosition.Value = NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.transform.position + new Vector3(1f, 1f);
            Debug.Log("NPC action");
        }
    }

    void DoAction(Vector3 previous, Vector3 current)
    {
        Vector3 targert = (current + new Vector3(1f, 1f));
        GameHandler.Instance.DoAction(previous, targert);
    }

}
