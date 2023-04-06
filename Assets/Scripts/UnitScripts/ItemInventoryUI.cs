using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemInventoryUI : MonoBehaviour
{
	public Transform itemsParent;   // The parent object of all the items
	//public GameObject inventoryUI;  // The entire UI

	public GameObject PlayerObject;

	ItemInventory inventory;    // Our current inventory

	ItemInventorySlot[] slots;  // List of all the slots

	void Start()
	{
		inventory = PlayerObject.GetComponent<ItemInventory>();
		inventory.itemsID.OnListChanged += UpdateUI;
		//inventory.onItemChangedCallback.AddListener(UpdateUI);    // Subscribe to the onItemChanged callback
		// Populate our slots array
		slots = itemsParent.GetComponentsInChildren<ItemInventorySlot>();
	}

	// Update the inventory UI by:
	//		- Adding items
	//		- Clearing empty slots
	// This is called using a delegate on the Inventory.
	public void UpdateUI(NetworkListEvent<int> changeevent)
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
