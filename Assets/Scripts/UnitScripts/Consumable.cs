using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


[CreateAssetMenu(fileName = "NewConsumable", menuName = "Item/Consumable")]
public class Consumable : Item
{
    public UnitSO.UnitStats extraStats;
    public int HealthRegen;
    public override void Use()
    {
        base.Use();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>().SubmitExtraStatsServerRpc(extraStats);
        if(HealthRegen > 0)
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>().SetCurrentHealthServerRpc(HealthRegen + NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>().CurrentHealth.Value);
        }
    }

}
