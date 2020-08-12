﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class UnburiedProject : MonoBehaviour
{
    public GameObject PhotoPrefab;
    public Grave Grave;

    public Transform PhotoParent;

    string ProjectPath;
    BonesData Data;

    public void Init(string path)
    {
        ProjectPath = path;

        Grave.Init(Path.GetFileName(path), string.Format("({0} - ???)", Directory.GetCreationTime(ProjectPath).ToString("MMM, yyyy")), "");

        LoadPhotos(path);
        LoadAudio(path);
    }

    void LoadPhotos(string path)
    {
        var photoPresetCount = PhotoParent.childCount;
        var imagePaths = Directory.EnumerateFiles(path, "*.png", SearchOption.AllDirectories).ToList();
        imagePaths.AddRange(Directory.EnumerateFiles(path, "*.jpg", SearchOption.AllDirectories));
        imagePaths.AddRange(Directory.EnumerateFiles(path, "*.jpeg", SearchOption.AllDirectories));
        imagePaths.RemoveAll(s => Path.GetFileName(s).StartsWith("."));

        if (photoPresetCount < imagePaths.Count)
        {
            var pathsToUse = new List<string>();
            for (var i = 0; i < photoPresetCount; i++)
            {
                var pathIndex = UnityEngine.Random.Range(0, imagePaths.Count);
                pathsToUse.Add(imagePaths[pathIndex]);
                imagePaths.RemoveAt(pathIndex);
            }
            imagePaths = pathsToUse;
        }

        for (var i = 0; i < imagePaths.Count; i++)
        {
            var newPhoto = Instantiate(PhotoPrefab, PhotoParent);
            newPhoto.transform.position = PhotoParent.GetChild(i).position;
            newPhoto.transform.rotation = PhotoParent.GetChild(i).rotation;

            var fileData = File.ReadAllBytes(imagePaths[i]);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            newPhoto.GetComponent<Photo>().Init(texture);
        }
    }

    void LoadAudio(string path)
    {

    }

    public void SetTitle()
    {
        GameState.Instance.SetState(GameState.State.UI);
        Data = new BonesData
        {
            Title = Path.GetFileName(ProjectPath),
            Date = string.Format("({0} - {1})", Directory.GetCreationTime(ProjectPath).ToString("MMM, yyyy"), DateTime.Now.ToString("MMM, yyyy")),
            Description = "Lorem Ipsum",
            Position = transform.position,
            Rotation = transform.eulerAngles
        };
        TextDialog.Instance.Init("Project Name", SetTitle);
        TextDialog.Instance.Show();
    }

    void SetTitle(string title)
    {
        Data.Title = title;
        TextDialog.Instance.Init("Would you like to say anything about this project?", SetDescription);
        TextDialog.Instance.Show();
    }

    void SetDescription(string description)
    {
        Data.Description = description;
        File.WriteAllText(Path.Combine(ProjectPath, Path.GetFileName(ProjectPath) + ".bones"), JsonUtility.ToJson(Data, true));

        GraveyardLoader.Instance.AddGrave(Data);
        Destroy(gameObject);

        GameState.Instance.SetState(GameState.State.World);
    }
}
