using System.Collections;
using System.Collections.Generic;
using Game.Data;
using GameModel;
using UnityEngine;

public class LegendCardData : UnitCardData
{
    public readonly List<Effect> Effects;

    public LegendCardData(string id, string name,
            string shortText, string description,
            int blessings, int health, int attack,
            List<Effect> effects,
            Factions faction, Elements element) : base(id, name, shortText, description, blessings, health, attack, faction, element)
    {
        Effects = effects;
    }

    public LegendCardData(LegendCardData cardData, int gameID) : base(cardData, gameID)
    {
        Effects = cardData.Effects;
    }
}
