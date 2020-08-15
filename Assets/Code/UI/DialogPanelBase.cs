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
        GameState.Instance.SetState(GameState.State.UI);
        PostShow();
    }

    protected virtual void PostShow()
    {

    }

    public void Hide()
    {
        PreHide();
        GameState.Instance.SetState(GameState.State.World);
        Panel.SetActive(false);
    }

    protected virtual void PreHide()
    {

    }
}
