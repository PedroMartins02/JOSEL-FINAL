using System;
using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class BattleTacticCardData : CardData
    {
        public readonly List<Effect> Effects;

        public BattleTacticCardData(string id, string name,
            string shortText, string description,
            int blessings,
            List<Effect> effects,
            Factions faction, Elements element) : base(id, name, shortText, description, blessings, faction, element)
        {
            this.Effects = effects;
        }
    }
}