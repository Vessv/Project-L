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
        HealthSystem = new HealthSystem(Stats.Value.Vitality * 10);
        ActionPoints.Value = 2;
        ownedActionList = new NetworkList<int>();

    }

    public void LoadUnitStats()
    {
        Stats.Value = UnitScriptableObject.Stats;
    }

    public void TakeDamage(int damage)
    {
        if (HealthSystem.IsDead()) return;

        HealthSystem.Damage(damage);

        if (HealthSystem.IsDead()) Die();

        //AudioManager.Instance.Play("HumanPain");
    }

    public void Die()
    {
        //Die remove unit form turn handler, remove unit from grid
        this.gameObject.SetActive(false);
        Debug.Log(this + " Died");
    }

    public enum ActionState
    {
        Normal,
        Busy,
    }
}
