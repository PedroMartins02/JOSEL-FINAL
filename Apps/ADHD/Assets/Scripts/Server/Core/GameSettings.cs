using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public class GameSettings
    {
        public int StartingHand { get; private set; }
        public int CardsPerTurn { get; private set; }
        public int StartingBlessings { get; private set; }
        public int BlessingsPerTurn { get; private set; }
        public bool InfiniteBlessings { get; private set; }
        public int FieldSize { get; private set; } // Number of cards that can be in the field
        public int StartingHP { get; private set; }

        // TODO: Implementar os Setters com validações 
    }
}
