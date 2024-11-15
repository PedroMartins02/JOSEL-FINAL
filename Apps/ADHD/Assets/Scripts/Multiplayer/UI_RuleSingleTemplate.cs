using GameModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UI_RuleSingleTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ruleTargetText;
    [SerializeField] private TMP_InputField ruleInputField;
    [SerializeField] private int inputCharacterLimit;


    private GameRule rule;


    private void Start()
    {
        ruleInputField.characterLimit = inputCharacterLimit;
        ruleInputField.onValueChanged.AddListener(OnInputValueChanged);

    }

    private void OnInputValueChanged(string newValue)
    {
        // Update the game rule value whenever the input changes
        if (rule.ValueType.Equals(ValueType.String))
            rule.SetStringValue(newValue);
        else if (rule.ValueType.Equals(ValueType.Integer))
        {
            rule.SetIntValue(newValue);
        }
    }

    public void SetRule(GameRule rule)
    {
        this.rule = rule;

        ruleTargetText.text = rule.Target.ToString();

        if(rule.ValueType.Equals(ValueType.String))
            ruleInputField.text = rule.GetStringValue();
        else if (rule.ValueType.Equals(ValueType.Integer))
            ruleInputField.text = rule.GetIntValue().ToString();
    }

    public GameRule GetRule()
    {
        return rule;
    }
}
