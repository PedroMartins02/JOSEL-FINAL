using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        StartingHandSize,
        BlessingPerTurn,
        StartingBlessings,
        CardsDrawnPerTurn,
        MaximumBlessings
    }

    [Serializable]
    public class GameRule
    {
        [SerializeField]
        public string Description { get; private set; }

        public ValueType ValueType { get; private set; }

        public RuleTarget Target { get; private set; }

        [SerializeField]
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
                this.Target = target;
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

        public bool SetIntValue(int value)
        {
            if (ValueType != ValueType.Integer)
                return false;

            if (value <= 0)
            {
                this.Value = value;
                return false;
            }

            return true;
        }

        public bool SetIntValue(string value)
        {
            if (ValueType != ValueType.Integer)
                return false;

            try
            {
                int numVal = Int32.Parse(value);
                this.Value = value;
                return true;
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public string GetStringValue()
        {
            if (ValueType != ValueType.String)
                throw new InvalidOperationException($"Cannot get string value from a {ValueType} rule.");

            return (string)Value;
        }

        public bool SetStringValue(string value)
        {
            if (ValueType != ValueType.String)
                return false;

            this.Value = value;
            return true;
        }

        public override string ToString()
        {
            return $"GameRule: {Target}, Type: {ValueType}, Value: {Value}";
        }

        // Hopefully temporary
        public static List<GameRule> GetDefaultRules()
        {
            List<GameRule> rules = new List<GameRule>();

            rules.Add(new GameRule("", ValueType.Integer, 20, RuleTarget.StartingHealth));
            rules.Add(new GameRule("", ValueType.Integer, 5, RuleTarget.StartingHandSize));
            rules.Add(new GameRule("", ValueType.Integer, 1, RuleTarget.CardsDrawnPerTurn));
            rules.Add(new GameRule("", ValueType.Integer, 2, RuleTarget.StartingBlessings));
            rules.Add(new GameRule("", ValueType.Integer, 1, RuleTarget.BlessingPerTurn));
            rules.Add(new GameRule("", ValueType.Integer, 1, RuleTarget.MaximumBlessings));

            return rules;
        }
    }
}