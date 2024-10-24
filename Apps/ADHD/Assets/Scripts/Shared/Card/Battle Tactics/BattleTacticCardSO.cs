using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    [CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Battle Tactic Card")]
    public class BattleTacticCardSO : CardSO
    {

        public CardSO MapToCardSO()
        {
            // TODO: Implement this shit
            return CreateInstance<CardSO>();
        }
    }
}