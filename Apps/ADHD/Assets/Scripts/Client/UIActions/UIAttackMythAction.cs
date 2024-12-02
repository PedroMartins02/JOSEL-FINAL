using System.Collections;
using System.Collections.Generic;
using GameCore.Events;
using Unity.Netcode;
using UnityEngine;

namespace Game.Logic.Actions.UI
{
    public class UIAttackMythAction : IUIAction
    {
        private ulong _playerID;
        private ulong _targetPlayerID;
        private int _attackingCardGameID;
        private int _damageDealt;

        public UIAttackMythAction(ulong playerID, ulong targetPlayerID, int attackingCardGameID, int damageDealt)
        {
            _playerID = playerID;
            _targetPlayerID = targetPlayerID;
            _attackingCardGameID = attackingCardGameID;
            _damageDealt = damageDealt;
        }

        public IEnumerator Execute()
        {
            EventManager.TriggerEvent(GameEventsEnum.MythDamaged, 
                new MythAttackedEventArgs 
                { 
                    PlayerID = _playerID,
                    TargetPlayerID = _targetPlayerID,
                    AttackingCardGameID = _attackingCardGameID, 
                    DamageDealt = _damageDealt,
                });

            yield return null;
        }
    }
}