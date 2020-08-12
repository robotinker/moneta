using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Grave : MonoBehaviour
{
    public TextMeshPro TitleText;
    public TextMeshPro DateText;
    public TextMeshPro DescriptionText;
    public MeshRenderer FlowerRenderer;

    public void Init(string title, string date, string description, Color flowerColor)
    {
        TitleText.text = title;
        DateText.text = date;
        DescriptionText.text = description;
        if (FlowerRenderer != null)
        {
            FlowerRenderer.materials[1].color = flowerColor;
        }
    }
}
