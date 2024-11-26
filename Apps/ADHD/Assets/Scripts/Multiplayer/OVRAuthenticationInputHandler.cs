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
    /// <summary>
    /// Set an input field to connect to the Virtual Keyboard with the Unity Inspector
    /// </summary>
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;

    private bool _isSelected;

    /// <summary>
    /// Set/Get an input field to connect to the Virtual Keyboard at runtime
    /// </summary>
    public TMP_InputField InputField
    {
        get => usernameField;
        set
        {
            if (value == usernameField)
            {
                return;
            }
            if (usernameField)
            {
                usernameField.onValueChanged.RemoveListener(ProxyOnValueChanged);
            }
            usernameField = value;
            if (usernameField)
            {
                usernameField.onValueChanged.AddListener(ProxyOnValueChanged);
            }
            OnTextChanged?.Invoke(Text);
        }
    }

    public override Action<string> OnTextChanged { get; set; }

    public override string Text => usernameField ? usernameField.text : string.Empty;

    public override bool SubmitOnEnter => usernameField;

    public override bool IsFocused => usernameField && usernameField.isFocused;

    public override void Submit()
    {
        if (!usernameField)
        {
            return;
        }
        usernameField.onEndEdit.Invoke(usernameField.text);
    }

    public override void AppendText(string s)
    {
        if (!usernameField)
        {
            return;
        }
        usernameField.text += s;
    }

    public override void ApplyBackspace()
    {
        if (!usernameField || string.IsNullOrEmpty(usernameField.text))
        {
            return;
        }
        usernameField.text = Text.Substring(0, Text.Length - 1);
    }

    public override void MoveTextEnd()
    {
        if (!usernameField)
        {
            return;
        }
        usernameField.MoveTextEnd(false);
    }

    protected void Start()
    {
        if (usernameField)
        {
            usernameField.onValueChanged.AddListener(ProxyOnValueChanged);
        }
    }

    protected void ProxyOnValueChanged(string arg0)
    {
        OnTextChanged?.Invoke(arg0);
    }
}
