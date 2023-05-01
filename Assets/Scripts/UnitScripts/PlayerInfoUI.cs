using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfoUI : MonoBehaviour
{
    public Image unitImage;
    public Image headImage;
    public Image chestImage;
    public Image feetImage;
    public Image weaponImage;
    public TextMeshProUGUI Strength;
    public TextMeshProUGUI Vitality;
    public TextMeshProUGUI Intelligence;
    public TextMeshProUGUI Speed;
    public TextMeshProUGUI Endurance;
    public TextMeshProUGUI Dexterity;
    public TextMeshProUGUI Stamina;
    public TextMeshProUGUI Luck;
    private void OnEnable()
    {
        UpdateInfoUI();
    }

    public void UpdateInfoUI()
    {
        GameObject playerObject = transform.root.gameObject;
        PlayerUnit unit = playerObject.GetComponent<PlayerUnit>();
        EquipmentManager equipmentManager = playerObject.GetComponent<EquipmentManager>();

        unitImage.sprite = unit.UnitScriptableObject.UnitSprite;

        Vitality.text = "Vitality: " + unit.Stats.Value.Vitality;
        Strength.text = "Strength: " + unit.Stats.Value.Strength;
        Intelligence.text = "Intelligence:  " + unit.Stats.Value.Intelligence;
        Speed.text = "Speed: " + unit.Stats.Value.Speed;
        Endurance.text = "Endurance: " + unit.Stats.Value.Endurance;
        Dexterity.text = "Dexterity: " + unit.Stats.Value.Dexterity;
        Stamina.text = "Stamina: " + unit.Stats.Value.Stamina;
        Luck.text = "Luck: " + unit.Stats.Value.Luck;

        if (equipmentManager.currentEquipment[0] > 0)
        {
            headImage.enabled = true;
            headImage.sprite = equipmentManager.GetEquipmentFromItemID(equipmentManager.currentEquipment[0]).icon;
        }
        else
        {
            headImage.sprite = null;
            headImage.enabled = false;
        }

        if (equipmentManager.currentEquipment[1] > 0)
        {
            chestImage.enabled = true;
            chestImage.sprite = equipmentManager.GetEquipmentFromItemID(equipmentManager.currentEquipment[1]).icon;
        }
        else
        {
            chestImage.sprite = null;
            chestImage.enabled = false;

        }

        if (equipmentManager.currentEquipment[2] > 0)
        {
            feetImage.enabled = true;
            feetImage.sprite = equipmentManager.GetEquipmentFromItemID(equipmentManager.currentEquipment[2]).icon;
        }
        else
        {
            feetImage.sprite = null;
            feetImage.enabled = false;

        }

        if (equipmentManager.currentEquipment[3] > 0)
        {
            weaponImage.enabled = true;
            weaponImage.sprite = equipmentManager.GetEquipmentFromItemID(equipmentManager.currentEquipment[3]).icon;
        }
        else
        {
            weaponImage.sprite = null;
            weaponImage.enabled = false;

        }
    }
}
