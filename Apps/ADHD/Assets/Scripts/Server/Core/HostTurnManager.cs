using System;
using System.Collections.Generic;
using GameModel;

namespace GameCore
{
    public class HostTurnManager: ITurnManager
    {
        private List<Player> _players;
        private int _currentPlayerIdx;

        private ITurnState _currentState;

        public HostTurnManager(List<Player> playerList)
        {
            _players = playerList;
            _currentPlayerIdx = 0;
        }

        public void StartTurn() 
        {
            if (_currentPlayerIdx == 0)
                EventManager.TriggerEvent("RoundStart", null);

            Player currentPlayer = _players[_currentPlayerIdx];

            EventManager.TriggerEvent("TurnStart", currentPlayer);
        }

        public void EndTurn() 
        {
            Player currentPlayer = _players[_currentPlayerIdx];

            EventManager.TriggerEvent("TurnEnd", currentPlayer);

            _currentPlayerIdx = (_currentPlayerIdx + 1) % _players.Count;

            if (_currentPlayerIdx == 0)
                EventManager.TriggerEvent("RoundEnd", null);

            StartTurn();
        }

        public void SetState(ITurnState newState)
        {
            _currentState?.ExitState();
            _currentState = newState;
            _currentState.EnterState();
        }

        public void NextPhase()
        {
            _currentState?.NextPhase();
        }
    }
}