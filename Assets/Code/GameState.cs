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

    public State CurrentState = State.World;

    public static GameState Instance;

    private void Awake()
    {
        Instance = this;
    }
}
