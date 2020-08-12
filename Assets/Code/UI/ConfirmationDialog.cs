using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : DialogPanelBase
{
    public Text Prompt;
    public Text ConfirmLabel;
    public Text CancelLabel;

    public static ConfirmationDialog Instance;

    Action OnConfirm;
    Action OnCancel;

    public void Init(string prompt, Action onConfirm, Action onCancel, string confirmLabel = "Confirm", string cancelLabel = "Cancel")
    {
        Prompt.text = prompt;
        ConfirmLabel.text = confirmLabel;
        CancelLabel.text = cancelLabel;
        OnConfirm = onConfirm;
        OnCancel = onCancel;
    }

    protected override void PostAwake()
    {
        Instance = this;
    }

    protected override void PostShow()
    {
        
    }

    public void HandleConfirm()
    {
        Hide();
        OnConfirm?.Invoke();
    }

    public void HandleCancel()
    {
        Hide();
        OnCancel?.Invoke();
    }
}
