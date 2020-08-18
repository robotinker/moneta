using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPanelBase : MonoBehaviour
{
    public GameObject Panel;

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        Panel.SetActive(true);
        GameState.Instance.SetState(GameState.State.UI, this);
        PostShow();
    }

    protected virtual void PostShow()
    {

    }

    public void Hide()
    {
        PreHide();
        GameState.Instance.SetState(GameState.State.World, this);
        Panel.SetActive(false);
    }

    protected virtual void PreHide()
    {

    }
}
