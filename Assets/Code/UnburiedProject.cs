using System;
using System.IO;
using UnityEngine;

public class UnburiedProject : MonoBehaviour
{
    public Grave Grave;

    string ProjectPath;
    BonesData Data;

    public void Init(string path, DateTime creationTime)
    {
        ProjectPath = path;

        Grave.Init(Path.GetFileName(path), string.Format("({0} - ???)", Directory.GetCreationTime(ProjectPath).ToString("MMM, yyyy")), "");
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
