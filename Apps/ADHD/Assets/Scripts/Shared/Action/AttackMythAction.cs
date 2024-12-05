using GameModel;
using System;
using UnityEngine;

namespace Game.Logic.Actions
{
    public class AttackMythAction : IAction
    {
        private Player attackingPlayer;
        private Player targetPlayer;
        private ICombatCard attackerCard;
        private ICombatSystem combatSystem;

        public AttackMythAction(Player attackingPlayer, Player targetPlayer, int attackerCardGameID, ICombatSystem combatSystem = null)
        {
            this.attackingPlayer = attackingPlayer;
            this.targetPlayer = targetPlayer;
            attackerCard = CardManager.Instance.GetCardByGameId(attackerCardGameID) as ICombatCard;

            if (attackerCard == null || !attackerCard.IsCombatCard) 
            {
                throw new ArgumentException("Tried to create an Attack Action with non Combat Card!");
            }

            this.combatSystem = combatSystem ?? CombatSystem.Instance;
        }

        public bool IsLegal()
        {
            return attackerCard.CanAttack 
                && TurnManager.Instance.IsCurrentPlayer(attackingPlayer.playerData.ClientId);
        }

        public void Execute()
        {
            Debug.Log("Executing Attack Myth Action!");

            int damageDealt = combatSystem.AttackMyth(attackerCard, targetPlayer);

            attackerCard.HandleAction(CardActions.Attack);

            ActionData actionData = new ActionData
            {
                ActionType = ActionType.AttackMyth,
                PlayerId = attackingPlayer.playerData.ClientId,
                TargetPlayerId = targetPlayer.playerData.ClientId,
                CardGameID = attackerCard.Data.GameID,
                Damage = damageDealt,
            };

            GameplayManager.Instance.BroadcastActionExecutedRpc(actionData);
        }
    }
}