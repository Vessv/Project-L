using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using TMPro;

public class UnitAction : NetworkBehaviour
{
    public GameObject ActionsUI;
    PlayerUnit _unit;
    // Start is called before the first frame update
    //TODO: hacerlo más readeable, hacer un SO para cada acción con su imagen de ui, costo etc, meterlo en un holder y comparar el index de uiholder para saber cual SO coger.
    void Start()
    {
        if (!IsLocalPlayer) return;
        _unit = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>();
        ActionsUI.SetActive(true);
        for(int i = 0; i < ActionsUI.transform.childCount; i++)
        {
            int capturedIndex = i;
            GameObject ActionButtonGameObject = ActionsUI.transform.GetChild(i).gameObject;
            ActionButtonGameObject.GetComponent<Button>().onClick.AddListener(delegate { CallAction(capturedIndex); });

            if(_unit.ownedActionArray[i] != 0) 
            {
                ActionButtonGameObject.GetComponent<UnitActionUI>().SetActionIndex(_unit.ownedActionArray[i]);
                ActionButtonGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Action";

            }
        }
    }

    void CallAction(int capturedIndex)
    {
        ActionsUI.transform.GetChild(capturedIndex).gameObject.GetComponent<UnitActionUI>().UseAction(_unit);
    }

    public enum Action
    {
        None,
        Move,
        Shoot
    }
}
