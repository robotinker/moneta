using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeControl : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ConfirmationDialog.Instance.Init("Exit?", () => Application.Quit(), () => ConfirmationDialog.Instance.Hide());
        }
    }
}
