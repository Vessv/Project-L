using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class BaseUnit : NetworkBehaviour, IDamageable
{
    public NetworkVariable<bool> IsMyTurn = new NetworkVariable<bool>();
    public NetworkVariable<ActionState> ActionStatus = new NetworkVariable<ActionState>();
    public NetworkVariable<Vector3> UnitPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> TargetPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<UnitAction.Action> SelectedAction = new NetworkVariable<UnitAction.Action>();

    //Unit Stats
    public UnitSO UnitScriptableObject;
    public HealthSystem HealthSystem;
    public UnitSO.UnitStats Stats;

    public bool CanInteract => IsMyTurn.Value && IsLocalPlayer;
    public bool IsBusy => ActionStatus.Value == ActionState.Busy;
    public bool CanPlay => !IsBusy && SelectedAction.Value != UnitAction.Action.None;
    public bool CanMove => SelectedAction.Value == UnitAction.Action.Move;
    public bool CanShoot => SelectedAction.Value == UnitAction.Action.Shoot;

    private void Awake()
    {
        LoadUnitStats();
        HealthSystem = new HealthSystem(Stats.Vitality * 10);
    }

    public void LoadUnitStats()
    {
        Stats = UnitScriptableObject.Stats;
    }

    public void TakeDamage(int damage)
    {
        if (HealthSystem.IsDead()) return;

        HealthSystem.Damage(damage);

        if (HealthSystem.IsDead()) Die();
    }

    public void Die()
    {
        //Die
        Debug.Log(this + " Died");
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
    public void SubmitPositionServerRpc(Vector3 position)
    {
        UnitPosition.Value = position;
    }

    public enum ActionState
    {
        Normal,
        Busy,
    }
}
