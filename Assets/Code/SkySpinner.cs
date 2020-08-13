using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySpinner : MonoBehaviour
{
    public Vector3 Velocity;

    void Start()
    {
        GetComponent<Rigidbody>().angularVelocity = Velocity;
    }
}
