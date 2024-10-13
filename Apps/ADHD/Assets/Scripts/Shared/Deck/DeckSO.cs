using GameModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameModel
{
    [CreateAssetMenu(fileName = "New Deck", menuName = "Decks/Deck")]
    public class DeckSO : ScriptableObject
    {
        public string Name;
        public Card Myth;
        public List<Card> Cards;
        public Factions Faction;
    }
}