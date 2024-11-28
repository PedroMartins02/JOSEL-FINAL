using System.Collections.Generic;
using Game.Data;
using Game.Logic;
using Game.Utils.Logic;

namespace GameModel
{
    public class BattleTacticCard : Card
    {
        public BattleTacticCard(BattleTacticCardData cardData) : base(cardData)
        {
            this.CurrentEffects = cardData.Effects;
            this.CurrentModifiers = new List<Modifier>();

            StateMachine = new StateMachine<CardStateType, CardActions>();

            StateMachine.AddState(new InDeckState(this));
            StateMachine.AddState(new InHandState(this));
            StateMachine.AddState(new InPlayState(this));
            StateMachine.AddState(new DiscardedState(this));
            StateMachine.AddState(new ExhaustedState(this));

            StateMachine.SetState(CardStateType.InDeck);
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