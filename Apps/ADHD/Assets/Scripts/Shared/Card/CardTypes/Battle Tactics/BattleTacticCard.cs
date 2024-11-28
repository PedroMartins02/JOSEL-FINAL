using System.Collections.Generic;
using Game.Data;

namespace GameModel
{
    public class BattleTacticCard : Card
    {
        public BattleTacticCard(BattleTacticCardData cardData) : base(cardData)
        {
            this.CurrentEffects = cardData.Effects;
            this.CurrentModifiers = new List<Modifier>();
        }

        public override CardDataSnapshot GetDataSnapshot()
        {
            BattleTacticCardData data = (BattleTacticCardData)Data;

            return new CardDataSnapshot
            {
                Id = data.Id,
                GameID = data.GameID,
                CardType = CardType.BattleTactic,
                StartingBlessings = data.Blessings,
                CurrentBlessings = CurrentBlessingsCost,
                CurrentState = StateMachine.CurrentState.StateType,
            };
        }

        protected override CardData CardDataWithID(CardData cardData, int gameID)
        {
            if (cardData.GetType() != typeof(BattleTacticCardData))
                return null;

            return new BattleTacticCardData((BattleTacticCardData)cardData, gameID);
        }

        #region Conditions
        public override bool IsTargatable
        {
            get
            {
                return false;
            }
        }

        public override bool CanAttack 
        {
            get
            {
                return false;
            }
        }

       
        #endregion
    }
}