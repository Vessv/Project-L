using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemInventoryUI : MonoBehaviour
{
	public Transform itemsParent;

	public GameObject PlayerObject;

	ItemInventory inventory; 

	ItemInventorySlot[] slots; 

	void Start()
	{
		inventory = PlayerObject.GetComponent<ItemInventory>();
		inventory.itemsID.OnListChanged += UpdateUI;
		slots = itemsParent.GetComponentsInChildren<ItemInventorySlot>();
	}

	public void UpdateUI(NetworkListEvent<int> changeevent)
	{
		// Loop through all the slots
		for (int i = 0; i < slots.Length; i++)
		{
			if (i < inventory.itemsID.Count)
			{
				Item itemToAdd = GameHandler.Instance.GetItemsSOArray()[inventory.itemsID[i]];
				slots[i].AddItem(itemToAdd); 
			}
			else
			{
				slots[i].ClearSlot();
			}
		}
	}
}
