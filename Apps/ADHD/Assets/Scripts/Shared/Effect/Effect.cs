using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Logic.Actions;
using UnityEngine;


namespace GameModel
{
    [CreateAssetMenu(fileName = "New Card Effect", menuName = "Effect")]
    public class Effect : ScriptableObject
    {
        [SerializeField] protected List<Condition> conditionList;
        [SerializeField] protected List<IAction> actionList;

        public void Apply()
        {
            // Applies the effect
        }

        public void Remove()
        {
            // Removes the effect
        }
    }
}