using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


[CreateAssetMenu(fileName = "New Blessing", menuName = "Blessing/Blessing")]

public class BlessingSO : ScriptableObject
{
    public string title;
    public Rarity rarity;
    public string description;
    public Sprite sprite;
    public int itemID = -1;
    public int actionID;
    public UnitSO.UnitStats blessingStats;


    public void Get()
    {
        if(itemID != -1)
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>().inventory.Add(itemID);
        }
        if ( actionID != 0 )
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>().AddActionToListServerRpc(actionID);
        }
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>().SubmitExtraStatsServerRpc(blessingStats);
    }
}
