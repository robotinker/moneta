using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingMessage : DialogPanelBase
{
    public static LoadingMessage Instance;

    protected override void PostAwake()
    {
        Instance = this;
    }
}
