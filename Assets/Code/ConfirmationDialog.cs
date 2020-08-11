using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationDialog : MonoBehaviour
{
    public GameObject DialogPanel;

    public Action OnHide;

    public static ConfirmationDialog Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Show()
    {
        DialogPanel.SetActive(true);
    }

    public void Hide()
    {
        DialogPanel.SetActive(false);
        OnHide?.Invoke();
    }
}
