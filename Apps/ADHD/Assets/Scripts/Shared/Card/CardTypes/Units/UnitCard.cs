using System;
using Game.Data;
using Game.Logic;
using Game.Utils.Logic;
using GameCore.Events;
using UnityEngine;

namespace GameModel
{
    public class UnitCard : Card, ICombatCard
    {
        public int CurrentAttack { get; protected set; }
        public int CurrentHealth { get; protected set; }

        public UnitCard(UnitCardData cardData) : base(cardData)
        {
            this.CurrentAttack = cardData.Attack;
            this.CurrentHealth = cardData.Health;

            StateMachine = new StateMachine<CardStateType, CardActions>();

            StateMachine.AddState(new InDeckState(this));
            StateMachine.AddState(new InHandState(this));
            StateMachine.AddState(new InPlayState(this));
            StateMachine.AddState(new DiscardedState(this));
            StateMachine.AddState(new ExhaustedState(this));

            StateMachine.SetState(CardStateType.InDeck);
        }

        public void Update()
        {
            StateMachine.Update();
        }

        public void Heal(int amount)
        {
            if (!CanReceiveHealth)
                return;

            CurrentHealth += amount;

            EventManager.TriggerEvent(GameEventsEnum.CardHealed, this);
        }

        public void TakeDamage(int amount)
        {
            //if (!CanTakeDamage)
            //    return;

            CurrentHealth -= amount;

            EventManager.TriggerEvent(GameEventsEnum.CardDamaged, this);

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Stun(int stunDuration)
        {
            StunnedState stunnedState = new(this, stunDuration);

            StateMachine.AddState(stunnedState);
        }

        private void Die()
        {
            StateMachine.SetState(CardStateType.Discarded);

            Debug.Log("Card Died");

            EventManager.TriggerEvent(GameEventsEnum.CardDied, this);
        }

        #region Modifiers
        public void AddAttack(int amount)
        {
            CurrentAttack += amount;
        }

        public void DecreaseAttack(int amount)
        {
            CurrentAttack -= amount;

            if (CurrentAttack < 0)
                CurrentAttack = 0;
        }
        #endregion

        #region Conditions
        public override bool IsTargatable
        {
            get
            {
                return StateMachine.CurrentState is InPlayState;
            }
        }

        public override bool CanAttack
        {
            get
            {
                Type currentStateType = StateMachine.CurrentState.GetType();

                return StateMachine.CurrentState is InPlayState
                    && currentStateType != typeof(ExhaustedState)
                    && currentStateType != typeof(StunnedState);
            }
        }

        public virtual bool CanTakeDamage
        {
            get
            {
                return StateMachine.CurrentState is InPlayState;
            }
        }

        public virtual bool CanReceiveHealth
        {
            get
            {
                return StateMachine.CurrentState is InPlayState;
            }
        }
        #endregion
    }
}