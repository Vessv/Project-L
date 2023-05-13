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
    public NetworkVariable<UnitSO.UnitStats> Stats;
    public NetworkVariable<int> Threat;

    public bool CanInteract => IsMyTurn.Value && IsLocalPlayer;
    public bool IsBusy => ActionStatus.Value == ActionState.Busy;
    public bool CanPlay => !IsBusy && SelectedAction.Value != UnitAction.Action.None;
    public bool CanMove => SelectedAction.Value == UnitAction.Action.Move;
    public bool CanMeele => SelectedAction.Value == UnitAction.Action.Meele;
    public bool CanRanged => SelectedAction.Value == UnitAction.Action.Ranged;
    public bool CanMagic => SelectedAction.Value == UnitAction.Action.Magic;
    public bool CanShieldBash => SelectedAction.Value == UnitAction.Action.ShieldBash;
    public bool CanFireball => SelectedAction.Value == UnitAction.Action.Fireball;
    public bool CanHeadbutt => SelectedAction.Value == UnitAction.Action.Headbutt;
    public bool CanMeteor => SelectedAction.Value == UnitAction.Action.Meteor;
    public bool CanPoison => SelectedAction.Value == UnitAction.Action.Poison;
    public bool CanStun => SelectedAction.Value == UnitAction.Action.Stun;
    public bool CanHoly => SelectedAction.Value == UnitAction.Action.Holy;
    public bool CanHeal => SelectedAction.Value == UnitAction.Action.Heal;
    public bool CanTree => SelectedAction.Value == UnitAction.Action.Tree;
    public bool CanTaunt => SelectedAction.Value == UnitAction.Action.Taunt;
    public bool CanIgnite => SelectedAction.Value == UnitAction.Action.Ignite;
    public bool CanCleave => SelectedAction.Value == UnitAction.Action.Cleave;
    public bool CanMist => SelectedAction.Value == UnitAction.Action.Mist;

    public bool DifferentPositionFlag;

    private void Awake()
    {
        LoadUnitStats();
        CurrentHealth.Value = Stats.Value.Vitality; //cambiar de health a esto
        CurrentHealth.OnValueChanged += OnHealthChanged;
        ownedActionList = new NetworkList<int>();

    }

    public void LoadUnitStats()
    {
        Stats.Value = UnitScriptableObject.Stats;
    }

    [ClientRpc]
    public void SpawnProjectileClientRpc(int objectIndex)
    {
        GameObject projectile = Instantiate(GameHandler.Instance.ProjectileSOArray[objectIndex], transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().targetPosition = TargetPosition.Value;
    }
    int meteorCount;
    [ClientRpc]
    public void SpawnMeteorRainClientRpc(Vector3 targetPosition)
    {
        meteorCount = 10;
        StartCoroutine(SpawnMeteor(targetPosition, GameHandler.Instance.ProjectileSOArray[3]));
    }

    IEnumerator SpawnMeteor(Vector3 targetPosition, GameObject meteorPrefab)
    {
        if(meteorCount > 0)
        {
            Vector3 offset = new Vector3(0f,0f);
            offset.x += Random.Range(-2f, 2f);
            offset.y += Random.Range(-2f, 2f);
            Instantiate(meteorPrefab, targetPosition+ offset-new Vector3(-0.5f, 0.5f), Quaternion.identity);
            meteorCount--;
            yield return new WaitForSeconds(0.15f);
            StartCoroutine(SpawnMeteor(targetPosition, meteorPrefab));
        }
    }

    [ClientRpc]
    public void SpawnObjectClientRpc(Vector3 targetPosition, int projectileIndex)
    {
        Instantiate(GameHandler.Instance.ProjectileSOArray[projectileIndex], targetPosition - new Vector3(-0.5f, 0.5f), Quaternion.identity);
    }

    [ClientRpc]
    public void TakeDamageClientRpc(int damage)
    {
        TakeDamage(damage);
    }
    public void TakeDamage(int damage)
    {
        if (isDead.Value) return;

        if (IsOwner)
        {
            if(damage > Stats.Value.Endurance)
            {
                TakeDamageServerRpc(damage);
            } else
            {
                damage = (int)Mathf.Floor(damage/2);
                TakeDamageServerRpc(damage);

            }
        }
        StartCoroutine(TakeDamageFlashRed());
        Debug.Log("dano tomado: " + damage);
        //AudioManager.Instance.Play("HumanPain"); Clientrpc o no
    }

    IEnumerator TakeDamageFlashRed()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(TakeDamageFlashRevert());
    }

    IEnumerator TakeDamageFlashRevert()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return null;
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
    public void UpdateWalkVariableClientRpc(bool value)
    {
        GetComponent<Animator>().SetBool("walking", value);
    }

    public enum ActionState
    {
        Normal,
        Busy,
    }
}
