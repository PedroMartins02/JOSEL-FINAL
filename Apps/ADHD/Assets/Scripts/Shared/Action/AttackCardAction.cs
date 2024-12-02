using GameModel;
using System;

namespace Game.Logic.Actions
{
    public class AttackCardAction : IAction
    {
        private Player player;
        private ICombatCard attacker;
        private ICombatCard defender;
        private ICombatSystem combatSystem;

        public AttackCardAction(Player attackingPlayer, int attackerGameID, int defenderGameID, ICombatSystem combatSystem = null)
        {
            player = attackingPlayer;
            attacker = CardManager.Instance.GetCardByGameId(attackerGameID) as ICombatCard;
            defender = CardManager.Instance.GetCardByGameId(defenderGameID) as ICombatCard;

            if (attacker == null || defender == null || !attacker.IsCombatCard || !defender.IsCombatCard) 
            {
                throw new ArgumentException("Tried to create an Attack Action with non Combat Card(s)!");
            }

            this.combatSystem = combatSystem ?? CombatSystem.Instance;
        }

        public bool IsLegal()
        {
            //Debug.Log($"Checking if Attack is Legal: {attacker.StateMachine.CurrentState.StateType} - {defender.StateMachine.CurrentState.StateType}");
            return attacker.CanAttack 
                && defender.IsTargatable 
                && TurnManager.Instance.IsCurrentPlayer(player.playerData.ClientId);
        }

        public void Execute()
        {
            int damageDealt = combatSystem.AttackCard(attacker, defender);

            attacker.HandleAction(CardActions.Attack);

            ActionData actionData = new ActionData
            {
                ActionType = ActionType.AttackCard,
                PlayerId = player.playerData.ClientId,
                CardGameID = attacker.Data.GameID,
                TargetCardGameID = defender.Data.GameID,
                Damage = damageDealt,
            };

            GameplayManager.Instance.BroadcastActionExecutedRpc(actionData);
        }
    }
}