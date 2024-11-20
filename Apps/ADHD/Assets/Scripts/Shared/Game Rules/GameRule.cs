using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;

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
                int numVal = 0;

                if (!Int32.TryParse(value, out numVal))
                    return false;

                this.Value = numVal;
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
            return $"GameRule: {Target}, Type: {ValueType}, Value: {Value.ToString()}";
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

        public static GameRule FromString(string gameRule)
        {
            string pattern = @"GameRule: (?<Target>[^,]+), Type: (?<ValueType>[^,]+), Value: (?<Value>.+)";

            Match match = Regex.Match(gameRule, pattern);

            if (match.Success)
            {
                string targetString = match.Groups["Target"].Value;
                string valueTypeString = match.Groups["ValueType"].Value;
                string value = match.Groups["Value"].Value;

                Debug.Log("1: " + targetString);
                Debug.Log("2: " + valueTypeString);
                Debug.Log("3: " + value);

                ValueType valueType = (ValueType)Enum.Parse(typeof(ValueType), valueTypeString);
                RuleTarget target = (RuleTarget)Enum.Parse(typeof(RuleTarget), targetString);

                if (valueType.Equals(ValueType.Integer))
                    return new GameRule("", valueType, Int32.Parse(value), target);
                else if (valueType.Equals(ValueType.String))
                    return new GameRule("", valueType, value, target);

                return null;
            }
            else
            {
                throw new ArgumentException("The string format does not match the expected pattern.");
            }
        }

        public static string SerializeGameRules(List<GameRule> gameRules)
        {
            return string.Join("|", gameRules.Select(rule => rule.ToString()));
        }

        public static List<GameRule> DeserializeGameRules(string serializedRules)
        {
            string[] rules = serializedRules.Split('|');
            List<GameRule> gameRules = new List<GameRule>();

            foreach (string rule in rules)
            {
                if (!string.IsNullOrWhiteSpace(rule))
                {
                    GameRule gameRule = GameRule.FromString(rule);
                    gameRules.Add(gameRule);
                }
            }

            return gameRules;
        }
    }
}