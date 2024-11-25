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

        public void AttackCard(ICombatCard attacker, ICombatCard defender)
        {
            Debug.Log($"Processing Attack: - Attacker: {attacker.CurrentAttack} Ad - Deffender: {defender.CurrentHealth} Hp");
            defender.TakeDamage(attacker.CurrentAttack);
            Debug.Log($"Attack Finished: - Attacker: {attacker.CurrentAttack} Ad - Deffender: {defender.CurrentHealth} Hp");

            EventManager.TriggerEvent(GameEventsEnum.CardAttacked, attacker.Data.Id);
        }
    }
}