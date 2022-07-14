using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UnitAction : NetworkBehaviour
{
    public GameObject actionsUI;
    Unit unit;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer) return;
        unit = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Unit>();
        actionsUI.SetActive(true);
        actionsUI.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(SetSelectedAction);
        actionsUI.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(SetSelectedAction2);
    }

    public void SetSelectedAction()
    {
        unit.SubmitUnitActionServerRpc(Action.Move);
    }
    public void SetSelectedAction2()
    {
        unit.SubmitUnitActionServerRpc(Action.Shoot);
    }

    public enum Action
    {
        None,
        Move,
        Shoot
    }
}
