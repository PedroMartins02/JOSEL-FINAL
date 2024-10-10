using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCard : Card
{


    public UnitCard(UnitCardSO cardSO) : base(cardSO.MapToCardSO())
    {

    }
}
