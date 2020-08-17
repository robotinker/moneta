using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
    public List<Transform> Parents;

    int ParentIndex;

    public void CreatePrefab(GameObject prefab)
    {
        Utils.CreateAsAlignedChild(prefab, Parents[ParentIndex]);
    }

    public void SetParentIndex(int i)
    {
        ParentIndex = Mathf.Clamp(i, 0, Parents.Count - 1);
    }
}
