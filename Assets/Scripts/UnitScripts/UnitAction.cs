using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UnitAction : NetworkBehaviour
{
    public GameObject ActionsUI;
    PlayerUnit _unit;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer) return;
        _unit = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>();
        ActionsUI.SetActive(true);
        ActionsUI.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(SetSelectedAction);
        ActionsUI.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(SetSelectedAction2);
    }

    public void SetSelectedAction()
    {
        _unit.SubmitUnitActionServerRpc(Action.Move);
    }
    public void SetSelectedAction2()
    {
        _unit.SubmitUnitActionServerRpc(Action.Shoot);
    }

    public enum Action
    {
        None,
        Move,
        Shoot
    }
}
