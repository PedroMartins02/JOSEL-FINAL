using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModel;

namespace Game.Logic
{
    public interface ICombatSystem
    {
        public int AttackCard(ICombatCard attacker, ICombatCard defender);
        public int AttackMyth(ICombatCard attackerCard, Player targetPlayer);
    }
}