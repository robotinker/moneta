using SFB;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class UnburiedProject : MonoBehaviour
{
    public Transform BurialEffectLocation;
    public GameObject BurialEffectPrefab;
    public GameObject PhotoPrefab;
    public Grave Grave;

    public Transform PhotoParent;
    public Animator BurialAnimator;

    string ProjectPath;
    BonesData Data;
    bool IsBurying;

    public void Init(string path)
    {
        ProjectPath = path;

        Grave.Init(Path.GetFileName(path), string.Format("({0} - ???)", Directory.GetCreationTime(ProjectPath).ToString("MMM, yyyy")), "");

        ResetPhotos();
    }

    public void ResetPhotos()
    {
        ClearPhotos();
        LoadPhotos(ProjectPath);
    }

    void ClearPhotos()
    {
        Utils.DestroyChildrenWithComponent<Photo>(PhotoParent);
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
        if (IsBurying)
            return;

        ConfirmationDialog.Instance.Init("Bury this project?", SetTitle, null);
    }

    public void SetTitle()
    {
        Data = new BonesData
        {
            Title = Path.GetFileName(ProjectPath),
            Date = string.Format("({0} - {1})", Directory.GetCreationTime(ProjectPath).ToString("MMM, yyyy"), DateTime.Now.ToString("MMM, yyyy")),
            Description = "Lorem Ipsum"
        };
        TextDialog.Instance.Init("Project Name", SetTitle);
    }

    void SetTitle(string title)
    {
        Data.Title = title;
        TextDialog.Instance.Init("Would you like to say anything about this project?", SetDescription);
    }

    void SetDescription(string description)
    {
        Data.Description = description;
        Data.FlowerColor = Utils.GetRandom(GraveyardLoader.Instance.FlowerColors);
        ConfirmationDialog.Instance.Init("Is there a photo you'd like to keep?",
            () =>
            {
                var paths = StandaloneFileBrowser.OpenFilePanel("Choose A Photo", ProjectPath, new[] {new ExtensionFilter("images", "jpg", "jpeg", "png")}, false);
                if (paths.Length > 0)
                {
                    SetPhoto(paths[0]);
                }
                else
                {
                    SetPhoto("");
                }
            },
            () =>
            {
                SetPhoto("");
            }, "Yes", "No");
    }

    void SetPhoto(string photoPath)
    {
        Data.Photo = photoPath;

        File.WriteAllText(Path.Combine(ProjectPath, Path.GetFileName(ProjectPath) + ".bones"), JsonUtility.ToJson(Data, true));

        StartCoroutine(BurialRoutine());
    }

    IEnumerator BurialRoutine()
    {
        BurialAnimator.SetTrigger("Bury");
        IsBurying = true;

        yield return new WaitForSeconds(3.5f);
        Instantiate(BurialEffectPrefab, BurialEffectLocation.position, BurialEffectLocation.rotation);
        yield return new WaitForSeconds(0.5f);
        GraveyardLoader.Instance.AddGrave(Data, ProjectPath, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
