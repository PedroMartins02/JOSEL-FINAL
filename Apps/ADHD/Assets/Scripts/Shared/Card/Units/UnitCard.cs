using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public class UnitCard : Card
    {
        private int attack;
        private int health;

        public UnitCard(UnitCardSO cardSO)
        {
            this.Name = cardSO.Name;
            this.ShortText = cardSO.ShortText;
            this.Description = cardSO.Description;
            //this.Art = cardSO.Art.GetSpriteID();
            this.Faction = cardSO.Faction;
            this.Effects = new List<Effect>();
            this.Modifiers = new List<Modifier>();
            this.Blessings = cardSO.Blessings;
            this.attack = cardSO.Attack;
            this.health = cardSO.Health;
        }
    }
}