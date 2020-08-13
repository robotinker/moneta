using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuriedProject : MonoBehaviour
{
    public Animator FlowerAnimator;
    public MeshRenderer FlowerRenderer;
    public AudioSource MusicEmitter;
    public AudioSource SFXEmitter;
    public ParticleSystem ParticleSystem;
    ParticleSystemRenderer PSRenderer;

    List<AudioClip> Music = new List<AudioClip>();
    List<AudioClip> SFX = new List<AudioClip>();
    List<Texture2D> Images = new List<Texture2D>();

    bool IsPlaying;

    public void Init(Color flowerColor, string path)
    {
        PSRenderer = ParticleSystem.GetComponent<ParticleSystemRenderer>();

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
            MusicEmitter.Stop();
            IsPlaying = false;
        }
        else
        {
            if (Music.Count > 0)
            {
                MusicEmitter.clip = Utils.GetRandom(Music);
                MusicEmitter.Play();
            }
            StartCoroutine(SFXLoop());
            IsPlaying = true;
        }
        FlowerAnimator.SetBool("Playing", IsPlaying);
    }

    IEnumerator SFXLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 5f));

            if (Images.Count > 0 && !ParticleSystem.isPlaying)
            {
                var texture = Utils.GetRandom(Images);
                PSRenderer.material.mainTexture = texture;
                var maxDimension = Mathf.Max(texture.width, texture.height);
                var mainModule = ParticleSystem.main;
                mainModule.startSizeX = texture.width / (float)maxDimension;
                mainModule.startSizeY = texture.height / (float)maxDimension;
                mainModule.startSizeZ = texture.height / (float)maxDimension;
                ParticleSystem.Play();
            }

            if (SFX.Count > 0 && !SFXEmitter.isPlaying)
            {
                SFXEmitter.clip = Utils.GetRandom(SFX);
                SFXEmitter.Play();
            }
        }
    }
}
