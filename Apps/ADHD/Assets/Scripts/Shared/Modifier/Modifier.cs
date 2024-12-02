using System.Collections.Generic;
using Unity.VisualScripting;

namespace Game.Logic.Modifiers
{
    public abstract class Modifier
    {
        protected ModifierType _modifierType;

        protected Dictionary<ModifierTarget, int> _modifierDict;

        public abstract void Apply(IModifiable[] targets);
        public abstract void Remove(IModifiable[] targets);

        protected void AddModifierEffect(ModifierTarget modifierTarget, int modifierValue)
        {
            if (_modifierDict.ContainsKey(modifierTarget))
                _modifierDict[modifierTarget] = modifierValue;
            else 
                _modifierDict.Add(modifierTarget, modifierValue);
        }

        public ModifierType Type => _modifierType;
        public Dictionary<ModifierTarget, int> Dictionary => new Dictionary<ModifierTarget, int>(_modifierDict);
    }

    public enum ModifierTarget
    {
        Health,
        Attack,
        Blessings,
        All,
    }

    public enum ModifierType
    {
        GenericBuff,
        GenericDebuff,
    }
}