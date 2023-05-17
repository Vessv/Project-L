using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ActionInventoryHandler : MonoBehaviour
{
    public GameObject ActionPrefab;
    //public ActionSO[] ActionsSOArray;
    PlayerUnit _unit;
    private void OnEnable()
    {
        //if (!IsOwner) return;
        _unit = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnit>();
        for(int i = 0; i < _unit.ownedActionList.Count; i++)
        {
            Transform actionHolder = gameObject.transform.GetChild(i);
            actionHolder.GetComponent<ActionHolderUI>().ActionSO = GameHandler.Instance.GetActionsSOArray()[_unit.ownedActionList[i]-1];
            actionHolder.GetComponent<ActionHolderUI>().DraggableActionObject.GetComponent<DraggableImage>().ActionSO = GameHandler.Instance.GetActionsSOArray()[_unit.ownedActionList[i]-1];
            actionHolder.gameObject.SetActive(true);
        }
        StartCoroutine(UpdateUI());
    }

    IEnumerator UpdateUI()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < _unit.ownedActionList.Count; i++)
        {
            Transform actionHolder = gameObject.transform.GetChild(i);
            actionHolder.GetComponent<ActionHolderUI>().ActionSO = GameHandler.Instance.GetActionsSOArray()[_unit.ownedActionList[i] - 1];
            actionHolder.GetComponent<ActionHolderUI>().DraggableActionObject.GetComponent<DraggableImage>().ActionSO = GameHandler.Instance.GetActionsSOArray()[_unit.ownedActionList[i] - 1];
            actionHolder.gameObject.SetActive(true);
        }
    }
}
