using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public class CardSO : ScriptableObject
    {
        public string Name;
        public int Blessings;
        public Sprite Art;
        public string ShortText;
        public string Description;
        public Factions Faction;
    }
}