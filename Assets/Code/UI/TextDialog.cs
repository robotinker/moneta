using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDialog : DialogPanelBase
{
    public Text Prompt;
    public InputField TextInput;

    Action<string> OnConfirmed;

    public static TextDialog Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Init(string prompt, Action<string> onConfirmed)
    {
        Prompt.text = prompt;
        TextInput.text = "";
        OnConfirmed = onConfirmed;
        Show();
    }

    public void Confirm()
    {
        Hide();
        OnConfirmed?.Invoke(TextInput.text);
    }
}
