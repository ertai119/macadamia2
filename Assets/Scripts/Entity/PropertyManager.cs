using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePropertyType
{
    INVALID = 0,
    SPEED,
    SCALE,
}

public class PropertyManager : MonoBehaviour
{
    public PropertyDictionary propertyList;

    public float AddValue(ePropertyType eType, float value)
    {
        if (propertyList.ContainsKey(eType))
        {
            return propertyList[eType] += value;
        }

        return 0f;
    }

    public float GetValue(ePropertyType eType)
    {
        if (propertyList.ContainsKey(eType))
        {
            return propertyList[eType];
        }

        return 0f;
    }
}
