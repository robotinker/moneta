using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static T GetRandom<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static T PopRandom<T>(List<T> list)
    {
        var index = Random.Range(0, list.Count);
        var output = list[index];
        list.RemoveAt(index);
        return output;
    }
}
