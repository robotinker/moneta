using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
    public Transform Parent;

    public void CreatePrefab(GameObject prefab)
    {
        Instantiate(prefab, Parent);
    }
}
