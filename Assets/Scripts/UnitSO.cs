using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New unit", menuName = "Unit")]
public class UnitSO : ScriptableObject
{
    public string unitName;
    public Sprite unitSprite;

    public int strenght;
    public int vitality;
    public int agility;

    public Faction faction;
    public enum Faction
    {
        Hero,
        Demon,
        Undead
    }
}
