using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.EventSystems;


public class PlayerUnit : BaseUnit, IPointerEnterHandler, IPointerExitHandler
{
    public ItemInventory inventory;

    [SerializeField]
    bool isPointerOverUI = false;

    public GameObject ActionCanvas;
    public GameObject MapHolder;
    public GameObject ActionInventory;
    public GameObject ItemInventoryUI;

    private void Start()
    {
        if (!IsLocalPlayer) return;
        SubmitActionStateServerRpc(ActionState.Normal);
        ActionCanvas.SetActive(true);
        inventory = GetComponent<ItemInventory>();
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

    public void Testing(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer || !context.performed) return;

        inventory.Add(0);
        inventory.Add(1);
        inventory.Add(2);
        inventory.Add(3);
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
    public void SetMapVisualTileActiveClientRpc(int x, int y) //cambiar esto a una networklist
    {
        if(!IsLocalPlayer) return;
        Transform[,] MapVisualArray = MapHolder.GetComponent<MapVisual>().GridVisualArray;
        MapVisualArray[x, y].gameObject.SetActive(true);
        SpriteRenderer renderer = MapVisualArray[x, y].gameObject.GetComponent<SpriteRenderer>();
        renderer.color = Color.red;
    }

    [ClientRpc]
    public void HideAllMapVisualTileClientRpc()
    {
        MapHolder.GetComponent<MapVisual>().HideAll();
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOverUI = false;
    }
}
