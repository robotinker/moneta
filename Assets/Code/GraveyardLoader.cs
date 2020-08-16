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

    public List<Color> FlowerColors;

    public static GraveyardLoader Instance;

    const string GraveyardPathKey = "GraveyardPath";
    string GraveyardPath = "";

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return Teardown();
        if (PlayerPrefs.HasKey(GraveyardPathKey))
        {
            Setup(PlayerPrefs.GetString(GraveyardPathKey));
        }
    }

    public void StartLoadInteraction()
    {
        ConfirmationDialog.Instance.Init("Visit another cemetary?", ChooseGraveyard, null);
    }

    public void ChooseGraveyard()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Choose An Archive", "", false);
        if (paths.Length > 0 && (paths[0] != GraveyardPath))
        {
            var directoryPath = paths[0];

            StartCoroutine(LoadGraveyardRoutine(paths[0]));
        }
    }

    IEnumerator LoadGraveyardRoutine(string path)
    {
        PlayerPrefs.SetString(GraveyardPathKey, path);
        LoadingMessage.Instance.Show();
        yield return null;
        yield return null;

        yield return Teardown();
        Setup(path);
        LoadingMessage.Instance.Hide();
    }

    void Setup(string directory)
    {
        GraveyardPath = directory;
        var directories = Directory.EnumerateDirectories(directory);

        var i = 0;
        var gravePresetCount = GraveParent.childCount;
        foreach (var path in directories)
        {
            if (i >= gravePresetCount)
                break;

            var preset = GraveParent.GetChild(i);

            var bonesFiles = Directory.GetFiles(path, "*.bones");
            if (bonesFiles.Length > 0)
            {
                try
                {
                    var data = (BonesData)JsonUtility.FromJson(File.ReadAllText(bonesFiles[0]), typeof(BonesData));
                    AddGrave(data, path, preset.position, preset.rotation);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Failed initialize grave with file: {0}... {1}", bonesFiles[0], e.Message);
                }
            }
            else if (i < gravePresetCount)
            {
                var newBurialPlot = Instantiate(UnburiedProjectPrefab, GraveParent);
                newBurialPlot.transform.position = preset.position;
                newBurialPlot.transform.rotation = preset.rotation;
                newBurialPlot.GetComponent<UnburiedProject>().Init(path);
            }
            i++;
        }
    }

    IEnumerator Teardown()
    {
        Utils.DestroyChildrenWithComponent<BuriedProject>(GraveParent);
        Utils.DestroyChildrenWithComponent<UnburiedProject>(GraveParent);
        yield return new WaitForEndOfFrame();
    }

    public void AddGrave(BonesData data, string path, Vector3 position, Quaternion rotation)
    {
        var newGrave = Instantiate(GravePrefab);

        newGrave.transform.position = position;
        newGrave.transform.rotation = rotation;
        newGrave.transform.SetParent(GraveParent);

        newGrave.GetComponentInChildren<Grave>().Init(data.Title, data.Date, data.Description);
        newGrave.GetComponent<BuriedProject>().Init(path, data.FlowerColor, data.Photo);
    }

    public void StartClearBonesInteraction()
    {
        GameState.Instance.SetState(GameState.State.UI);

        ConfirmationDialog.Instance.Init("Dig up all graves?", DestroyAllBonesFiles, null);
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
