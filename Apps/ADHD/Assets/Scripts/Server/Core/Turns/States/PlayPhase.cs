using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModel;

namespace GameCore
{
    public class PlayPhase : ITurnState
    {
        private TurnManager _turnManager;
        private Player _currentPlayer;

        public PlayPhase(TurnManager manager, Player player)
        {
            this._turnManager = manager;
            this._currentPlayer = player;
        }

        public void EnterState()
        {
            Debug.Log($"{_currentPlayer.Name} is now in the Play Phase!");

            // TODO: Implement card playing logic
        }

        public void ExitState()
        {
            
        }

        public void NextPhase()
        {
            _turnManager.EndTurn();
        }
    }
}
