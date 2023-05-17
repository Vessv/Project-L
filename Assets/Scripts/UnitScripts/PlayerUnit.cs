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
    public NetworkVariable<bool> IsDead = new NetworkVariable<bool>();

    public ItemInventory inventory;

    [SerializeField]
    bool isPointerOverUI = false;

    public GameObject ActionCanvas;
    public GameObject MapHolder;
    public GameObject ActionInventory;
    public GameObject BlessingDisplay;

    public GameObject EndUI;
    public GameObject ActionInventoryUI;
    public GameObject PlayerInfoUI;
    public GameObject ItemInventoryUI;
    public GameObject GameStateInfoUI;
    public GameObject NextFloorUI;

    public GameObject GameHandlerObject;

    public int FloorNumber = 0;

    public UnitSO[] UnitSOArray;

    private void Start()
    {
        if (!IsLocalPlayer) return;
        SubmitActionStateServerRpc(ActionState.Normal);
        ActionCanvas.SetActive(true);
        GameStateInfoUI.SetActive(true);
        inventory = GetComponent<ItemInventory>();
        inventory.Add(11);
        inventory.Add(12);
        inventory.Add(13);
        inventory.Add(1);
        Stats.OnValueChanged += OnStatsChange;
        IsMyTurn.OnValueChanged += OnMyTurnChange;
        CurrentHealth.OnValueChanged += OnHealthChangePlayer;
        ActionPoints.OnValueChanged += OnHealthChangePlayer;
        StartCoroutine(InitializeGameHandler());
    }

    IEnumerator InitializeGameHandler()
    {
        yield return new WaitForSeconds(2f);
        GameHandlerObject = GameHandler.Instance.gameObject;
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
        AudioManager.Instance.Play("inventory", true);


    }

    public void InventoryActionUI(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer || !context.performed) return;

        ActionInventoryUI.SetActive(!ActionInventoryUI.activeSelf);
        AudioManager.Instance.Play("inventory", true);

    }

    public void PlayerInformationUI(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer || !context.performed) return;

        PlayerInfoUI.SetActive(!PlayerInfoUI.activeSelf);
        AudioManager.Instance.Play("inventory", true);


    }

    public void Testing(InputAction.CallbackContext context) //quitar esto porfavor
    {
        /*if (!IsLocalPlayer || !context.performed) return;
        inventory.Add(0);
        inventory.Add(1);
        inventory.Add(2);
        inventory.Add(3);
        inventory.Add(12);
        inventory.Add(14);
        inventory.Add(17);

        //GameHandler.Instance.FloorEnd();*/
    }

    public override void Die()
    {
        base.Die();
        GameHandler.Instance.GetGrid().GetGridObject(transform.position).RemoveUnit();
        if (IsMyTurn.Value)
        {
            IsMyTurn.Value = false;
        }
        SetThreatServerRpc(-9999);
        UpdateIsDeadServerRpc(true);
        SpawnObjectClientRpc(transform.position + new Vector3(-0.5f, 0.5f), 9);
        //playsound?
    }


    bool wait = true;
    public void OnStatsChange(UnitSO.UnitStats previous, UnitSO.UnitStats current)
    {
        GameStateInfoUI.GetComponent<GameStateInfoUI>().UpdateUI();
        if (!wait)
        {
            PlayerInfoUI.GetComponentInChildren<PlayerInfoUI>().UpdateInfoUI();
        }
        wait = false;

    }

    public void OnMyTurnChange(bool previous, bool current)
    {
        if (IsDead.Value)
        {
            SetThreatServerRpc(-9999);
            SetIsMyTurnServerRpc(false);
        }
        GameStateInfoUI.GetComponent<GameStateInfoUI>().UpdateUI();
    }

    public void OnHealthChangePlayer(int previous, int current)
    {
        GameStateInfoUI.GetComponent<GameStateInfoUI>().UpdateUI();
    }


    [ClientRpc]
    public void UpdateTilesetGroundClientRpc(int index, bool active)
    {
        if (!IsLocalPlayer) return;
        GameHandler.Instance.GroundVariations[index].SetActive(active);
    }

    [ClientRpc]
    public void UpdateTilesetTopWallClientRpc(int index, bool active)
    {
        if (!IsLocalPlayer) return;
        GameHandler.Instance.TopwallVariations[index].SetActive(active);
    }
    [ClientRpc]
    public void UpdateTilesetBotWallClientRpc(int index, bool active)
    {
        if (!IsLocalPlayer) return;
        GameHandler.Instance.botwallVariations[index].SetActive(active);
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
    public void ActiveGameEndUIClientRpc()
    {
        if (!IsLocalPlayer) return;
        EndUI.SetActive(true);
    }
    
    [ClientRpc]
    public void NextFloorUIClientRpc()
    {
        if (!IsLocalPlayer) return;
        NextFloorUI.SetActive(true);
        NextFloorUI.GetComponent<Image>().color = new Color(0.1415094f, 0.1415094f, 0.1415094f, 1f);
        NextFloorUI.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(FadeNextFloorUI());
    }

    IEnumerator FadeNextFloorUI()
    {
        yield return new WaitForSeconds(2f);
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
    public void SetIsMyTurnServerRpc(bool isMyTurn)
    {
        IsMyTurn.Value = isMyTurn;
    }

    [ServerRpc]
    public void SetThreatServerRpc(int value)
    {
        Threat.Value = value;
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
    public void UpdateIsDeadServerRpc(bool isDead)
    {
        IsDead.Value = isDead;
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
