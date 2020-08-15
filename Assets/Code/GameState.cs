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

    private void Awake()
    {
        Instance = this;
    }

    public void SetState(State newState)
    {
        CurrentState = newState;

        switch (newState)
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
