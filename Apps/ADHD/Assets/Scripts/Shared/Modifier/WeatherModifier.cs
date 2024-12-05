using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

namespace Game.Logic.Modifiers
{
    public class WeatherModifier : Modifier
    {
        int attackModifier;
        int healthModifier;

        public WeatherModifier(int attackAmount = 1, int healthAmount = 1)
        {
            _modifierDict = new();
            _modifierType = ModifierType.GenericBuff;

            attackModifier = attackAmount;
            healthModifier = healthAmount;

            AddModifierEffect(ModifierTarget.Attack, attackModifier);
            AddModifierEffect(ModifierTarget.Health, healthModifier);
        }

        public override void Apply(IModifiable[] targets)
        {
            foreach (var target in targets)
            {
                target.ApplyModifier(this);
            }
        }

        public override void Remove(IModifiable[] targets)
        {
            foreach (var target in targets)
            {
                target.RemoveModifier(this);
            }
        }
    }
}