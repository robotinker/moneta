using SFB;
using System;
using System.IO;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class GraveyardLoader : MonoBehaviour
{
    public GameObject GravePrefab;
    public GameObject UnburiedProjectPrefab;
    public Transform GraveParent;
    public Transform GravePresetParent;

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
        GameState.Instance.SetState(GameState.State.UI);

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
        GameState.Instance.SetState(GameState.State.World);
    }

    void Setup(string directory)
    {
        var directories = Directory.EnumerateDirectories(directory);
        Debug.Log("Directories: " + string.Join(", ", directories));

        var i = 0;
        var gravePresetCount = GravePresetParent.childCount;
        foreach (var path in directories)
        {
            var bonesFiles = Directory.GetFiles(path, "*.bones");
            if (bonesFiles.Length > 0)
            {
                try
                {
                    var data = (BonesData)JsonUtility.FromJson(File.ReadAllText(bonesFiles[0]), typeof(BonesData));
                    AddGrave(data);
                }
                catch
                {
                    Debug.LogErrorFormat("Failed to parse bones file: {0}", bonesFiles[0]);
                }
            }
            else if (i < gravePresetCount)
            {
                var newBurialPlot = Instantiate(UnburiedProjectPrefab, GraveParent);
                newBurialPlot.transform.position = GravePresetParent.GetChild(i).position;
                newBurialPlot.transform.rotation = GravePresetParent.GetChild(i).rotation;
                newBurialPlot.GetComponent<UnburiedProject>().Init(path, Directory.GetCreationTime(path));
            }
            i++;
        }
    }

    void Teardown()
    {
        for (var i = 0; i < GraveParent.childCount; i++)
        {
            Destroy(GraveParent.GetChild(i).gameObject);
        }
    }

    public void AddGrave(BonesData data)
    {
        var newGrave = Instantiate(GravePrefab);

        newGrave.transform.position = data.Position;
        newGrave.transform.rotation = Quaternion.Euler(data.Rotation);
        newGrave.transform.SetParent(GraveParent);

        newGrave.GetComponent<Grave>().Init(data.Title, data.Date, data.Description);
    }
}
