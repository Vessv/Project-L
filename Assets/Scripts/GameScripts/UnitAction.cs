using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UnitAction : NetworkBehaviour
{
    public GameObject actionsUI;
    UnitO unit;
    // Start is called before the first frame update
    void Start()
    {
        actionsUI = GameObject.FindGameObjectWithTag("ActionsUI");
        unit = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<UnitO>();
        actionsUI.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(SetSelectedAction);
    }

    public void SetSelectedAction()
    {
        unit.SetSelectedActionServerRpc("Move");
    }
}
