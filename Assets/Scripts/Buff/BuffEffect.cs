using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffEffect
{
    public ePropertyType propType;
    public float value;
    public float duration;

    public GameObject target { get; set; }

    public void OnEnter()
    {
        if (target == null)
            return;

        PropertyManager propMgr = target.GetComponent<PropertyManager>();
        if (propMgr == null)
            return;

        propMgr.AddValue(propType, value);
    }

    public void OnUpdate()
    {
        duration -= Time.deltaTime;
    }

    public void OnExit()
    {
        if (target == null)
            return;

        PropertyManager propMgr = target.GetComponent<PropertyManager>();
        if (propMgr == null)
            return;

        propMgr.AddValue(propType, -value);
    }

    public bool IsEnd() { return duration < 0f; }
}