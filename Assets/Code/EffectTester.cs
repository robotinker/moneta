using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTester : MonoBehaviour
{
    public List<GameObject> Effects;

    int NextEffectIndex;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
            }
            Instantiate(Effects[NextEffectIndex], transform);
            Debug.Log("Effect: " + Effects[NextEffectIndex].name);
            NextEffectIndex = (NextEffectIndex + 1) % Effects.Count;
        }
    }
}
