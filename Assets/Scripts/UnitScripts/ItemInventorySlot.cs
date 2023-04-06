using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventorySlot : MonoBehaviour
{
    public Image icon;          // Reference to the Icon image
    public Button removeButton; // Reference to the remove button

    public Item item;  // Current item in the slot
    public ItemInventory inventory;


    private void Start()
    {
        inventory = transform.root.GetComponent<ItemInventory>();
    }

    // Add item to the slot
    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    // Clear the slot
    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    // Called when the remove button is pressed
    public int GetItemID()
    {
        return item.itemID;
    }

    public void OnRemoveButton()
    {
        inventory.Remove(item.itemID);
    }

    // Called when the item is pressed
    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }
}
