using System.Collections;
using System.Collections.Generic;
using GameCore.Events;
using Unity.Netcode;
using UnityEngine;

namespace Game.Logic.Actions.UI
{
    public class UIAttackCardAction : IUIAction
    {
        private int _attackingCardGameID;
        private int _targetCardGameID;
        private int _damage;
        private ulong _playerID;

        public UIAttackCardAction(int attackingCardGameID, int targetCardGameID, int damage, ulong playerID)
        {
            _attackingCardGameID = attackingCardGameID;
            _targetCardGameID = targetCardGameID;
            _damage = damage;
            _playerID = playerID;
        }

        public IEnumerator Execute()
        {
            EventManager.TriggerEvent(GameEventsEnum.CardAttacked, 
                new CardAttackedEventArgs 
                { 
                    AttackingCardGameID = _attackingCardGameID, 
                    TargetCardGameID = _targetCardGameID,
                    DamageDealt = _damage,
                    PlayerID = _playerID,
                });

            yield return null;
        }
    }
}