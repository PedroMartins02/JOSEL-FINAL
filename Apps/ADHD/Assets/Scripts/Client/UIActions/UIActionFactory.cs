using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic.Actions.UI
{
    public static class UIActionFactory
    {
        public static IUIAction CreateUIAction(ActionData actionData)
        {
            switch (actionData.ActionType)
            {
                case ActionType.DrawCard:
                    return new UIDrawCardAction(actionData.CardData, actionData.PlayerId);
                case ActionType.PlayCard:
                    return new UIPlayCardAction(actionData.CardGameID, actionData.PlayerId);
                case ActionType.Heal:

                    break;
                case ActionType.Effect:
                    
                    break;
                case ActionType.AttackCard:
                    return new UIAttackCardAction(actionData.CardGameID, actionData.TargetCardGameID, actionData.Damage, actionData.PlayerId);
                case ActionType.AttackMyth:
                    return new UIAttackMythAction(actionData.PlayerId, actionData.TargetPlayerId, actionData.CardGameID, actionData.Damage);
                default:
                    return null;
            }

            return null;
        }
    }
}
