using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class PlayerUnit : BaseUnit, IPointerEnterHandler, IPointerExitHandler
{
    public ItemInventory inventory;

    [SerializeField]
    bool isPointerOverUI = false;

    public GameObject ActionCanvas;
    public GameObject MapHolder;
    public GameObject ActionInventory;
    public GameObject BlessingDisplay;

    public GameObject ActionInventoryUI;
    public GameObject PlayerInfoUI;
    public GameObject ItemInventoryUI;
    public GameObject GameStateInfoUI;
    public GameObject NextFloorUI;

    public UnitSO[] UnitSOArray;

    private void Start()
    {
        if (!IsLocalPlayer) return;
        SubmitActionStateServerRpc(ActionState.Normal);
        ActionCanvas.SetActive(true);
        GameStateInfoUI.SetActive(true);
        inventory = GetComponent<ItemInventory>();
        Stats.OnValueChanged += OnStatsChange;
        IsMyTurn.OnValueChanged += OnMyTurnChange;
        CurrentHealth.OnValueChanged += OnHealthChangePlayer;
        ActionPoints.OnValueChanged += OnHealthChangePlayer;
    }

    public void Click(InputAction.CallbackContext context)
    {
        if (!CanInteract || !context.performed) return;

        if (CanPlay && !isPointerOverUI)
        {
            Vector3 mousePosition = GetDifferentTargetPosition();
            HideAllMapVisualTileClientRpc();
            SubmitTargetPositionServerRpc(mousePosition);
        }
    }

    public void Inventory(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer || !context.performed) return;

        ItemInventoryUI.SetActive(!ItemInventoryUI.activeSelf);

    }

    public void InventoryActionUI(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer || !context.performed) return;

        ActionInventoryUI.SetActive(!ActionInventoryUI.activeSelf);

    }

    public void PlayerInformationUI(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer || !context.performed) return;

        PlayerInfoUI.SetActive(!PlayerInfoUI.activeSelf);

    }

     public void Testing(InputAction.CallbackContext context) //quitar esto porfavor
    {
        if (!IsLocalPlayer || !context.performed) return;
        inventory.Add(0);
        inventory.Add(1);
        inventory.Add(2);
        inventory.Add(3);
        inventory.Add(12);
        inventory.Add(14);
        inventory.Add(17);

        //GameHandler.Instance.FloorEnd();
    }

    public override void Die()
    {
        base.Die();
    }

    public void OnStatsChange(UnitSO.UnitStats previous, UnitSO.UnitStats current)
    {
        PlayerInfoUI.GetComponentInChildren<PlayerInfoUI>().UpdateInfoUI();
        GameStateInfoUI.GetComponent<GameStateInfoUI>().UpdateUI();

    }

    public void OnMyTurnChange(bool previous, bool current)
    {
        GameStateInfoUI.GetComponent<GameStateInfoUI>().UpdateUI();
    }

    public void OnHealthChangePlayer(int previous, int current)
    {
        GameStateInfoUI.GetComponent<GameStateInfoUI>().UpdateUI();
    }

    [ClientRpc]
    public void InitializeMapHolderClientRpc()
    {
        MapHolder = GameObject.FindGameObjectWithTag("MapVisualHolder");
    }

    //This function was made in case of trying to submit the same targetposition, i.e if the target hasn't moved.
    Vector3 GetDifferentTargetPosition()
    {
        DifferentPositionFlag = !DifferentPositionFlag;
        float yOffset = DifferentPositionFlag ? 0.001f : 0.002f;

        Vector3 mousePosition = Utils.GetMouseWorldPosition();
        if (mousePosition == TargetPosition.Value)
        {
            mousePosition = TargetPosition.Value + new Vector3(0, yOffset, 0);
        }
        return mousePosition;
    }

    
    [ClientRpc]
    public void NextFloorUIClientRpc()
    {
        if (!IsLocalPlayer) return;
        NextFloorUI.SetActive(true);
        NextFloorUI.GetComponent<Image>().color = new Color(0.1415094f, 0.1415094f, 0.1415094f, 1f);
        NextFloorUI.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(true);
        StartCoroutine(FadeNextFloorUI());
    }

    IEnumerator FadeNextFloorUI()
    {
        yield return new WaitForSeconds(1f);
        bool isVisible = true;
        float alpha = 1f;
        NextFloorUI.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        while (isVisible)
        {
            alpha -= 0.05f;
            NextFloorUI.GetComponent<Image>().color = new Color(0.1415094f, 0.1415094f, 0.1415094f, alpha);
            yield return new WaitForSeconds(0.05f);
            if(alpha <= 0f)
            {
                isVisible = false;
                NextFloorUI.SetActive(false);
                yield break;
            }
        }
        
    }

    [ClientRpc]
    public void SetMapVisualTileActiveClientRpc(int x, int y, Color color) //cambiar esto a una networklist
    {
        if(!IsLocalPlayer) return;
        Transform[,] MapVisualArray = MapHolder.GetComponent<MapVisual>().GridVisualArray;
        MapVisualArray[x, y].gameObject.SetActive(true);
        SpriteRenderer renderer = MapVisualArray[x, y].gameObject.GetComponent<SpriteRenderer>();
        renderer.color = color;
    }

    [ClientRpc]
    public void HideAllMapVisualTileClientRpc()
    {
        MapHolder.GetComponent<MapVisual>().HideAll();
    }

    [ClientRpc]
    public void DisplayBlessingSelectionClientRpc()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        BlessingDisplay.gameObject.SetActive(true);

    }

    [ClientRpc]
    public void UpdateUnitSOClientRpc(int index)
    {
        UpdateUnitSO(index);
        GetComponent<Animator>().SetInteger("chara_type", index);
    }
    
    public void UpdateUnitSO(int index)
    {
        UnitScriptableObject = UnitSOArray[index];
        GetComponent<SpriteRenderer>().sprite = UnitSOArray[index].UnitSprite;

    }

    [ServerRpc]
    public void SubmitUnitActionServerRpc(UnitAction.Action action)
    {
        SelectedAction.Value = action;
    }

    [ServerRpc]
    public void SubmitTargetPositionServerRpc(Vector3 targetPosition)
    {
        TargetPosition.Value = targetPosition;
    }

    [ServerRpc]
    public void SubmitActionStateServerRpc(ActionState state)
    {
        ActionStatus.Value = state;
    }

    [ServerRpc]
    public void SubmitExtraStatsServerRpc(UnitSO.UnitStats extraStats)
    {
        Stats.Value += extraStats;
    }

    [ServerRpc]
    public void RemoveExtraStatsServerRpc(UnitSO.UnitStats extraStats)
    {
        Stats.Value -= extraStats;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOverUI = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerUnit))]
public class UnitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerUnit unit = (PlayerUnit)target;
        //EditorGUILayout.LabelField("Strength", unit.Stats.Value.Strength.ToString());
        //EditorGUILayout.LabelField("Vitality", unit.Stats.Value.Vitality.ToString());
        // ...
    }
}
#endif
