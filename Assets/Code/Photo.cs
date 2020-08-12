using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photo : MonoBehaviour
{
    public MeshRenderer Face;

    public void Init(Texture texture)
    {
        Face.material.mainTexture = texture;
    }
}
