using System;
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

        var backupPaths = new List<string>();

        if (photoPresetCount < imagePaths.Count)
        {
            var pathsToUse = new List<string>();
            for (var i = 0; i < photoPresetCount; i++)
            {
                pathsToUse.Add(PopRandom(imagePaths));
            }
            backupPaths.AddRange(imagePaths);
            imagePaths = pathsToUse;
        }

        for (var i = 0; i < imagePaths.Count; i++)
        {
            var parent = PhotoParent.GetChild(i);
            var newPhoto = Instantiate(PhotoPrefab, parent);
            newPhoto.transform.position = parent.position;
            newPhoto.transform.rotation = parent.rotation;

            var texture = new Texture2D(2, 2);
            var fileData = File.ReadAllBytes(imagePaths[i]);
            var failed = false;
            try
            {
                texture.LoadImage(fileData);
            }
            catch
            {
                failed = true;
                while (backupPaths.Count > 0 && failed)
                {
                    var backup = PopRandom(backupPaths);
                    try
                    {
                        fileData = File.ReadAllBytes(backup);
                        texture.LoadImage(fileData);
                        failed = false;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            if (!failed)
            {
                newPhoto.GetComponent<Photo>().Init(texture);
            }
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

    public static string PopRandom(List<string> list)
    {
        var index = UnityEngine.Random.Range(0, list.Count);
        var output = list[index];
        list.RemoveAt(index);
        return output;
    }
}
