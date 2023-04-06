using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item")]
public class Item : ScriptableObject
{
    new public string name = "New item";
    public Rarity rarity;
    public string description;
    public Sprite icon = null;
    public Sprite sprite = null;
    public bool isDefaultItem = false;
    public float sellPrice = 1;
    public int itemID;

    public virtual string ColouredName()
    {
        string hexColour = ColorUtility.ToHtmlStringRGB(rarity.textColour);
        return $"<color=#{hexColour}>{name}</color>";
    }
    public virtual string ColouredRarity()
    {
        string hexColour = ColorUtility.ToHtmlStringRGB(rarity.textColour);
        return $"<color=#{hexColour}>{rarity.name}</color>";
    }

    public virtual string GetTooltipInfoText()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("<color=green>Description: ").Append(description).Append("</color>").AppendLine();
        builder.Append("Sell price: ").Append(sellPrice).Append(" Gold").AppendLine();
        builder.Append(ColouredRarity()).AppendLine();
        return builder.ToString();
    }

    public virtual void Use()
    {

    }
    public void RemoveFromInventory()
    {
        ItemInventory.instance.Remove(this.itemID);
        Debug.Log("remover inventario:" + this);
    }
}