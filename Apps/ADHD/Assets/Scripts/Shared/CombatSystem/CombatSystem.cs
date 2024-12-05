using System;
using GameCore.Events;
using GameModel;
using UnityEngine;

namespace Game.Logic
{
    public class CombatSystem : MonoBehaviour, ICombatSystem
    {
        public static CombatSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public Tuple<int, int> AttackCard(ICombatCard attacker, ICombatCard defender)
        {
            int damageDealt = attacker.CurrentAttack;

            // Handle any other damage modifiers here

            defender.TakeDamage(damageDealt);

            EventManager.TriggerEvent(GameEventsEnum.CardAttacked, attacker.Data.Id);

            int damageReceived = defender.CurrentAttack;

            // Handle any other damage modifiers here

            attacker.TakeDamage(damageReceived);

            EventManager.TriggerEvent(GameEventsEnum.CardDamaged, attacker.Data.Id);

            return new Tuple<int, int>(damageDealt, damageReceived);
        }

        public int AttackMyth(ICombatCard attackerCard, Player targetPlayer)
        {
            int damage = attackerCard.CurrentAttack;

            // Handle any other damage modifiers here

            targetPlayer.Damage(damage);

            return damage;
        }
    }
}