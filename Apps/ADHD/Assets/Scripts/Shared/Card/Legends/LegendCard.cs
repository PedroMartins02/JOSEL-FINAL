using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

namespace GameModel
{
    public class LegendCard : UnitCard
    {
        public LegendCard(LegendCardSO cardSO) : base(cardSO.MapToUnitCardSO())
        {
            Effects = cardSO.Effects;
        }
    }
}