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