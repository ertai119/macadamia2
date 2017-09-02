using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class StageJsonData
{
    public int obstacleCount;
    public bool spawnBoss;
    public string pathPrefabName;
    public string floorPrefabName;
    public string wallPrefabName;
    public List<Vector3> pathList = new List<Vector3>();
}

public class MapManager : MonoBehaviour
{
    [System.Serializable]
    public class Map
    {
        public int obstacleCount;
        public bool spawnBoss;
        public GameObject floorPrefab;
        public GameObject wallPrefab;
        public GameObject pathPrefab;
        public GameObject pathObj;
    }

    public MapJsonDataDictionary maps;
    public int mapIndex = 0;
    public bool loaded = false;
    public Map loadedMap;

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
        return maps.Count;
    }

    public Vector3 GetCurMapSize()
    {
        if (loadedMap == null)
            return Vector3.zero;

        if (loadedMap.floorPrefab == null)
            return Vector3.zero;
        
        return loadedMap.floorPrefab.GetComponent<Renderer>().bounds.size;
    }

    public int GetObstacleMaxCount()
    {
        if (loadedMap == null)
            return 0;

        return loadedMap.obstacleCount;
    }

    public bool EnableSpawnBoss()
    {
        if (loadedMap == null)
            return false;

        return loadedMap.spawnBoss;
    }

    public MotionPath GetMotionPath()
    {
        if (loadedMap == null)
            return null;

        if (loadedMap.pathObj == null)
            return null;

        return loadedMap.pathObj.GetComponent<MotionPath>();
    }

    void ReleaseStage()
    {
        loadedMap = null;
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

        if (maps.ContainsKey(mapIndex) == false)
        {
            Debug.LogError("invalid index");
            return;
        }

        StageJsonData stageData = maps[mapIndex];
        loadedMap = new Map();

        loadedMap.pathPrefab = Resources.Load(stageData.pathPrefabName) as GameObject;
        loadedMap.floorPrefab = Resources.Load(stageData.floorPrefabName) as GameObject;
        loadedMap.wallPrefab = Resources.Load(stageData.wallPrefabName) as GameObject;
        loadedMap.obstacleCount = stageData.obstacleCount;
        loadedMap.spawnBoss = stageData.spawnBoss;

        GameObject floorObj = Instantiate(loadedMap.floorPrefab, Vector3.zero, Quaternion.identity);
        floorObj.transform.parent = mapHolder;

        GameObject pathObj = Instantiate(loadedMap.pathPrefab, Vector3.zero, Quaternion.identity);
        pathObj.transform.parent = mapHolder;
        loadedMap.pathObj = pathObj;

        MotionPath path = pathObj.GetComponent<MotionPath>();
        path.controlPoints = new Vector3[stageData.pathList.Count];
        path.controlPoints = stageData.pathList.ToArray();
        path.Init();

        pathObj.GetComponent<PathController>().VisiblePath();

        CreateOutterWall(loadedMap.wallPrefab, loadedMap.floorPrefab, mapHolder);
    }

    public void SaveMap()
    {
        if (maps.ContainsKey(mapIndex) == false)
        {
            Debug.LogError("invalid index");
            return;
        }

        StageJsonData jsonData = maps[mapIndex];

        jsonData.obstacleCount = loadedMap.obstacleCount;
        jsonData.floorPrefabName = loadedMap.floorPrefab.name;
        jsonData.wallPrefabName = loadedMap.wallPrefab.name;
        jsonData.pathPrefabName = loadedMap.pathPrefab.name;
        jsonData.spawnBoss = loadedMap.spawnBoss;

        jsonData.pathList.Clear();

        PathController pathObj = loadedMap.pathObj.GetComponent<PathController>();
        MotionPath path = pathObj.GetComponent<MotionPath>();
        for (int i = 0; i < path.controlPoints.Length; i++)
        {
            jsonData.pathList.Add(path.controlPoints[i]);
        }

        pathObj.VisiblePath();
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

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh ();
#endif
    }

    string GetResourcePath()
    {
        return CommonUtil.pathForDocumentsFile("Assets/Resources/LevelData/level.json");
    }

    public void WriteData(MapJsonDataDictionary data)
    {
        SortedDictionary<int, StageJsonData> sortedData = new SortedDictionary<int, StageJsonData>();
        foreach(var pair in data)
        {
            sortedData.Add(pair.Key, pair.Value);
        }

        string fileName = "level.json";
        MapJsonDataDictionary writeData = new MapJsonDataDictionary();
        writeData.CopyFrom(sortedData);

        string jsonStr = JsonUtility.ToJson(writeData, true);


        string filePath = GetResourcePath();
        System.IO.File.WriteAllText(filePath, jsonStr, System.Text.Encoding.Unicode);
        Debug.Log(string.Format("========== write : ( {0} ) ==========", fileName));
    }

    public MapJsonDataDictionary LoadMapFromJson()
    {
        string levelName = "leveldata/level";
        TextAsset txtAsset = Resources.Load(levelName) as TextAsset; 
        StringReader stringReader = new StringReader(txtAsset.text); 
        string json = stringReader.ReadToEnd();

        Debug.Log(string.Format(" =============== load : ({0}) ============= ", levelName));
        var data = JsonUtility.FromJson<MapJsonDataDictionary> (json);
        return data;
    }

}
