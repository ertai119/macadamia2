using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PropertyManager))]
public class Entity : MonoBehaviour
{
    public bool debugMode = false;
    protected bool pause = false;
    protected PropertyManager propMgr;

    protected virtual void Awake()
    {
        propMgr = GetComponent<PropertyManager>();
    }

    protected virtual void OnDestroy()
    {
    }

    public bool IsPaused()
    {
        return pause;
    }

    public void SetPause(bool flag)
    {
        pause = flag;

        OnPause(flag);
    }

    protected virtual void OnPause(bool flag)
    {
    }
}
