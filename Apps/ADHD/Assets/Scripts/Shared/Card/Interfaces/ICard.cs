using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Utils.Logic;
using Game.Logic.Modifiers;
using GameModel;
using UnityEngine;

namespace Game.Logic
{
    public interface ICard : IModifiable
    {
        public CardData Data { get; }
        public int CurrentBlessingsCost { get; }
        public List<Effect> CurrentEffects { get; }
        public List<Modifier> CurrentModifiers { get; }

        public StateMachine<CardStateType, CardActions> StateMachine { get; }
        public void HandleAction(CardActions cardActions);


        public void IncreaseBlessingCost(int amount);
        public void DecreaseBlessingCost(int amount);


        public CardDataSnapshot GetDataSnapshot();

        public bool IsTargatable { get; }
        public bool CanAttack { get; }
        public bool IsCombatCard { get; }
    }
}
