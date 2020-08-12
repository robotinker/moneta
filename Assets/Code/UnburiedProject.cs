using System;
using System.IO;
using UnityEngine;

public class UnburiedProject : MonoBehaviour
{
    string ProjectPath;

    public void Init(string path, DateTime creationTime)
    {
        ProjectPath = path;
    }

    public void Bury()
    {
        var data = new BonesData
        {
            Title = Path.GetFileName(ProjectPath),
            Date = string.Format("({0} - {1})", Directory.GetCreationTime(ProjectPath).ToString("MMM, yyyy"), DateTime.Now.ToString("MMM, yyyy")),
            Description = "Lorem Ipsum",
            Position = transform.position,
            Rotation = transform.eulerAngles
        };
        File.WriteAllText(Path.Combine(ProjectPath, Path.GetFileName(ProjectPath) + ".bones"), JsonUtility.ToJson(data));

        GraveyardLoader.Instance.AddGrave(data);
        Destroy(gameObject);
    }
}
