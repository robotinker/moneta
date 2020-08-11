using SFB;
using System;
using System.IO;
using UnityEngine;

public class GraveyardLoader : MonoBehaviour
{
    public GameObject GravePrefab;

    public static GraveyardLoader Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ConfirmationDialog.Instance.Hide();
    }

    public void StartInteraction()
    {
        GameState.Instance.CurrentState = GameState.State.UI;
        Cursor.lockState = CursorLockMode.None;

        ConfirmationDialog.Instance.Show();
    }

    public void ChooseGraveyard()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Choose Root Directory", "", false);
        if (paths.Length > 0)
        {
            var directoryPath = paths[0];
            var directories = Directory.EnumerateDirectories(directoryPath);
            Debug.Log("Directories: " + string.Join(", ", directories));
        }
        EndInteraction();
    }

    public void EndInteraction()
    {
        ConfirmationDialog.Instance.Hide();
        Cursor.lockState = CursorLockMode.Locked;
        GameState.Instance.CurrentState = GameState.State.World;
    }
}
