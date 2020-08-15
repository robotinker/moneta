using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingMessage : DialogPanelBase
{
    public static LoadingMessage Instance;

    void Awake()
    {
        Instance = this;
    }
}
