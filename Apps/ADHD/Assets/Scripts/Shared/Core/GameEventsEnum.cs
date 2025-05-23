using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Logic;
using UnityEngine;


namespace GameCore.Events
{
    public enum GameEventsEnum
    {
        // General Events
        GameStarted,
        GameOver,
        ActionExecuted,
        PlayerInfoChanged,
        // Turn Events
        TurnStarted,
        TurnEnded,
        // Deck Events
        CardDrawn,
        DeckExhausted,
        // Hand Events
        CardAddedToHand,
        // Card Events
        CardPlayed,
        CardAttacked,
        CardDamaged,
        CardHealed,
        CardDied,
        CardStateChanged,
        // Myth Events
        MythDamaged,
        MythHealed,
    }

    public struct CardDrawnEventArgs
    {
        public CardDataSnapshot CardData;
        public ulong PlayerID;
    }

    public struct CardPlayedEventArgs
    {
        public int CardGameID;
        public ulong PlayerID;
    }

    public struct  CardAttackedEventArgs
    {
        public ulong PlayerID;
        public int AttackingCardGameID;
        public int TargetCardGameID;
        public int DamageDealt;
    }

    public struct MythAttackedEventArgs
    {
        public ulong PlayerID;
        public ulong TargetPlayerID;
        public int AttackingCardGameID;
        public int DamageDealt;
    }

    public struct CardStateChangedEventArgs
    {
        public int CardGameID;
        public CardStateType OldCardState;
        public CardStateType NewCardState;
    }
}