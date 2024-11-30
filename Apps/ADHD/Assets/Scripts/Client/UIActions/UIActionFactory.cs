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
