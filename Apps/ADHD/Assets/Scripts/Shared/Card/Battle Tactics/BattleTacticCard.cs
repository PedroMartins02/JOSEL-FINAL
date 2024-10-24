using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public class BattleTacticCard : Card
    {


        public BattleTacticCard(BattleTacticCardSO cardSO) : base(cardSO.MapToCardSO())
        {

        }
    }
}