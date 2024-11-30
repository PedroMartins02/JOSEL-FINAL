using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

namespace Game.Data
{
    public class PlayerGameData
    {
        public readonly ulong ClientId;
        public readonly string Name;
        public readonly int Health;
        public readonly int Blessings;
        public readonly MythCard MythCard;
        public readonly Deck Deck;

        public PlayerGameData(ulong clientId, string name, int health, int blessings, MythCard mythCard, Deck deck)
        {
            ClientId = clientId;
            Name = name;
            Health = health;
            Blessings = blessings;
            MythCard = mythCard;
            Deck = deck;
        }
    }
}