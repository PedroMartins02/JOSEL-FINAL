using System.Collections;
using System.Collections.Generic;
using Game.Data;
using GameModel;
using UnityEngine;

namespace GameModel
{
    public class LegendCard : UnitCard
    {
        public LegendCard(LegendCardData cardData) : base(cardData)
        {
            CurrentEffects = cardData.Effects;
        }
    }
}