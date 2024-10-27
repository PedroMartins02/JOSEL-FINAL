using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    [CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Unit Card")]
    public class UnitCardSO : CardSO
    {
        public int Attack;
        public int Health;
    }
}