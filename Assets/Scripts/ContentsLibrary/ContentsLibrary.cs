using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentsLibrary : MonoBehaviour
{
    public Group[] groups;

    Dictionary<string, GameObject[]> groupDictionary = new Dictionary<string, GameObject[]>();

    void Awake()
    {
        InitLibrary();
    }

    public void InitLibrary()
    {
        groupDictionary.Clear();

        foreach (Group group in groups)
        {
            groupDictionary.Add(group.groupID, group.group);
        }
    }

    public GameObject GetPrefabFromName(string name)
    {
        if (groupDictionary.ContainsKey(name))
        {
            GameObject[] prefabs = groupDictionary[name];
            if (prefabs.Length == 0)
                return null;
            
            return prefabs[Random.Range (0, prefabs.Length)];
        }

        return null;
    }

    [System.Serializable]
    public class Group
    {
        public string groupID;
        public GameObject[] group;
    }
}
