using System;
using Game.Logic;
using GameModel;

namespace Game.Data
{
    [Serializable]
    public class UnitCardData : CardData
    {
        public readonly int Health;
        public readonly int Attack;


        public UnitCardData(string id, string name, 
            string shortText, string description, 
            int blessings, int health, int attack,
            Factions faction, Elements element) : base(id, name, shortText, description,blessings, faction, element)
        {
            Health = health;
            Attack = attack;
        }

        public UnitCardData(UnitCardData unitCardData, int gameID) : base(unitCardData, gameID)
        {
            Health = unitCardData.Health;
            Attack = unitCardData.Attack;
        }
    }
}