using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentsManager : MonoBehaviourSingleton<ContentsManager>
{
    ContentsLibrary library;
	// Use this for initialization
	void Start () {
		
	}

    public void InitLibrary()
    {
        GameObject contentsLibObj = Resources.Load("ContentsLibrary") as GameObject;
        library = contentsLibObj.GetComponent<ContentsLibrary>();
        if (library)
        {
            library.InitLibrary();
        }
    }

    public GameObject GetPrefabFromName(string name)
    {
        if (library == null)
        {
            Debug.LogError("cms library null");
            return null;
        }

        return library.GetPrefabFromName(name);
    }
}
