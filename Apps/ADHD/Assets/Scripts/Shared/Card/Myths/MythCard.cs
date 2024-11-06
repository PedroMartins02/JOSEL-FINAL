using GameModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythCard : Card
{
    public MythCard(MythCardSO cardSO)
    {
        this.Name = cardSO.Name;
        this.ShortText = cardSO.ShortText;
        this.Description = cardSO.Description;
        //this.Art = cardSO.Art.GetSpriteID();
        this.Faction = cardSO.Faction;
        this.Element = cardSO.Element;
        this.Effects = new List<Effect>();
        this.Modifiers = new List<Modifier>();
        this.Blessings = cardSO.Blessings;
    }
}
