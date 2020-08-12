using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPanelBase : MonoBehaviour
{
    public GameObject Panel;

    protected void Awake()
    {
        Hide();
        PostAwake();
    }

    protected virtual void PostAwake()
    {

    }

    public void Show()
    {
        Panel.SetActive(true);
        PostShow();
    }

    protected virtual void PostShow()
    {

    }

    public void Hide()
    {
        PreHide();
        Panel.SetActive(false);
    }

    protected virtual void PreHide()
    {

    }
}
