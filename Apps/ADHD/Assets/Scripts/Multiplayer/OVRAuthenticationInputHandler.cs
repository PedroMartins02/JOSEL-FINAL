using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OVRAuthenticationInputHandler : OVRVirtualKeyboard.AbstractTextHandler
{
    public static OVRAuthenticationInputHandler Instance { get; private set; }


    /// <summary>
    /// Set an input field to connect to the Virtual Keyboard with the Unity Inspector
    /// </summary>
    [SerializeField]
    private TMP_InputField inputField;

    private bool _isSelected;

    /// <summary>
    /// Set/Get an input field to connect to the Virtual Keyboard at runtime
    /// </summary>
    public TMP_InputField InputField
    {
        get => inputField;
        set
        {
            if (value == inputField)
            {
                return;
            }
            if (inputField)
            {
                inputField.onValueChanged.RemoveListener(ProxyOnValueChanged);
            }
            inputField = value;
            if (inputField)
            {
                inputField.onValueChanged.AddListener(ProxyOnValueChanged);
            }
            OnTextChanged?.Invoke(Text);
        }
    }

    protected void Awake()
    {
        Instance = this;
    }

    protected void Start()
    {
        if (inputField)
        {
            inputField.onValueChanged.AddListener(ProxyOnValueChanged);
        }
        HideKeyboard();
    }

    public void ShowKeyboard()
    {
        this.gameObject.SetActive(true);
    }

    public void HideKeyboard()
    {
        this.gameObject.SetActive(false);
    }

    public override Action<string> OnTextChanged { get; set; }

    public override string Text => inputField ? inputField.text : string.Empty;

    public override bool SubmitOnEnter => inputField && inputField.lineType != TMP_InputField.LineType.MultiLineNewline;

    public override bool IsFocused => inputField && inputField.isFocused;

    public override void Submit()
    {
        if (!inputField)
        {
            return;
        }
        inputField.onEndEdit.Invoke(inputField.text);
        HideKeyboard();
    }

    public override void AppendText(string s)
    {
        if (!inputField)
        {
            return;
        }
        inputField.text += s;
    }

    public override void ApplyBackspace()
    {
        if (!inputField || string.IsNullOrEmpty(inputField.text))
        {
            return;
        }
        inputField.text = Text.Substring(0, Text.Length - 1);
    }

    public override void MoveTextEnd()
    {
        if (!inputField)
        {
            return;
        }
        inputField.MoveTextEnd(false);
    }

    protected void ProxyOnValueChanged(string arg0)
    {
        OnTextChanged?.Invoke(arg0);
    }
}
