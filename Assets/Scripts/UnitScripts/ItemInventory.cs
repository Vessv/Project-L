using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ItemInventory : NetworkBehaviour
{

    // Callback which is triggered when
    // an item gets added/removed.
    public UnityEvent onItemChangedCallback;
    public NetworkList<int> itemsID;


    public int space = 20;  // Amount of slots in inventory

    // Current list of items in inventory

    public static ItemInventory instance;

    void Awake()
    {
        itemsID = new NetworkList<int>();
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }

        instance = this;
    }

    // Add a new item. If there is enough room we
    // return true. Else we return false.
    public bool Add(int item)
    {
        // Don't do anything if it's a default item
        if (true)
        {
            // Check if out of space
            if (itemsID.Count >= space)
            {
                Debug.Log("Not enough room.");
                return false;
            }

            SubmitItemToInvetoryServerRpc(item);    // Add item to list

            // Trigger callback
            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }

        return true;
    }

    // Remove an item
    public void Remove(int itemID)
    {
        itemsID.Remove(itemID);     // Remove item from list

        // Trigger callback
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }


    [ServerRpc]
    public void SubmitItemToInvetoryServerRpc(int itemID)
    {
        itemsID.Add(itemID);
    }
}
