using SFB;
using System;
using System.IO;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class GraveyardLoader : MonoBehaviour
{
    public GameObject GravePrefab;
    public Transform GraveParent;

    public static GraveyardLoader Instance;

    const string GraveyardPathKey = "GraveyardPath";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ConfirmationDialog.Instance.Hide();

        if (PlayerPrefs.HasKey(GraveyardPathKey))
        {
            Setup(PlayerPrefs.GetString(GraveyardPathKey));
        }
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
            if (!PlayerPrefs.HasKey(GraveyardPathKey) || directoryPath != PlayerPrefs.GetString(GraveyardPathKey))
            {
                PlayerPrefs.SetString(GraveyardPathKey, directoryPath);
                Teardown();
                Setup(directoryPath);
            }
        }
        EndInteraction();
    }

    public void EndInteraction()
    {
        ConfirmationDialog.Instance.Hide();
        Cursor.lockState = CursorLockMode.Locked;
        GameState.Instance.CurrentState = GameState.State.World;
    }

    void Setup(string directory)
    {
        var directories = Directory.EnumerateDirectories(directory);
        Debug.Log("Directories: " + string.Join(", ", directories));

        foreach (var path in directories)
        {
            var newGrave = Instantiate(GravePrefab, GraveParent);
            newGrave.transform.position = Vector3.zero + Vector3.right * UnityRandom.Range(-5f, 5f) + Vector3.forward * UnityRandom.Range(-5f, 5f);
            newGrave.GetComponent<Grave>().Init(Path.GetFileName(path), Directory.GetCreationTime(path), "Lorem ipsum si cut dolor meus.");
        }
    }

    void Teardown()
    {
        for (var i = 0; i < GraveParent.childCount; i++)
        {
            Destroy(GraveParent.GetChild(i));
        }
    }
}
