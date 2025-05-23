using Game.Data;
using GameModel;
using System.Collections;
using System.Collections.Generic;
using Game.Logic.Modifiers;

public class MythCard : Card
{
    public MythCard(CardData cardData) : base(cardData)
    {
        this.CurrentEffects = new List<Effect>();
        this.CurrentModifiers = new List<Modifier>();
        this.CurrentBlessingsCost = cardData.Blessings;
    }

    public override CardDataSnapshot GetDataSnapshot()
    {
        return new CardDataSnapshot
        {
            Id = Data.Id,
            GameID = Data.GameID,
            CardType = CardType.Myth,
            StartingBlessings = Data.Blessings,
            CurrentBlessings = CurrentBlessingsCost,
            CurrentState = StateMachine.CurrentState.StateType,
        };
    }

    protected override CardData CardDataWithID(CardData cardData, int gameID)
    {
        return new CardData(cardData, gameID);
    }

    #region Modifiers
    public override void ApplyModifier(Modifier modifier)
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveModifier(Modifier modifier)
    {
        throw new System.NotImplementedException();
    }
    #endregion

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
