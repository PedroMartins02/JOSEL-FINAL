using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.Events
{
    public enum GameEventsEnum
    {
        // General Events
        GameStarted,
        GameOver,
        ActionExecuted,
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
        // Myth Events
        MythDamaged,
        MythHealed,
    }
}