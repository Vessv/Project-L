using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventoryUI : MonoBehaviour
{
	public Transform itemsParent;   // The parent object of all the items
	//public GameObject inventoryUI;  // The entire UI

	ItemInventory inventory;    // Our current inventory

	ItemInventorySlot[] slots;  // List of all the slots

	void Start()
	{
		inventory = ItemInventory.instance;
		inventory.onItemChangedCallback.AddListener(UpdateUI);    // Subscribe to the onItemChanged callback

		// Populate our slots array
		slots = itemsParent.GetComponentsInChildren<ItemInventorySlot>();
	}

	// Update the inventory UI by:
	//		- Adding items
	//		- Clearing empty slots
	// This is called using a delegate on the Inventory.
	void UpdateUI()
	{
		Debug.Log("Estoy funciando correctamente updateui");
		// Loop through all the slots
		for (int i = 0; i < slots.Length; i++)
		{
			if (i < inventory.itemsID.Count)  // If there is an item to add
			{
				Item itemToAdd = GameHandler.Instance.GetItemsSOArray()[inventory.itemsID[i]];
				slots[i].AddItem(itemToAdd);   // Add it
			}
			else
			{
				// Otherwise clear the slot
				slots[i].ClearSlot();
			}
		}
	}
}
