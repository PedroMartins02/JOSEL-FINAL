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

        protected override CardData CardDataWithID(CardData cardData, int gameID)
        {
            if (cardData.GetType() != typeof(LegendCardData))
                return null;

            return new LegendCardData((LegendCardData)cardData, gameID);
        }

        public override CardDataSnapshot GetDataSnapshot()
        {
            LegendCardData data = (LegendCardData)Data;

            return new CardDataSnapshot
            {
                Id = data.Id,
                GameID = data.GameID,
                CardType = CardType.Legend,
                StartingBlessings = data.Blessings,
                CurrentBlessings = CurrentBlessingsCost,
                StartingHealth = data.Health,
                CurrentHealth = CurrentHealth,
                StartingAttack = data.Attack,
                CurrentAttack = CurrentAttack,
                CurrentState = StateMachine.CurrentState.StateType,
            };
        }
    }
}