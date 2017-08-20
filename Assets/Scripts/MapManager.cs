using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class StageJsonData
{
    public int obstacleCount;
    public string pathPrefabName;
    public string floorPrefabName;
    public string wallPrefabName;
    public List<Vector3> pathList = new List<Vector3>();
}

[System.Serializable]
public class MapJsonData
{
    public List<StageJsonData> stageList = new List<StageJsonData>();
}

public class MapManager : MonoBehaviour
{
    [System.Serializable]
    public class Map
    {
        public int obstacleCount;
        public GameObject floorPrefab;
        public GameObject wallPrefab;
        public GameObject pathPrefab;
        public GameObject pathObj;
    }

    public MapJsonData maps;
    public int mapIndex = 0;
    public bool loaded = false;
    public Map curMap;

    // Create map holder object
    string holderName = "Generated Map";
    Transform mapHolder;

    public void SetCurMapIndex(int index)
    {
        mapIndex = index;
    }

    public int GetCurMapIndex()
    {
        return mapIndex;
    }

    public int GetTotalMapCount()
    {
        return maps.stageList.Count;
    }

    public Vector3 GetCurMapSize()
    {
        if (curMap == null)
            return Vector3.zero;

        if (curMap.floorPrefab == null)
            return Vector3.zero;
        
        return curMap.floorPrefab.GetComponent<Renderer>().bounds.size;
    }

    public int GetObstacleMaxCount()
    {
        if (curMap == null)
            return 0;

        return curMap.obstacleCount;
    }

    public MotionPath GetMotionPath()
    {
        if (curMap == null)
            return null;

        if (curMap.pathObj == null)
            return null;

        return curMap.pathObj.GetComponent<MotionPath>();
    }

    void ReleaseStage()
    {
        curMap = null;
        if (transform.Find (holderName))
        {
            DestroyImmediate (transform.Find (holderName).gameObject);
        }

        mapHolder = new GameObject (holderName).transform;
        mapHolder.parent = transform;
    }

    void Release()
    {
        maps = null;
        loaded = false;

        ReleaseStage();
    }

    void CreateOutterWall(GameObject wallPrefab, GameObject floorObj, Transform mapHolder)
    {
        Vector3 floorSize = floorObj.GetComponent<Renderer>().bounds.size;
        Vector3 wallSize = wallPrefab.GetComponent<Renderer>().bounds.size;

        GameObject maskLeft = Instantiate (wallPrefab, Vector3.left * ((floorSize.x + wallSize.x) / 2f), Quaternion.identity);
        maskLeft.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
        maskLeft.transform.parent = mapHolder;
        maskLeft.transform.localScale = new Vector3 (floorSize.z, 1f, 1f);

        GameObject maskRight = Instantiate (wallPrefab, Vector3.right * ((floorSize.x + wallSize.x) / 2f), Quaternion.identity);
        maskRight.transform.rotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
        maskRight.transform.parent = mapHolder;
        maskRight.transform.localScale = new Vector3 (floorSize.z, 1f, 1f);

        GameObject maskTop = Instantiate (wallPrefab, Vector3.forward * ((floorSize.z + wallSize.z ) / 2f), Quaternion.identity);
        maskTop.transform.parent = mapHolder;
        maskTop.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        maskTop.transform.localScale = new Vector3(floorSize.x + (wallSize.x * 2f), 1f, 1f);

        GameObject maskBottom = Instantiate (wallPrefab, Vector3.back * ((floorSize.z + wallSize.z) / 2f), Quaternion.identity);
        maskBottom.transform.parent = mapHolder;
        maskBottom.transform.localScale = new Vector3 (floorSize.x + (wallSize.x  * 2f), 1f, 1f);
    }

    public void LoadMap()
    {
        if (loaded == false)
            return;

        ReleaseStage();

        int convertedIndex = mapIndex - 1;

        if (convertedIndex < 0 || convertedIndex > maps.stageList.Count)
            return;

        StageJsonData stageData = maps.stageList[convertedIndex];
        curMap = new Map();

        curMap.pathPrefab = Resources.Load(stageData.pathPrefabName) as GameObject;
        curMap.floorPrefab = Resources.Load(stageData.floorPrefabName) as GameObject;
        curMap.wallPrefab = Resources.Load(stageData.wallPrefabName) as GameObject;
        curMap.obstacleCount = stageData.obstacleCount;

        GameObject floorObj = Instantiate(curMap.floorPrefab, Vector3.zero, Quaternion.identity);
        floorObj.transform.parent = mapHolder;

        GameObject pathObj = Instantiate(curMap.pathPrefab, Vector3.zero, Quaternion.identity);
        pathObj.transform.parent = mapHolder;
        curMap.pathObj = pathObj;

        MotionPath path = pathObj.GetComponent<MotionPath>();
        path.controlPoints = new Vector3[stageData.pathList.Count];
        path.controlPoints = stageData.pathList.ToArray();
        path.Init();

        pathObj.GetComponent<PathController>().VisiblePath();

        CreateOutterWall(curMap.wallPrefab, curMap.floorPrefab, mapHolder);
    }

    public void SaveMap()
    {
        int convertedIndex = mapIndex - 1;

        if (convertedIndex < 0 || convertedIndex > maps.stageList.Count)
        {
            Debug.LogError("invalid index");
            return;
        }

        StageJsonData jsonData = maps.stageList[convertedIndex];

        jsonData.obstacleCount = curMap.obstacleCount;
        jsonData.floorPrefabName = curMap.floorPrefab.name;
        jsonData.wallPrefabName = curMap.wallPrefab.name;
        jsonData.pathPrefabName = curMap.pathPrefab.name;

        jsonData.pathList.Clear();

        PathController pathObj = curMap.pathObj.GetComponent<PathController>();
        MotionPath path = pathObj.GetComponent<MotionPath>();
        for (int i = 0; i < path.controlPoints.Length; i++)
        {
            jsonData.pathList.Add(path.controlPoints[i]);
        }
    }

    public void DeleteMap()
    {
        int convertedIndex = mapIndex - 1;

        if (convertedIndex < 0 || convertedIndex > maps.stageList.Count)
        {
            Debug.LogError("invalid index");
            return;
        }

        maps.stageList.RemoveAt(convertedIndex);
    }

    public void AddMap()
    {
        if (loaded == false)
            return;
        
        StageJsonData data = new StageJsonData();
        maps.stageList.Add(data);
    }

    public void LoadFromJson()
    {
        Release();

        maps = LoadMapFromJson();
        loaded = true;

        mapIndex = 0;
    }

    public void SaveToJson()
    {
        WriteData(maps);
    }

    string GetResourcePath()
    {
        return CommonUtil.pathForDocumentsFile("Assets/Resources/LevelData/level.json");
    }

    public void WriteData(MapJsonData data)
    {
        string fileName = "level.json";
        string jsonStr = JsonUtility.ToJson(data, true);

        Debug.Log(string.Format("========== write : ( {0} ) ==========", fileName));
        Debug.Log(jsonStr);

        string filePath = GetResourcePath();
        System.IO.File.WriteAllText(filePath, jsonStr, System.Text.Encoding.Unicode);
    }

    public MapJsonData LoadMapFromJson()
    {
        string levelName = "leveldata/level";
        TextAsset txtAsset = Resources.Load(levelName) as TextAsset; 
        StringReader stringReader = new StringReader(txtAsset.text); 
        string json = stringReader.ReadToEnd();

        Debug.Log(string.Format(" =============== load : ({0}) ============= ", levelName));
        Debug.Log(json);
        var data = JsonUtility.FromJson<MapJsonData> (json);
        return data;
    }

}
