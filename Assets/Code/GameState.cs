using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum State
    {
        World,
        UI
    }

    public State CurrentState { get; private set; } = State.World;

    public static GameState Instance;

    List<object> UIVotes = new List<object>();

    private void Awake()
    {
        Instance = this;
    }

    public void SetState(State newState, object owner)
    {
        if (newState == State.UI)
        {
            if (!UIVotes.Contains(owner))
            {
                UIVotes.Add(owner);
            }
        }
        else
        {
            UIVotes.Remove(owner);
        }
        UpdateState();
    }

    public void ClearState(object owner)
    {
        UIVotes.Remove(owner);
        UpdateState();
    }

    void UpdateState()
    {
        CurrentState = UIVotes.Count > 0 ? State.UI : State.World;
        switch (CurrentState)
        {
            case State.World:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case State.UI:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }
}
