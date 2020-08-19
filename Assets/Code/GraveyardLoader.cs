using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraveyardLoader : MonoBehaviour
{
    public GameObject GravePrefab;
    public Transform GraveParent;

    public List<Color> FlowerColors;

    public List<GameObject> Tombstones;

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
            LoadingMessage.Instance.Show();
            yield return null;
            yield return null;
            Setup(PlayerPrefs.GetString(GraveyardPathKey));
            LoadingMessage.Instance.Hide();
        }
    }

    public void StartLoadInteraction()
    {
        ConfirmationDialog.Instance.Init("Visit another cemetary?", ChooseGraveyard, null);
    }

    public void ChooseGraveyard()
    {
        GameState.Instance.SetState(GameState.State.UI, this);
        var paths = StandaloneFileBrowser.OpenFolderPanel("Choose An Archive", "", false);
        GameState.Instance.SetState(GameState.State.World, this);
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
                    var newGrave = Utils.CreateAsAlignedChild(GravePrefab, preset);
                    newGrave.GetComponent<Project>().Init(path, data);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Failed initialize grave with file: {0}... {1}", bonesFiles[0], e.Message);
                }
            }
            else if (i < gravePresetCount)
            {
                var newGrave = Utils.CreateAsAlignedChild(GravePrefab, preset);
                newGrave.GetComponent<Project>().Init(path);
            }
            i++;
        }
    }

    IEnumerator Teardown()
    {
        Utils.DestroyChildrenWithComponent<Project>(GraveParent);
        yield return new WaitForEndOfFrame();
    }

    public void StartClearBonesInteraction()
    {
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
