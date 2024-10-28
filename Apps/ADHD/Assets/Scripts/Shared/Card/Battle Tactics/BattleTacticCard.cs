using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public class BattleTacticCard : Card
    {


        public BattleTacticCard(BattleTacticCardSO cardSO)
        {
            this.Name = cardSO.Name;
            this.ShortText = cardSO.ShortText;
            this.Description = cardSO.Description;
            this.Blessings = cardSO.Blessings;
            //this.Art = cardSO.Art.GetSpriteID();
            this.Faction = cardSO.Faction;
            this.Effects = cardSO.Effects;
            this.Modifiers = new List<Modifier>();
        }
    }
}