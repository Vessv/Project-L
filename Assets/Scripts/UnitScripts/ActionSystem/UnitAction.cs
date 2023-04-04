using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Events;

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
        UpdateActionUI();


    }

    void UpdateActionUI()
    {
        for (int i = 0; i < ActionsUI.transform.childCount; i++)
        {
            int capturedIndex = i;
            UnityAction CallActionDelegate = () => { CallAction(capturedIndex); };
            GameObject ActionButtonGameObject = ActionsUI.transform.GetChild(i).gameObject;
            ActionButtonGameObject.GetComponent<Button>().onClick.RemoveListener(CallActionDelegate);
            ActionButtonGameObject.GetComponent<Button>().onClick.AddListener(CallActionDelegate);



            if (_unit.ownedActionArray[i] != 0)
            {
                UnitActionUI unitActionUI = ActionButtonGameObject.GetComponent<UnitActionUI>();
                ActionSO actionSO = GameHandler.Instance.GetActionsSOArray()[_unit.ownedActionArray[i] - 1];
                unitActionUI.actionSO = actionSO;
                unitActionUI.UpdateActionUI();
                //ActionButtonGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = actionSO.actionName;
                //ActionButtonGameObject.transform.GetChild(1).GetComponent<Image>().sprite = actionSO.actionSprite;

            }
        }
    }

    void CallAction(int capturedIndex)
    {
        ActionsUI.transform.GetChild(capturedIndex).gameObject.GetComponent<UnitActionUI>().UseAction(_unit);
        Debug.Log("callaction"); // quitar
    }

    public enum Action
    {
        None,
        Move,
        Shoot
    }
}
