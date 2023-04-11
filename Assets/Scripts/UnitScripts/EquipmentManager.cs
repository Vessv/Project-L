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


    [SerializeField]
    ItemInventory inventory;
    private void Start()
    {
        inventory = GetComponent<ItemInventory>();

        //int numSlot = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        //currentEquipment = new Equipment[numSlot];
    }


    public void Equip(int itemID)
    {
        if (!IsLocalPlayer) return;
        EquipServerRPC((int)NetworkManager.Singleton.LocalClient.ClientId, itemID);
    }

    [ServerRpc]
    public void EquipServerRPC(int ClientID, int itemID)
    {
        int slotIndex = (int)GetEquipmentFromItemID(itemID).equipSlot;
        UnEquipServerRPC(ClientID, slotIndex);

        //inventory.Remove(itemID);
        currentEquipment[slotIndex].Value = itemID;
        NetworkObject unit = NetworkManager.Singleton.ConnectedClients[(ulong)ClientID].PlayerObject;
        unit.GetComponent<PlayerUnit>().Stats.Value += GetEquipmentFromItemID(itemID).extraStats;

    }
    [ServerRpc]
    public void UnEquipServerRPC(int ClientID, int slotIndex)
    {
        if (currentEquipment[slotIndex].Value > 0) //0 tambien es un item pero no es un equipment asi que lol tenerlo en cuenta
        {
            Equipment oldItem = GetEquipmentFromItemID(currentEquipment[slotIndex].Value);

            NetworkObject unit = NetworkManager.Singleton.ConnectedClients[(ulong)ClientID].PlayerObject;
            unit.GetComponent<PlayerUnit>().Stats.Value -= oldItem.extraStats; //cambiarlo cuando se cambie las Stats a variable de la network

            unit.GetComponent<ItemInventory>().Add(oldItem.itemID);
            currentEquipment[slotIndex].Value = -1;


        }
    }
    public Sprite GetSprite() //get sprite? //probalbmente ya no sirva
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

