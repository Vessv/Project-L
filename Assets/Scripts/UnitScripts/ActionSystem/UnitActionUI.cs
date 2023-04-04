using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UnitActionUI : MonoBehaviour, IDropHandler
{
    PlayerUnit _unit;
    [SerializeField]

    public ActionSO actionSO;
    public void UseAction(PlayerUnit unit)
    {
        _unit = unit;
        int actionIndex = 0;
        if (actionSO != null) actionIndex = actionSO.actionIndex;
        if (!_unit.IsBusy && _unit.IsMyTurn.Value) _unit.SubmitUnitActionServerRpc((UnitAction.Action)actionIndex);

    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropObject = eventData.pointerDrag;
        ActionSO dragActionSO = dropObject.GetComponent<DraggableImage>().ActionSO;
        actionSO = dragActionSO;
        UpdateActionUI();
    }

    public void UpdateActionUI()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = actionSO.actionName;
        transform.GetChild(1).GetComponent<Image>().sprite = actionSO.actionSprite;
    }
}
