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

    virtual public void OnEnter()
    {
        if (target == null)
            return;

        PropertyManager propMgr = target.GetComponent<PropertyManager>();
        if (propMgr == null)
            return;

        float curValue = propMgr.AddValue(propType, value);

        if (propType == ePropertyType.SCALE && curValue != 0f)
        {
            target.transform.localScale = CommonUtil.BaseScale() * curValue;
        }
    }

    virtual public void OnUpdate()
    {
        duration -= Time.deltaTime;
    }

    virtual public void OnExit()
    {
        if (target == null)
            return;

        PropertyManager propMgr = target.GetComponent<PropertyManager>();
        if (propMgr == null)
            return;

        float curValue = propMgr.AddValue(propType, -value);

        if (propType == ePropertyType.SCALE && curValue != 0f)
        {
            target.transform.localScale = CommonUtil.BaseScale() * curValue;
        }
    }

    public bool IsEnd() { return duration < 0f; }
}