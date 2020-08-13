using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraveyardLoader : MonoBehaviour
{
    public GameObject GravePrefab;
    public GameObject UnburiedProjectPrefab;
    public Transform GraveParent;
    public Transform GravePresetParent;

    public List<Color> FlowerColors;

    public static GraveyardLoader Instance;

    const string GraveyardPathKey = "GraveyardPath";
    string GraveyardPath = "";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(GraveyardPathKey))
        {
            Setup(PlayerPrefs.GetString(GraveyardPathKey));
        }
    }

    public void StartLoadInteraction()
    {
        GameState.Instance.SetState(GameState.State.UI);

        ConfirmationDialog.Instance.Init("Visit another cemetary?", ChooseGraveyard, EndInteraction);
        ConfirmationDialog.Instance.Show();
    }

    public void ChooseGraveyard()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Choose Root Directory", "", false);
        if (paths.Length > 0 && (paths[0] != GraveyardPath))
        {
            var directoryPath = paths[0];

            StartCoroutine(LoadGraveyardRoutine(paths[0]));
        }
        else
        {
            EndInteraction();
        }
    }

    IEnumerator LoadGraveyardRoutine(string path)
    {
        PlayerPrefs.SetString(GraveyardPathKey, path);
        LoadingMessage.Instance.Show();
        yield return null;
        yield return null;

        Teardown();
        Setup(path);
        LoadingMessage.Instance.Hide();
        EndInteraction();
    }

    public void EndInteraction()
    {
        GameState.Instance.SetState(GameState.State.World);
    }

    void Setup(string directory)
    {
        GraveyardPath = directory;
        var directories = Directory.EnumerateDirectories(directory);

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
                    AddGrave(data, path);
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
                newBurialPlot.GetComponent<UnburiedProject>().Init(path);
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

    public void AddGrave(BonesData data, string path)
    {
        var newGrave = Instantiate(GravePrefab);

        newGrave.transform.position = data.Position;
        newGrave.transform.rotation = Quaternion.Euler(data.Rotation);
        newGrave.transform.SetParent(GraveParent);

        newGrave.GetComponent<Grave>().Init(data.Title, data.Date, data.Description);
        newGrave.GetComponent<BuriedProject>().Init(data.FlowerColor, path);
    }

    public void StartClearBonesInteraction()
    {
        GameState.Instance.SetState(GameState.State.UI);

        ConfirmationDialog.Instance.Init("Dig up all graves?", DestroyAllBonesFiles, EndInteraction);
        ConfirmationDialog.Instance.Show();
    }

    public void DestroyAllBonesFiles()
    {
        if (string.IsNullOrEmpty(GraveyardPath))
            return;

        var directories = Directory.EnumerateDirectories(GraveyardPath);

        foreach (var path in directories)
        {
            var bonesFiles = Directory.GetFiles(path, "*.bones");
            foreach (var file in bonesFiles)
            {
                File.Delete(file);
            }
        }

        StartCoroutine(LoadGraveyardRoutine(GraveyardPath));
    }
}
