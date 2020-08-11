using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float WalkSpeed = 3f;
    public float RunSpeed = 6f;
    CharacterController Controller;
    void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (GameState.Instance.CurrentState != GameState.State.World)
            return;

        var moveDir = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        Controller.SimpleMove(moveDir * (Input.GetKey(KeyCode.LeftShift) ? RunSpeed : WalkSpeed));        
    }
}
