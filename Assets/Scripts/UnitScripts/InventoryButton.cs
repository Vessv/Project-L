using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemInventorySlot slot;
    public TooltipPopUp tooltipPopUp;
    void Start()
    {
        slot = GetComponentInParent<ItemInventorySlot>();
    }
    public void InventoryOpen()
    {
        if (slot.item != null)
        {
            tooltipPopUp.HideInfo();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Debug.Log("Left click");
        else if (eventData.button == PointerEventData.InputButton.Middle)
        { slot.UseItem(); tooltipPopUp.HideInfo(); }

        else if (eventData.button == PointerEventData.InputButton.Right)
            Debug.Log("Right click");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("hola acabo de entrar");
        tooltipPopUp.DisplayInfo(slot.item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (slot.item != null)
        {
            tooltipPopUp.HideInfo();
        }
    }
}
