using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot equipSlot;

    public UnitSO.UnitStats extraStats;
    public override void Use()
    {
        base.Use();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<EquipmentManager>().Equip(itemID);
        //cambiarlo cuando se cambie las Stats a variable de la network
        //EquipmentManager.instance.Equip(this);
        //RemoveFromInventory();
    }

    public override string GetTooltipInfoText()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("<color=#333333> ").Append(description).Append("</color>").AppendLine();
        builder.Append(GetStatsText());
        builder.Append("<color=#FFFF00> ").Append("Right Click: Use").Append("</color>").AppendLine();
        builder.Append(ColouredRarity()).AppendLine();
        return builder.ToString();
    }

    public string GetStatsText()
    {
        StringBuilder builder = new StringBuilder();

        builder.AppendLine();

        string plusText = "";

        if (extraStats.Vitality != 0)
        {
            builder.Append(plusText).Append("<color=#2E8B57> ").Append("VIT: ").Append(extraStats.Vitality).Append("</color>").AppendLine();

        }

        if (extraStats.Endurance != 0)
        {
            builder.Append(plusText).Append("<color=#FFA500> ").Append("END: ").Append(extraStats.Endurance).Append("</color>").AppendLine();

        }

        if (extraStats.Strength != 0)
        {
            builder.Append(plusText).Append("<color=#B22222> ").Append("STR: ").Append(extraStats.Strength).Append("</color>").AppendLine();

        }

        if (extraStats.Intelligence != 0)
        {
            builder.Append(plusText).Append("<color=#1E90FF> ").Append("INT: ").Append(extraStats.Intelligence).Append("</color>").AppendLine();

        }

        if (extraStats.Dexterity != 0)
        {
            builder.Append(plusText).Append("<color=#F5F5F5> ").Append("DEX: ").Append(extraStats.Dexterity).Append("</color>").AppendLine();

        }

        if (extraStats.Speed != 0)
        {
            builder.Append(plusText).Append("<color=#708090> ").Append("SPD: ").Append(extraStats.Speed).Append("</color>").AppendLine();

        }

        if (extraStats.Stamina != 0)
        {
            builder.Append(plusText).Append("<color=#9370DB> ").Append("STA: ").Append(extraStats.Stamina).Append("</color>").AppendLine();

        }

        builder.AppendLine();

        return builder.ToString();
    }

}
public enum EquipmentSlot { Head, Chest, Feet, Weapon }