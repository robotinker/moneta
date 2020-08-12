using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationDialog : DialogPanelBase
{
    public static ConfirmationDialog Instance;

    protected override void PostAwake()
    {
        Instance = this;
    }
}
