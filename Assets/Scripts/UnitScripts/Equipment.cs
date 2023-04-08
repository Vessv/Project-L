using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot equipSlot;

    public UnitSO.UnitStats extraStats;
    public override void Use()
    {
        base.Use();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<EquipmentManager>().EquipServerRPC(itemID);

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>().Stats += extraStats; //cambiarlo cuando se cambie las Stats a variable de la network
        //EquipmentManager.instance.Equip(this);
        //RemoveFromInventory();
    }

}
public enum EquipmentSlot { Head, Chest, Feet, Weapon }