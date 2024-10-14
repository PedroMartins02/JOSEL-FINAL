using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameModel
{
    public abstract class Card
    {
        public string Name { get; private set; }
        public string ShortText { get; private set; }
        public string Description { get; private set; }
        public int Blessings { get; private set; }
        //public GUID Art { get; private set; } // TODO: Verificar como guardar uma referencia para o asset
        public Factions Faction { get; private set; }

        public Card(CardSO cardSO)
        {
            this.Name = cardSO.Name;
            this.ShortText = cardSO.ShortText;
            this.Description = cardSO.Description;
            this.Blessings = cardSO.Blessings;
            //this.Art = cardSO.Art.GetSpriteID();
            this.Faction = cardSO.Faction;
        }
    }
}