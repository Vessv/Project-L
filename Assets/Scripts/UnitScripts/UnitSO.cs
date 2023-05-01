using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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
        public int Intelligence;
        public int Speed;
        public int Endurance;
        public int Dexterity;
        public int Stamina;
        public int Luck;

        public static UnitStats operator +(UnitStats a, UnitStats b)
        {
            return new UnitStats()
            {
                Strength = a.Strength + b.Strength,
                Vitality = a.Vitality + b.Vitality,
                Intelligence = a.Intelligence + b.Intelligence,
                Speed = a.Speed + b.Speed,
                Endurance = a.Endurance + b.Endurance,
                Dexterity = a.Dexterity + b.Dexterity,
                Stamina = a.Stamina + b.Stamina,
                Luck = a.Luck + b.Luck
            };
        }

        public static UnitStats operator -(UnitStats a, UnitStats b)
        {
            return new UnitStats()
            {
                Strength = a.Strength - b.Strength,
                Vitality = a.Vitality - b.Vitality,
                Intelligence=a.Intelligence - b.Intelligence,
                Speed = a.Speed - b.Speed,
                Endurance = a.Endurance - b.Endurance,
                Dexterity = a.Dexterity - b.Dexterity,
                Stamina = a.Stamina - b.Stamina,
                Luck = a.Luck - b.Luck
            };
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Strength);
            serializer.SerializeValue(ref Vitality);
            serializer.SerializeValue(ref Intelligence);
            serializer.SerializeValue(ref Speed);
            serializer.SerializeValue(ref Endurance);
            serializer.SerializeValue(ref Dexterity);
            serializer.SerializeValue(ref Stamina);
            serializer.SerializeValue(ref Luck);
        }
    }

    
}
