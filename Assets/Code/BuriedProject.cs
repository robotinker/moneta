using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuriedProject : MonoBehaviour
{
    public GameObject PhotoPrefab;
    public Transform PhotoParent;
    public Transform TombstoneParent;
    public Animator FlowerAnimator;
    public MeshRenderer FlowerRenderer;
    public AudioSource MusicEmitter;
    public AudioSource SFXEmitter;
    public ParticleSystem GhostParticleSystem;
    ParticleSystemRenderer GhostRenderer;

    List<AudioClip> Music = new List<AudioClip>();
    List<AudioClip> SFX = new List<AudioClip>();
    List<Texture2D> Images = new List<Texture2D>();

    float StartingMusicVolume;
    Coroutine FadeRoutine;

    public bool IsPlaying { get; private set; }

    public static Action<BuriedProject> OnStartSolo;

    private void Awake()
    {
        StartingMusicVolume = MusicEmitter.volume;
        OnStartSolo += HandleSoloStarted;
    }

    private void OnDestroy()
    {
        OnStartSolo -= HandleSoloStarted;
    }

    void HandleSoloStarted(BuriedProject target)
    {
        if (IsPlaying && target != this)
        {
            TogglePlaying();
        }
    }

    public void Init(string path, Color flowerColor, string photoPath)
    {
        Utils.DestroyChildrenWithComponent<Photo>(PhotoParent);
        if (!string.IsNullOrEmpty(photoPath))
        {
            var newPhoto = Instantiate(PhotoPrefab, PhotoParent);
            newPhoto.transform.position = PhotoParent.position;
            newPhoto.transform.rotation = PhotoParent.rotation;
            var texture = new Texture2D(2, 2);
            try
            {
                var fileData = File.ReadAllBytes(photoPath);
                texture.LoadImage(fileData);
                newPhoto.GetComponent<Photo>().Init(texture);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        GhostRenderer = GhostParticleSystem.GetComponent<ParticleSystemRenderer>();

        if (FlowerRenderer != null)
        {
            FlowerRenderer.materials[1].color = flowerColor;
        }
        if (!string.IsNullOrEmpty(path))
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
    }

    public void TogglePlaying()
    {
        StopAllCoroutines();

        if (IsPlaying)
        {
            FadeRoutine = StartCoroutine(FadeOutMusic());
            IsPlaying = false;
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
            IsPlaying = true;
            OnStartSolo?.Invoke(this);
        }
        FlowerAnimator.SetBool("Playing", IsPlaying);
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
}
