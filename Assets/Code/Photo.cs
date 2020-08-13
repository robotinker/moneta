using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photo : MonoBehaviour
{
    public MeshRenderer Face;

    public void Init(Texture texture)
    {
        Face.material.mainTexture = texture;

        var maxDimension = Mathf.Max(texture.width, texture.height);
        Face.transform.localScale = new Vector3(texture.width / (float)maxDimension, texture.height / (float)maxDimension, 1f) * Face.transform.localScale.x;
    }
}
