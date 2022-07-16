using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New unit", menuName = "Unit")]
public class UnitSO : ScriptableObject
{
    public string UnitName;
    public Sprite UnitSprite;

    public int Strenght;
    public int Vitality;
    public int Agility;

    public Faction UnitFaction;
    public enum Faction
    {
        Hero,
        Demon,
        Undead
    }
}
