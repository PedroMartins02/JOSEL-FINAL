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
            switch (actionData.ActionType)
            {
                case ActionType.DrawCard:
                    Player player = PlayerManager.Instance.GetPlayerByClientId(actionData.PlayerId);

                    return new DrawCardAction(player, actionData.NumberOfCardsDrawn);
                case ActionType.PlayCard:

                    break;
                case ActionType.Heal:

                    break;
                case ActionType.Effect:

                    break;
                case ActionType.Attack:

                    break;
                default:
                    return null;
            }

            return null;
        }
    }
}