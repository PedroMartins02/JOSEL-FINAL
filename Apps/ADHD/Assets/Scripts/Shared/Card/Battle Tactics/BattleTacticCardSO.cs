using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    [CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Battle Tactic")]
    public class BattleTacticCardSO : CardSO
    {
        public List<Effect> Effects;
    }
}