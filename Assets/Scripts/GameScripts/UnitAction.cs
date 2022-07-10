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
        if (!IsLocalPlayer) return;
        unit = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<UnitO>();
        actionsUI.SetActive(true);
        actionsUI.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(SetSelectedAction);
        actionsUI.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(SetSelectedAction2);
    }

    public void SetSelectedAction()
    {
        unit.SetSelectedActionServerRpc("Move");
    }
    public void SetSelectedAction2()
    {
        unit.SetSelectedActionServerRpc("Shoot");
    }
}
