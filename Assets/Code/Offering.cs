using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offering : MonoBehaviour
{
    bool IsPlaying;
    Animator Animator;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Toggle()
    {
        IsPlaying = !IsPlaying;
        Animator.SetBool("Playing", IsPlaying);
    }
}
