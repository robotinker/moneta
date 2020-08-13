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
    }

    void LoadPhotos(string path)
    {
        var textures = Utils.GetTextures(path, PhotoParent.childCount);
        
        for (var i = 0; i < textures.Count; i++)
        {
            var parent = PhotoParent.GetChild(i);
            var newPhoto = Instantiate(PhotoPrefab, parent);
            newPhoto.transform.position = parent.position;
            newPhoto.transform.rotation = parent.rotation;
            newPhoto.GetComponent<Photo>().Init(textures[i]);
        }
    }

    public void StartBurialInteraction()
    {
        GameState.Instance.SetState(GameState.State.UI);
        ConfirmationDialog.Instance.Init("Bury this project?", SetTitle, EndInteraction);
        ConfirmationDialog.Instance.Show();
    }

    public void SetTitle()
    {
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
        Data.FlowerColor = Utils.GetRandom(GraveyardLoader.Instance.FlowerColors);
        File.WriteAllText(Path.Combine(ProjectPath, Path.GetFileName(ProjectPath) + ".bones"), JsonUtility.ToJson(Data, true));

        GraveyardLoader.Instance.AddGrave(Data, ProjectPath);
        Destroy(gameObject);

        EndInteraction();
    }

    void EndInteraction()
    {
        GameState.Instance.SetState(GameState.State.World);
    }
}
