using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ActionInventoryHandler : NetworkBehaviour
{
    public GameObject ActionPrefab;
    public ActionSO[] ActionsSOArray;
    PlayerUnit _unit;
    private void OnEnable()
    {
        if (!IsOwner) return;
        _unit = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>();
        for(int i = 0; i < _unit.ownedActionList.Count; i++)
        {
            Transform actionHolder = gameObject.transform.GetChild(i);
            actionHolder.GetComponent<ActionHolderUI>().ActionSO = ActionsSOArray[_unit.ownedActionList[i]-1];
            actionHolder.gameObject.SetActive(true);
        }
    }
}
