using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.EventSystems;


public class PlayerUnit : BaseUnit, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    bool isPointerOverUI = false;

    public GameObject ActionCanvas;
    private void Start()
    {
        if (!IsLocalPlayer) return;
        SubmitActionStateServerRpc(ActionState.Normal);
        ActionCanvas.SetActive(true);
    }

    public void Click(InputAction.CallbackContext context)
    {
        if (!CanInteract || !context.performed) return;

        if (CanPlay && !isPointerOverUI)
        {
            Vector3 mousePosition = GetDifferentTargetPosition();

            SubmitTargetPositionServerRpc(mousePosition);
        }
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
