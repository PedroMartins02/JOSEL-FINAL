using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic
{
    public interface ICombatCard : ICard
    {
        public int CurrentAttack { get; }
        public int CurrentHealth { get; }

        public void Heal(int amount);
        public void TakeDamage(int amount);

        public void AddAttack(int amount);
        public void DecreaseAttack(int amount);
    }
}