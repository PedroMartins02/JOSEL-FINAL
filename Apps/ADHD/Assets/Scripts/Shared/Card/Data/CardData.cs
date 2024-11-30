using System;
using Game.Logic;
using GameModel;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

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

        // Only set after the card gets registered in the CardManager
        public readonly int GameID;

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

        public CardData(CardData cardData, int gameID)
        {
            Id = cardData.Id;
            Name = cardData.Name;
            ShortText = cardData.ShortText;
            Description = cardData.Description;
            Blessings = cardData.Blessings;
            Faction = cardData.Faction;
            Element = cardData.Element;

            GameID = gameID;
        }
    }
}