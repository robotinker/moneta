using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SecondaryInteraction : MonoBehaviour
{
    public UnityEvent OnActivate;
    public void Activate()
    {
        OnActivate.Invoke();
    }
}
