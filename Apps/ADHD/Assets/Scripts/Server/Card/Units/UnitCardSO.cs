using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Unit Card")]
public class UnitCardSO : CardSO
{
    public int Attack;
    public int Health;
    public List<IModifier> Modifiers;
    public Elements Element;

    public CardSO MapToCardSO()
    {
        // TODO: Implement this shit
        return CreateInstance<CardSO>();
    }
}
