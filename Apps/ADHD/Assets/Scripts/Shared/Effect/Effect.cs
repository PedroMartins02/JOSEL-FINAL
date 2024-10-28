using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameModel
{
    [CreateAssetMenu(fileName = "New Card Effect", menuName = "Effect")]
    public class Effect : ScriptableObject
    {
        [SerializeField] protected List<Condition> conditionList;
        [SerializeField] protected List<Action> actionList;

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