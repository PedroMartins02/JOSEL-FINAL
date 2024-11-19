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
        public CardSO Myth;
        public List<CardSO> Cards;
        public Factions Faction;


        public DeckSO(string name, CardSO myth, List<CardSO> cards, Factions faction)
        {
            Name = name;
            Myth = myth;
            Cards = cards;
            Faction = faction;
        }
    }
}