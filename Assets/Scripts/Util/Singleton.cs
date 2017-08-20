using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour
    where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T> ();
                if (instance == null) 
                {
                    GameObject obj = new GameObject ();
                    obj.hideFlags = HideFlags.DontSave;
                    instance = obj.AddComponent<T> ();
                    obj.name = obj.GetComponent<T>().GetType().Name + " (Singleton)";
                }
            }
            return instance;
        }
    }
}

public class MonoBehaviourSingletonPersistent<T> : MonoBehaviour
    where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake ()
    {
        if (Instance == null) {
            Instance = this as T;
            DontDestroyOnLoad (this);
        } else {
            Destroy (gameObject);
        }
    }
}