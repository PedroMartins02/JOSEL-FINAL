using System;
using GameModel;

namespace Game.Data
{
    [Serializable]
    public class CardData
    {
        public readonly string Id;
        public readonly string Name;
        public readonly string ShortText;
        public readonly string Description;
        public readonly int Blessings;
        public readonly Factions Faction;
        public readonly Elements Element;

        public CardData(string id, string name, string shortText, string description, int blessings, Factions faction, Elements element)
        {
            Id = id;
            Name = name;
            ShortText = shortText;
            Description = description;
            Blessings = blessings;
            Faction = faction;
            Element = element;
        }
    }
}