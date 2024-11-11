using System;

namespace GameModel
{
    // While currently we only use integer types, in the future String types of rules can be used
    // to define gamemodes, for example, or other stuff
    public enum ValueType
    {
        Integer,
        String
    }

    public enum RuleTarget
    {
        StartingHealth,
        ManaPerTurn,
        StartingMana,
        CardsDrawnPerTurn,
    }

    public class GameRule
    {
        public string Description { get; private set; }

        public ValueType ValueType { get; private set; }

        public RuleTarget Target { get; private set; }

        private object Value;


        public GameRule(string description, ValueType valueType, object value, RuleTarget target) 
        {
            // Verifications
            if (value == null ||
                (valueType == ValueType.Integer && value is int) ||
                (valueType == ValueType.String && value is string))
            {
                this.Description = description;
                this.ValueType = valueType;
                this.Value = value;
            }
            else
            {
                throw new ArgumentException($"Value type does not match the Type for {target}");
            }

        }

        public int GetIntValue()
        {
            if (ValueType != ValueType.Integer)
                throw new InvalidOperationException($"Cannot get integer value from a {ValueType} rule.");

            return (int)Value;
        }

        public string GetStringValue()
        {
            if (ValueType != ValueType.String)
                throw new InvalidOperationException($"Cannot get string value from a {ValueType} rule.");

            return (string)Value;
        }

        public override string ToString()
        {
            return $"GameRule: {Target}, Type: {ValueType}, Value: {Value}";
        }

    }
}