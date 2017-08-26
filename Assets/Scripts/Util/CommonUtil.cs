using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class CommonUtil
{
    static public Vector3 BaseScale()
    {
        return new Vector3(1f, 1f, 1f);
    }

    static public Vector3 GetDirToTarget(Vector3 from, Vector3 to)
    {
        return to - from;
    }

    static public Vector3 GetDirToTargetAdjustHeight(Vector3 from, Vector3 to)
    {
        from.y = to.y;

        return to - from;
    }

    static public Vector3 GetStraightDir(eStraightType type)
    {
        if (type == eStraightType.RANDOM)
        {
            type = (eStraightType)Random.Range((float)eStraightType.LEFT, (float)eStraightType.BOTTOM);
        }

        switch(type)
        {
            case eStraightType.TOP:
                return new Vector3(0f, 0f, 0.1f);
            case eStraightType.BOTTOM:
                return new Vector3(0f, 0f, -0.1f);
            case eStraightType.LEFT:
                return new Vector3(-0.1f, 0f, 0f);
            case eStraightType.RIGHT:
                return new Vector3(0.1f, 0f, 0f);
        }

        return new Vector3(0f, 0f, 0f);
    }

    static public bool IsNpc(GameObject obj)
    {
        return obj.CompareTag("Npc");
    }

    static public bool IsWall(GameObject obj)
    {
        return obj.CompareTag("Wall");
    }

    static public bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player");
    }

    static public bool IsBuff(GameObject obj)
    {
        return obj.CompareTag("BuffEntity");
    }

    static public string pathForDocumentsFile( string filename ) 
    { 
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring( 0, Application.dataPath.Length - 5 );
            path = path.Substring( 0, path.LastIndexOf( '/' ) );
            return System.IO.Path.Combine( System.IO.Path.Combine( path, "Documents" ), filename );
        }

        else if(Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath; 
            path = path.Substring(0, path.LastIndexOf( '/' ) ); 
            return System.IO.Path.Combine (path, filename);
        } 

        else 
        {
            string path = Application.dataPath; 
            path = path.Substring(0, path.LastIndexOf( '/' ) );
            return System.IO.Path.Combine (path, filename);
        }
    }
}
