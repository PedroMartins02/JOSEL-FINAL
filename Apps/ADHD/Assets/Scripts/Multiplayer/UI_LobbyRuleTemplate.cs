using GameModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_LobbyRuleTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ruleTargetText;
    [SerializeField] private TextMeshProUGUI ruleValueText;

    private GameRule rule;


    private void Start()
    {

    }

    public void SetRule(GameRule rule)
    {
        this.rule = rule;

        ruleTargetText.text = rule.Target.ToString();

        if (rule.ValueType.Equals(ValueType.String))
            ruleValueText.text = rule.GetStringValue();
        else if (rule.ValueType.Equals(ValueType.Integer))
            ruleValueText.text = rule.GetIntValue().ToString();
    }

    public GameRule GetRule()
    {
        return rule;
    }
}
