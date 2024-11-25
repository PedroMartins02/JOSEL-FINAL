using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic
{
    public enum CardStateType
    {
        Attacking,  // Currently Attacking another card or player
        InDeck,     // In the player's Deck
        InHand,     // In the player's Hand
        InPlay,     // Currently on the board (in the play area)
        Exhausted,  // Recovering after an attack for example
        Discarded,  // In Cemetery or something
        Stunned,    // Card can't perform any action
    }
}
