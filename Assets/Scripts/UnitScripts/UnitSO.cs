using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New unit", menuName = "BaseUnit")]
public class UnitSO : ScriptableObject
{
    public string UnitName;
    public Sprite UnitSprite;

    public UnitStats Stats;

    public Faction UnitFaction;
    public enum Faction
    {
        Hero,
        Demon,
        Undead
    }

    [System.Serializable]
    public struct UnitStats
    {
        public int Strength;
        public int Vitality;
        public int Speed;
        public int Endurance;
        public int Dexterity;
        public int Stamina;
        public int Luck;
    }
}
