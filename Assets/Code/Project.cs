using SFB;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Project : MonoBehaviour
{
    public Transform TombstonePosition;

    public Transform PhotoParent;
    public GameObject PhotoPrefab;
    public Transform PostBurialPhotoPosition;

    public MeshRenderer FlowerRenderer;
    public Animator FlowerAnimator;

    public AudioSource MusicEmitter;
    public AudioSource SFXEmitter;
    public ParticleSystem GhostParticleSystem;
    ParticleSystemRenderer GhostRenderer;

    List<AudioClip> Music = new List<AudioClip>();
    List<AudioClip> SFX = new List<AudioClip>();
    List<Texture2D> Images = new List<Texture2D>();

    Animator Animator;
    string ProjectPath;
    BonesData Data;
    bool IsBurying;
    bool IsBuried;
    float StartingMusicVolume;
    Coroutine FadeRoutine;

    public bool IsPlayingSolo { get; private set; }

    public static Action<Project> OnStartSolo;

    void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        StartingMusicVolume = MusicEmitter.volume;
        GhostRenderer = GhostParticleSystem.GetComponent<ParticleSystemRenderer>();

        OnStartSolo += HandleSoloStarted;
    }

    void OnDestroy()
    {
        OnStartSolo -= HandleSoloStarted;
    }

    public void Init(string path, BonesData data = null)
    {
        IsBuried = data != null;

        Animator.SetBool("IsBuried", IsBuried);
        if (IsBuried)
        {
            Animator.SetTrigger("Force");
        }

        PreloadAssets(path);

        if (IsBuried)
        {
            SetupBuriedProject(path, data);
        }
        else
        {
            SetupUnburiedProject(path);
        }
    }

    public void HandlePrimaryInteraction()
    {
        if (IsBuried)
        {
            ToggleSolo();
        }
        else
        {
            StartBurialInteraction();
        }
    }

    public void HandleSecondaryInteraction()
    {
        if (IsBuried)
        {

        }
        else
        {
            ResetPhotos();
        }
    }

    void PreloadAssets(string path)
    {
        Images = Utils.GetTextures(path, 15);
        Debug.LogFormat("Found {0} textures for {1}", Images.Count, Path.GetFileName(path));

        Utils.GetAudioClips(path, 15, x =>
        {
            Debug.LogFormat("Received {0} clips for {1}", x.Count, Path.GetFileName(path));
            foreach (var clip in x)
            {
                if (clip.length > 3)
                {
                    Music.Add(clip);
                }
                else
                {
                    SFX.Add(clip);
                }
            }
        });
    }

    void SetupUnburiedProject(string path)
    {
        ProjectPath = path;

        SetupTombstone(path, new BonesData
        {
            Title = Path.GetFileName(path),
            Description = "",
            Date = string.Format("({0} - ???)", Directory.GetCreationTime(ProjectPath).ToString("MMM, yyyy"))
        });
        ResetPhotos();
    }

    void SetupBuriedProject(string path, BonesData data)
    {
        IsBuried = true;
        SetupTombstone(path, data);
        ClearPhotos();
        SetupBuriedProjectPhoto(data);
        SetupBuriedProjectOfferings(data);
    }

    void SetupTombstone(string path, BonesData data)
    {
        Utils.DestroyChildrenWithComponent<Grave>(TombstonePosition);
        var newTombstone = Utils.CreateAsAlignedChild(Utils.GetRandom(GraveyardLoader.Instance.Tombstones, path.GetHashCode()), TombstonePosition);
        newTombstone.GetComponentInChildren<Grave>().Init(data.Title, data.Date, data.Description);
    }

    void SetupBuriedProjectPhoto(BonesData data)
    {
        if (string.IsNullOrEmpty(data.Photo))
            return;

        var newPhoto = Utils.CreateAsAlignedChild(PhotoPrefab, PostBurialPhotoPosition);
        Utils.TryLoadTextureFromPathToPhoto(data.Photo, newPhoto.GetComponent<Photo>());
    }

    void SetupBuriedProjectOfferings(BonesData data)
    {
        if (FlowerRenderer != null)
        {
            FlowerRenderer.materials[1].color = data.FlowerColor;
        }
    }

    void HandleSoloStarted(Project target)
    {
        if (IsBuried && IsPlayingSolo && target != this)
        {
            ToggleSolo();
        }
    }

    public void ToggleSolo()
    {
        if (!IsBuried)
            return;

        StopAllCoroutines();

        if (IsPlayingSolo)
        {
            FadeRoutine = StartCoroutine(FadeOutMusic());
            IsPlayingSolo = false;
        }
        else
        {
            if (FadeRoutine != null)
            {
                StopCoroutine(FadeRoutine);
                MusicEmitter.volume = StartingMusicVolume;
                FadeRoutine = null;
            }
            if (Music.Count > 0)
            {
                MusicEmitter.clip = Utils.GetRandom(Music);
                MusicEmitter.Play();
            }
            StartCoroutine(SFXLoop());
            IsPlayingSolo = true;
            OnStartSolo?.Invoke(this);
        }
        FlowerAnimator.SetBool("Playing", IsPlayingSolo);
    }

    IEnumerator FadeOutMusic()
    {
        var fadeTime = 1f;
        var timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            MusicEmitter.volume = StartingMusicVolume * (1f - timer / fadeTime);
            yield return null;
        }
        MusicEmitter.Stop();
        MusicEmitter.volume = StartingMusicVolume;
        FadeRoutine = null;
    }

    IEnumerator SFXLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f));

            if (Images.Count > 0 && !GhostParticleSystem.isPlaying)
            {
                var texture = Utils.GetRandom(Images);
                GhostRenderer.material.mainTexture = texture;
                var maxDimension = Mathf.Max(texture.width, texture.height);
                var mainModule = GhostParticleSystem.main;
                mainModule.startSizeX = texture.width / (float)maxDimension;
                mainModule.startSizeY = texture.height / (float)maxDimension;
                mainModule.startSizeZ = texture.height / (float)maxDimension;
                GhostParticleSystem.Play();
            }

            if (SFX.Count > 0 && !SFXEmitter.isPlaying)
            {
                SFXEmitter.clip = Utils.GetRandom(SFX);
                SFXEmitter.Play();
            }
        }
    }

    void ResetPhotos()
    {

        ClearPhotos();
        LoadPhotos(ProjectPath);
    }

    void ClearPhotos()
    {
        Utils.DestroyChildrenWithComponent<Photo>(PhotoParent);
        Utils.DestroyChildrenWithComponent<Photo>(PostBurialPhotoPosition);
    }

    void LoadPhotos(string path)
    {
        if (IsBuried)
            return;

        var textures = Utils.GetTextures(path, PhotoParent.childCount);

        for (var i = 0; i < textures.Count; i++)
        {
            var parent = PhotoParent.GetChild(i);
            var newPhoto = Utils.CreateAsAlignedChild(PhotoPrefab, parent);
            newPhoto.GetComponent<Photo>().Init(textures[i]);
        }
    }

    void StartBurialInteraction()
    {
        if (IsBurying)
            return;

        ConfirmationDialog.Instance.Init("Bury this project?", SetTitle, null);
    }

    void SetTitle()
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
                GameState.Instance.SetState(GameState.State.UI, this);
                var paths = StandaloneFileBrowser.OpenFilePanel("Choose A Photo", ProjectPath, new[] { new ExtensionFilter("images", "jpg", "jpeg", "png") }, false);
                GameState.Instance.SetState(GameState.State.World, this);
                if (paths.Length > 0)
                {
                    SetPostBurialPhoto(paths[0]);
                }
                else
                {
                    SetPostBurialPhoto("");
                }
            },
            () =>
            {
                SetPostBurialPhoto("");
            }, "Yes", "No");
    }

    void SetPostBurialPhoto(string photoPath)
    {
        Data.Photo = photoPath;

        File.WriteAllText(Path.Combine(ProjectPath, Path.GetFileName(ProjectPath) + ".bones"), JsonUtility.ToJson(Data, true));

        StartCoroutine(BurialRoutine());
    }

    IEnumerator BurialRoutine()
    {
        Animator.SetBool("IsBuried", true);
        IsBurying = true;

        yield return new WaitForSeconds(4f);

        IsBuried = true;
        TombstonePosition.GetComponentInChildren<Grave>().Init(Data.Title, Data.Date, Data.Description);

    }
}
