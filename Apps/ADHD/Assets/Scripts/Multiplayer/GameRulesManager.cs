using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

public class GameRulesManager
{
    private static GameRulesManager instance;

    public static GameRulesManager Instance 
    { 
        get
        {
            if (instance == null)
                instance = new GameRulesManager();

            return instance;
        }
    }

    private Dictionary<RuleTarget, GameRule> gameRules;

    private GameRulesManager()
    {
        gameRules = new();
    }

    public void IntializeGameRules(List<GameRule> gameRules)
    {
        this.gameRules = new();

        foreach (GameRule rule in gameRules)
        {
            this.gameRules[rule.Target] = rule;
        }
    }

    public void AddGameRule(GameRule gameRule)
    {
        gameRules[gameRule.Target] = gameRule;
    }

    public GameRule GetRule(RuleTarget target)
    {
        return gameRules[target];
    }

    public int GetIntRuleValue(RuleTarget target)
    {
        GameRule rule = gameRules[target];

        if (rule.ValueType != ValueType.Integer)
            return -1;
        else
            return gameRules[target].GetIntValue();
    }
}
