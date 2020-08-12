using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Grave : MonoBehaviour
{
    public TextMeshPro TitleText;
    public TextMeshPro DateText;
    public TextMeshPro DescriptionText;

    public void Init(string title, string date, string description)
    {
        TitleText.text = title;
        DateText.text = date;
        DescriptionText.text = description;
    }
}
