using System;
using System.Collections.Generic;
using GameCore.Events;
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
            Player currentPlayer = _players[_currentPlayerIdx];

            EventManager.TriggerEvent(GameEventsEnum.TurnStarted, currentPlayer);
        }

        public void EndTurn() 
        {
            Player currentPlayer = _players[_currentPlayerIdx];

            EventManager.TriggerEvent(GameEventsEnum.TurnEnded, currentPlayer);

            _currentPlayerIdx = (_currentPlayerIdx + 1) % _players.Count;

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

        public void SetPhase()
        {
            throw new NotImplementedException();
        }
    }
}