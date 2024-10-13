using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameModel
{
    public abstract class Effect
    {
        protected List<ICondition> conditionList;
        protected List<IAction> actionList;

        public void Apply()
        {
            bool AreConditionsMet = conditionList.All(x => x.IsTrue());

            if (!AreConditionsMet)
            {
                Debug.Log("Couldn't apply effects");
                return;
            }

            foreach (var action in actionList)
            {
                if (action.IsLegal())
                {
                    ActionQueueManager.AddPriorityAction(action);
                }
            }
        }
    }
}