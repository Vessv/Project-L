using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemInventorySlot slot;
    public TooltipPopUp tooltipPopUp;

    private void OnEnable()
    {
        tooltipPopUp.HideInfo();
    }

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
        { }

        else if (eventData.button == PointerEventData.InputButton.Right)
        { slot.UseItem(); tooltipPopUp.HideInfo(); }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
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
