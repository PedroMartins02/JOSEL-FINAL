using Game.Data;
using GameModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythCard : Card
{
    public MythCard(CardData cardData) : base(cardData)
    {
        this.CurrentEffects = new List<Effect>();
        this.CurrentModifiers = new List<Modifier>();
        this.CurrentBlessingsCost = cardData.Blessings;
    }

    #region Conditions
    public override bool CanAttack
    {
        get
        {
            return false;
        }
    }

    public override bool IsTargatable
    {
        get
        {
            {
                return true;
            }
        }
    }
    #endregion
}
