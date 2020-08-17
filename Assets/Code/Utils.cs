using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Utils
{
    public static T GetRandom<T>(List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T PopRandom<T>(List<T> list)
    {
        var index = UnityEngine.Random.Range(0, list.Count);
        var output = list[index];
        list.RemoveAt(index);
        return output;
    }

    public static GameObject CreateAsAlignedChild(GameObject prefab, Transform parent)
    {
        var output = GameObject.Instantiate(prefab, parent.position, parent.rotation);
        output.transform.SetParent(parent);
        output.transform.localScale = Vector3.one;
        return output;
    }

    public static bool TryLoadTextureFromPathToPhoto(string path, Photo photo)
    {
        var texture = new Texture2D(2, 2);
        try
        {
            var fileData = File.ReadAllBytes(path);
            texture.LoadImage(fileData);
            photo.Init(texture);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public static List<Texture2D> GetTextures(string path, int max)
    {
        var output = new List<Texture2D>();
        var imagePaths = Directory.EnumerateFiles(path, "*.png", SearchOption.AllDirectories).ToList();
        imagePaths.AddRange(Directory.EnumerateFiles(path, "*.jpg", SearchOption.AllDirectories));
        imagePaths.AddRange(Directory.EnumerateFiles(path, "*.jpeg", SearchOption.AllDirectories));
        imagePaths.RemoveAll(s => Path.GetFileName(s).StartsWith("."));
        imagePaths.RemoveAll(s => s.Contains("Editor"));
        imagePaths.RemoveAll(s => s.Contains("Example"));

        Debug.Log(string.Join("\n", imagePaths));

        var backupPaths = new List<string>();

        if (max < imagePaths.Count)
        {
            var pathsToUse = new List<string>();
            for (var i = 0; i < max; i++)
            {
                pathsToUse.Add(PopRandom(imagePaths));
            }
            backupPaths.AddRange(imagePaths);
            imagePaths = pathsToUse;
        }

        for (var i = 0; i < imagePaths.Count; i++)
        {
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
                output.Add(texture);
            }
        }
        return output;
    }

    public static void GetAudioClips(string path, int max, Action<List<AudioClip>> callback)
    {
        // TODO: Supporting MP3s is hard due to patent concerns.
        // See this thread: https://answers.unity.com/questions/1532988/unitywebrequestmultimedia-to-get-audioclip-from-ap.html
        var audioPaths = Directory.EnumerateFiles(path, "*.wav", SearchOption.AllDirectories).ToList();
        audioPaths.AddRange(Directory.EnumerateFiles(path, "*.ogg", SearchOption.AllDirectories));
        audioPaths.RemoveAll(s => Path.GetFileName(s).StartsWith("."));

        var backupPaths = new List<string>();

        if (max < audioPaths.Count)
        {
            var pathsToUse = new List<string>();
            for (var i = 0; i < max; i++)
            {
                pathsToUse.Add(PopRandom(audioPaths));
            }
            backupPaths.AddRange(audioPaths);
            audioPaths = pathsToUse;
        }

        GraveyardLoader.Instance.StartCoroutine(GetAudioClipsRoutine(audioPaths, backupPaths, callback));
    }

    static IEnumerator GetAudioClipsRoutine(List<string> paths, List<string> backupPaths, Action<List<AudioClip>> callback)
    {
        var output = new List<AudioClip>();

        foreach (var path in paths)
        {
            var audioType = AudioType.UNKNOWN;
            switch (Path.GetExtension(Path.GetFileName(path)))
            {
                case ".ogg":
                    audioType = AudioType.OGGVORBIS;
                    break;
                case ".wav":
                    audioType = AudioType.WAV;
                    break;
            }

            if (audioType == AudioType.UNKNOWN)
                continue;

            var url = "file://" + path;
            var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            yield return webRequest.SendWebRequest();
            output.Add(DownloadHandlerAudioClip.GetContent(webRequest));
        }

        callback?.Invoke(output);
    }

    public static void DestroyChildrenWithComponent<T>(Transform parent) where T : MonoBehaviour
    {
        foreach (var target in parent.GetComponentsInChildren<T>())
        {
            UnityEngine.Object.Destroy(target.gameObject);
        }
    }
}
