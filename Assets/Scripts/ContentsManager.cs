using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentsManager : MonoBehaviourSingleton<ContentsManager>
{
    public ContentsLibrary gamelib;
    public SoundLibrary soundLib;

    public void Init()
    {
        LoadLibrary<ContentsLibrary>(ref gamelib, "ContentsLibrary");
        LoadLibrary<SoundLibrary>(ref soundLib, "SoundLibrary");
        gamelib.InitLibrary();
        soundLib.InitLibrary();
    }

    void LoadLibrary<T>(ref T t, string libName)
    {
        GameObject obj = Resources.Load(libName) as GameObject;
        t = obj.GetComponent<T>();
    }

    public GameObject GetPrefabFromName(string name)
    {
        if (gamelib == null)
        {
            Debug.LogError("cms library null");
            return null;
        }

        return gamelib.GetPrefabFromName(name);
    }

    public AudioClip GetClipFromName(string name)
    {
        if (soundLib == null)
        {
            Debug.LogError("sound lib not loaded");
            return null;
        }

        return soundLib.GetClipFromName(name);
    }
}
