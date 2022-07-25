using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class PlayerUnit : BaseUnit
{
    private void Start()
    {
        if (!IsLocalPlayer) return;
        SubmitActionStateServerRpc(ActionState.Normal);
        SubmitPositionServerRpc(new Vector3(0.5f, 0.5f) + new Vector3(0f, (float)NetworkManager.Singleton.LocalClientId));

    }

    public void Click(InputAction.CallbackContext context)
    {
        if (!CanInteract || !context.performed) return;

        if (CanPlay)
        {
            SubmitTargetPositionServerRpc(Utils.GetMouseWorldPosition());
        }
    }
}
