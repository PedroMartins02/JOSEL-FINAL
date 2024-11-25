using System.Collections;
using GameModel;
using System;
using UnityEngine;

namespace Game.Logic.Actions
{
    public class AttackCardAction : IAction
    {
        private ICombatCard attacker;
        private ICombatCard defender;
        private ICombatSystem combatSystem;

        public AttackCardAction(ICard attacker, ICard defender, ICombatSystem combatSystem = null)
        {
            if (!attacker.IsCombatCard || !defender.IsCombatCard) 
            {
                throw new ArgumentException("Tried to create an Attack Action with non Combat Card(s)!");
            }

            this.attacker = attacker as ICombatCard;
            this.defender = defender as ICombatCard;

            this.combatSystem = combatSystem ?? CombatSystem.Instance;
        }

        public bool IsLegal()
        {
            //Debug.Log($"Checking if Attack is Legal: {attacker.StateMachine.CurrentState.StateType} - {defender.StateMachine.CurrentState.StateType}");
            return true; // attacker.CanAttack && defender.IsTargatable;
        }

        public IEnumerator Execute()
        {
            combatSystem.AttackCard(attacker, defender);

            // Should wait for animations and other things here

            attacker.HandleAction(CardActions.Attack);

            yield return null;
        }
    }
}