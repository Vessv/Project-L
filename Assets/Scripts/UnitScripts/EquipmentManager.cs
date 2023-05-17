using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EquipmentManager : NetworkBehaviour
{
    public NetworkList<int> currentEquipment;

    //Equipment[] currentEquipment;

    //public delegate void OnEquipmentChange(Equipment newItem, Equipment oldItem);
    //public OnEquipmentChange onEquipmentChange;


    [SerializeField]
    ItemInventory inventory;
    private void Awake()
    {
        currentEquipment = new NetworkList<int>();
    }
    private void Start()
    {
        if(IsLocalPlayer)
            InitializeCurrentEquipmentListServerRpc();

        //int numSlot = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        //currentEquipment = new Equipment[numSlot];
    }


    public void Equip(int itemID)
    {
        if (!IsLocalPlayer) return;
        int slotIndex = (int)GetEquipmentFromItemID(itemID).equipSlot;


        Debug.Log("Current slot value: " + slotIndex);
        Debug.Log("Current itemIndex value: " + itemID);
        if (currentEquipment[slotIndex] > 0) //0 tambien es un item pero no es un equipment asi que lol tenerlo en cuenta 
        {
            Equipment oldItem = GetEquipmentFromItemID(currentEquipment[slotIndex]);

            GetComponent<PlayerUnit>().RemoveExtraStatsServerRpc(oldItem.extraStats); //quitar stats del item

            GetComponent<ItemInventory>().Add(oldItem.itemID);

            UnEquipServerRPC(slotIndex);

        }


        EquipServerRPC(slotIndex, itemID);
        GetComponent<PlayerUnit>().SubmitExtraStatsServerRpc(GetEquipmentFromItemID(itemID).extraStats);
        GetComponent<PlayerUnit>().PlayerInfoUI.GetComponentInChildren<PlayerInfoUI>().UpdateInfoUI();

    }

    [ServerRpc]
    public void EquipServerRPC(int slotIndex, int itemID)
    {
        currentEquipment[slotIndex] = itemID;
    }

    [ServerRpc]
    public void UnEquipServerRPC(int slotIndex)
    {
        currentEquipment[slotIndex] = -1;
    }

    [ServerRpc]
    public void InitializeCurrentEquipmentListServerRpc()
    {
        currentEquipment.Add(-1);
        currentEquipment.Add(-1);
        currentEquipment.Add(-1);
        currentEquipment.Add(-1);
    }

    public Equipment GetEquipmentFromItemID(int itemID)
    {
        return (Equipment)GameHandler.Instance.GetItemsSOArray()[itemID];
    }
}

