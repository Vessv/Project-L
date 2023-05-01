using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class BaseUnit : NetworkBehaviour, IDamageable
{
    public NetworkVariable<bool> IsMyTurn = new NetworkVariable<bool>();
    public NetworkVariable<ActionState> ActionStatus = new NetworkVariable<ActionState>();
    public NetworkVariable<Vector3> TargetPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<UnitAction.Action> SelectedAction = new NetworkVariable<UnitAction.Action>();
    public NetworkList<int> ownedActionList;


    public NetworkVariable<int> ActionPoints;

    //public int[] ownedActionArray = new int[4];
    //public List<int> ownedActionList = new List<int>();

    //Unit Stats
    public UnitSO UnitScriptableObject;
    public NetworkVariable<int> CurrentHealth;
    public NetworkVariable<bool> isDead;
    public HealthSystem HealthSystem;
    public NetworkVariable<UnitSO.UnitStats> Stats;
    public int Threat;

    public bool CanInteract => IsMyTurn.Value && IsLocalPlayer;
    public bool IsBusy => ActionStatus.Value == ActionState.Busy;
    public bool CanPlay => !IsBusy && SelectedAction.Value != UnitAction.Action.None;
    public bool CanMove => SelectedAction.Value == UnitAction.Action.Move;
    public bool CanShoot => SelectedAction.Value == UnitAction.Action.Shoot;

    public bool DifferentPositionFlag;

    private void Awake()
    {
        LoadUnitStats();
        HealthSystem = new HealthSystem(Stats.Value.Vitality);
        CurrentHealth.Value = Stats.Value.Vitality * 5; //cambiar de health a esto
        CurrentHealth.OnValueChanged += OnHealthChanged;
        ActionPoints.Value = 2;
        ownedActionList = new NetworkList<int>();

    }

    public void LoadUnitStats()
    {
        Stats.Value = UnitScriptableObject.Stats;
    }

    public void TakeDamage(int damage)
    {
        if (isDead.Value) return;

        TakeDamageServerRpc(damage);

        //AudioManager.Instance.Play("HumanPain"); Clientrpc
    }

    public virtual void Die()
    {
        //Die remove unit form turn handler, remove unit from grid
        Debug.Log(this + " Died");
    }

    public void OnHealthChanged(int previous, int current)
    {
        if (!IsOwner)
        {
            return;
        }
        if (current <= 0)
        {
            Die();
        }
        if (current > Stats.Value.Vitality)
        {
            SetCurrentHealthServerRpc(Stats.Value.Vitality);
        }
    }

    [ServerRpc]
    public void SetCurrentHealthServerRpc(int health)
    {
        CurrentHealth.Value = health;

    }

    [ServerRpc]
    public void TakeDamageServerRpc(int damage)
    {
        CurrentHealth.Value -= damage;
    }

    [ServerRpc]
    public void SetIsDeadServerRpc(bool trueorfalse)
    {
        isDead.Value = trueorfalse;
    }

    [ServerRpc]
    public void AddActionToListServerRpc(int actionID)
    {
        ownedActionList.Add(actionID);
    }

    [ClientRpc]
    public void UpdateWalkVariableClientRpc()
    {
        GetComponent<Animator>().SetBool("walking", !GetComponent<Animator>().GetBool("walking"));
    }

    public enum ActionState
    {
        Normal,
        Busy,
    }
}
