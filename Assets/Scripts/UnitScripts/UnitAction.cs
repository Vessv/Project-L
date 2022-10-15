using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class UnitAction : NetworkBehaviour
{
    public GameObject ActionsUI;
    PlayerUnit _unit;
    // Start is called before the first frame update
    //TODO: hacerlo más readeable, probablemente cambiar todo este sistema
    void Start()
    {
        if (!IsLocalPlayer) return;
        _unit = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>();
        ActionsUI.SetActive(true);
        for(int i = 0; i < ActionsUI.transform.childCount; i++)
        {
            int capturedIndex = i;
            ActionsUI.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedAction(capturedIndex + 1); });
        }
    }

    public void SetSelectedAction(int index)
    {
        if(!_unit.IsBusy) _unit.SubmitUnitActionServerRpc((Action)index);
    }

    public enum Action
    {
        None,
        Move,
        Shoot
    }
}
