using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

namespace Game.Logic.Actions
{
    public static class ActionFactory
    {
        public static IAction CreateAction(ActionData actionData)
        {
            Player player;

            switch (actionData.ActionType)
            {
                case ActionType.DrawCard:
                    player = PlayerManager.Instance.GetPlayerByClientId(actionData.PlayerId);

                    return new DrawCardAction(player, actionData.NumberOfCardsDrawn);
                case ActionType.PlayCard:
                    player = PlayerManager.Instance.GetPlayerByClientId(actionData.PlayerId);

                    return new PlayCardAction(player, actionData.CardGameID);
                case ActionType.Heal:

                    break;
                case ActionType.Effect:

                    break;
                case ActionType.AttackCard:
                    player = PlayerManager.Instance.GetPlayerByClientId(actionData.PlayerId);

                    return new AttackCardAction(player, actionData.CardGameID, actionData.TargetCardGameID);
                case ActionType.AttackMyth:
                    player = PlayerManager.Instance.GetPlayerByClientId(actionData.PlayerId);

                    Player targetPlayer = PlayerManager.Instance.GetPlayerByClientId(actionData.TargetPlayerId);

                    return new AttackMythAction(player, targetPlayer, actionData.CardGameID);
                default:
                    return null;
            }

            return null;
        }
    }
}