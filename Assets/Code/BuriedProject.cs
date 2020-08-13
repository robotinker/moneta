using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuriedProject : MonoBehaviour
{
    public Animator FlowerAnimator;
    public MeshRenderer FlowerRenderer;
    public AudioSource MusicEmitter;
    public AudioSource SFXEmitter;

    List<AudioClip> Music = new List<AudioClip>();
    List<AudioClip> SFX = new List<AudioClip>();
    List<Texture2D> Images = new List<Texture2D>();

    bool IsPlaying;

    public void Init(Color flowerColor, string path)
    {
        if (FlowerRenderer != null)
        {
            FlowerRenderer.materials[1].color = flowerColor;
        }
        if (!string.IsNullOrEmpty(path))
        {
            Images = Utils.GetTextures(path, 5);
            Utils.GetAudioClips(path, 10, x =>
            {
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
            MusicEmitter.clip = Utils.GetRandom(Music);
            MusicEmitter.Play();
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
            SFXEmitter.clip = Utils.GetRandom(SFX);
            SFXEmitter.Play();
        }
    }
}
