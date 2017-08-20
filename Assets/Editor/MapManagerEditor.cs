using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MapManager))]
public class MapEditor : Editor 
{
    //private SerializedProperty samples;

    public override void OnInspectorGUI()
    {
        MapManager mapMgr = target as MapManager;

        DrawDefaultInspector();

        if (GUILayout.Button("1. Load Json"))
        {
            mapMgr.LoadFromJson();
        }

        if (GUILayout.Button("2. Load Map"))
        {
            mapMgr.LoadMap();
        }

        if (GUILayout.Button("3. Save Map"))
        {
            mapMgr.SaveMap();
        }

        if (GUILayout.Button("4. Save Json"))
        {
            mapMgr.SaveToJson();
        }

        if (GUILayout.Button("etc. Add Map"))
        {
            mapMgr.AddMap();
        }

        if (GUILayout.Button("etc. Delete Map"))
        {
            mapMgr.DeleteMap();
        }
    }


}
