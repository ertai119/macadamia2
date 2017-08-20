using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePropertyType
{
    INVALID = 0,
    SPEED,
}

public class PropertyManager : MonoBehaviour
{
    public PropertyDictionary propertyList;

    public bool AddValue(ePropertyType eType, float value)
    {
        if (propertyList.ContainsKey(eType))
        {
            propertyList[eType] += value;
            return true;
        }

        return false;
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
