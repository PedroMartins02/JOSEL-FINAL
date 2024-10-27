using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public class CardSO : ScriptableObject
    {
        public string Name;
        public Sprite Art;
        public Factions Faction;
        public Elements Element;
        public string ShortText;
        public string Description;
        public int Blessings;
    }
}