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

        public int AttackCard(ICombatCard attacker, ICombatCard defender)
        {
            int damage = attacker.CurrentAttack;

            // Handle any other damage modifiers here

            defender.TakeDamage(damage);

            EventManager.TriggerEvent(GameEventsEnum.CardAttacked, attacker.Data.Id);

            return damage;
        }
    }
}