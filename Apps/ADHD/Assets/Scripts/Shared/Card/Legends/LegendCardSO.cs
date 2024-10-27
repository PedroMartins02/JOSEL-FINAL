using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Legend Card", menuName = "Cards/Legend Card")]
public class LegendCardSO : CardSO
{
    public int Attack;
    public int Health;
    public List<Effect> Effects;

    public UnitCardSO MapToUnitCardSO()
    {
        UnitCardSO unitCardSO = new()
        {
            Name = Name,
            Art = Art,
            Faction = Faction,
            Element = Element,
            ShortText = ShortText,
            Description = Description,
            Blessings = Blessings,
            Attack = Attack,
            Health = Health,
        };

        return unitCardSO;
    }
}
