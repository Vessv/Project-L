using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EquipmentManager : NetworkBehaviour
{
    public NetworkVariable<int>[] currentEquipment = new NetworkVariable<int>[4];

    //Equipment[] currentEquipment;

    //public delegate void OnEquipmentChange(Equipment newItem, Equipment oldItem);
    //public OnEquipmentChange onEquipmentChange;



    ItemInventory inventory;
    private void Start()
    {
        inventory = GetComponent<ItemInventory>();

        //int numSlot = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        //currentEquipment = new Equipment[numSlot];
    }

    [ServerRpc]
    public void EquipServerRPC(int itemID)
    {
        int slotIndex = (int)GetEquipmentFromItemID(itemID).equipSlot;
        UnEquipServerRPC(slotIndex);

        //inventory.Remove(itemID);
        currentEquipment[slotIndex].Value = itemID;
    }
    [ServerRpc]
    public void UnEquipServerRPC(int slotIndex)
    {
        if (currentEquipment[slotIndex].Value > 0) //0 tambien es un item pero no es un equipment asi que lol tenerlo en cuenta
        {
            Equipment oldItem = GetEquipmentFromItemID(currentEquipment[slotIndex].Value);

            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>().Stats -= oldItem.extraStats; //cambiarlo cuando se cambie las Stats a variable de la network

            inventory.Add(oldItem.itemID);
            currentEquipment[slotIndex].Value = -1;

        }
    }
    public Sprite GetSprite() //get sprite?
    {
        //if (currentEquipment[3].Value > 0) //0 tambien es un item pero no es un equipment asi que lol tenerlo en cuenta
        //{
        ///    Sprite sprite = (Sprite)GetEquipmentFromItemID(currentEquipment[3].Value).sprite;
        //    return sprite;
        //}
        return null;

    }

    public Equipment GetEquipmentFromItemID(int itemID)
    {
        return (Equipment)GameHandler.Instance.GetItemsSOArray()[itemID];
    }
}

