using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Grave : MonoBehaviour
{
    public TextMeshPro TitleText;
    public TextMeshPro DateText;
    public TextMeshPro DescriptionText;

    public void Init(string title, DateTime date, string description)
    {
        TitleText.text = title;
        DescriptionText.text = description;
        DateText.text = string.Format("({0})", date.ToString("MMM, yyyy"));
    }
}
