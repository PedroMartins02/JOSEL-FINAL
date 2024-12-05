using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModel;
using System;

namespace Game.Logic
{
    public interface ICombatSystem
    {
        public Tuple<int, int> AttackCard(ICombatCard attacker, ICombatCard defender);
        public int AttackMyth(ICombatCard attackerCard, Player targetPlayer);
    }
}