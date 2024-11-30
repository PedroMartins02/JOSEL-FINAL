using System.Collections;
using System.Collections.Generic;
using Game.Logic;
using Game.Utils.Logic;
using Game.Data;

namespace GameModel
{
    public abstract class Card : ICard
    {
        public CardData Data { get; private set; }

        public int CurrentBlessingsCost { get; protected set; }
        public List<Effect> CurrentEffects { get; protected set; }
        public List<Modifier> CurrentModifiers { get; protected set; }

        public StateMachine<CardStateType, CardActions> StateMachine { get; protected set; }

        public Card(CardData cardData)
        {
            this.CurrentEffects = new List<Effect>();
            this.CurrentModifiers = new List<Modifier>();
            this.CurrentBlessingsCost = cardData.Blessings;

            int gameID = CardManager.Instance.RegisterCard(this);

            this.Data = CardDataWithID(cardData, gameID);

            CardManager.Instance.UpdateCard(this);
        }

        protected abstract CardData CardDataWithID(CardData cardData, int gameID);

        public abstract CardDataSnapshot GetDataSnapshot();

        public void HandleAction(CardActions cardActions)
        {
            StateMachine.HandleAction(cardActions);
        }

        public void GiveEffect(Effect newEffect)
        {

        }

        public void RemoveEffect(Effect effect)
        {

        }

        public void ApplyModifier(Modifier modifier)
        {

        }

        public void RemoveModifier(Modifier modifier)
        {

        }


        #region Modifiers
        public void IncreaseBlessingCost(int amount)
        {
            CurrentBlessingsCost += amount;
        }

        public void DecreaseBlessingCost(int amount)
        {
            CurrentBlessingsCost -= amount;

            if (CurrentBlessingsCost < 0)
                CurrentBlessingsCost = 0;
        }
        #endregion

        #region Conditions
        public abstract bool IsTargatable { get; }

        public abstract bool CanAttack { get; }

        public virtual bool IsCombatCard
        {
            get
            {
                return this is ICombatCard;
            }

        }
        #endregion
    }
}