using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSpawnType
{
    INVALID,
    NPC_NORMAL,
    NPC_BOSS,
    BUFF,
}

public class EntitySpawner : MonoBehaviour
{
    Dictionary<eSpawnType, List<GameObject>> spawnedObjList = new Dictionary<eSpawnType, List<GameObject>>();

    Dictionary<eSpawnerState, ISpawnState> states = new Dictionary<eSpawnerState, ISpawnState>();
    ISpawnState spawnState = null;
    public eSpawnerState nextState = eSpawnerState.IDLE;
    public eSpawnerState curState = eSpawnerState.INVALID;

    public int totalSpawnCount;
    public float spawnDelay;
    public bool enableSpawnBoss;
    public bool testMode = false;
    public bool pause = false;

    // Create map holder object
    string holderName = "Spawned Entities";
    Transform mapHolder;

    void Awake()
    {
        states.Add(eSpawnerState.IDLE, new IdleState());
        states.Add(eSpawnerState.NORMAL, new NormalState(this));
        states.Add(eSpawnerState.BOSS, new BossState(this));
    }

	void Start ()
    {
        if (transform.Find (holderName))
        {
            DestroyImmediate (transform.Find (holderName).gameObject);
        }

        mapHolder = new GameObject (holderName).transform;
        mapHolder.parent = transform;

        GameManager gameMgr = FindObjectOfType<GameManager>();
        if (gameMgr)
        {
            gameMgr.OnPauseEvent += SetPause;
        }
    }

    void OnDestroy()
    {
        GameManager gameMgr = FindObjectOfType<GameManager>();
        if (gameMgr)
        {
            gameMgr.OnPauseEvent -= SetPause;
        }
    }

    void ChangeState(eSpawnerState state)
    {
        if (states.ContainsKey(state) == false)
            return;

        if (spawnState != null)
        {
            spawnState.OnExit();
        }

        spawnState = states[state];
        spawnState.OnEnter();

        curState = state;
    }

    void FixedUpdate()
    {
        if (pause == true)
            return;
        
        if (spawnState != null)
        {
            spawnState.OnUpdate(Time.fixedDeltaTime);
        }

        if (nextState != eSpawnerState.INVALID)
        {
            ChangeState(nextState);
            nextState = eSpawnerState.INVALID;
        }
    }

    public void SetNextState(eSpawnerState state)
    {
        nextState = state;
    }

    public int GetCurSpawnedObjCount(eSpawnType type)
    {
        if (spawnedObjList.ContainsKey(type) == false)
        {
            return 0;
        }

        return spawnedObjList[type].Count;
    }

    public void StartSpawn(int maxCount, float delay, bool lastSpawnBoss)
    {
        pause = false;
        totalSpawnCount = maxCount;
        spawnDelay = delay;
        enableSpawnBoss = lastSpawnBoss;
        SetNextState(eSpawnerState.NORMAL);
    }

    public void SpawnEntity(eSpawnType type, Vector3 spawnPos, string entityName)
    {
        GameObject prefab = ContentsManager.Instance.GetPrefabFromName(entityName);
        if (prefab == null)
            return;

        GameObject spawnedObj = Instantiate(prefab, spawnPos, transform.rotation);
        if (spawnedObj == null)
            return;

        if (spawnedObjList.ContainsKey(type) == false)
        {
            spawnedObjList.Add(type, new List<GameObject>());
        }
        spawnedObjList[type].Add(spawnedObj);

        Entity entity = spawnedObj.GetComponent<Entity>();
        if (entity)
        {
            entity.SetPause(false);
        }

        spawnedObj.transform.parent = mapHolder;
    }

    void SetPause(bool flag)
    {
        pause = flag;
    }

    public void ClearAllObstacle()
    {
        foreach(var pair in spawnedObjList)
        {
            List<GameObject> objList = pair.Value;
            for(int i = 0; i < objList.Count; i++)
            {
                Destroy(objList[i], 0);
            }
        }
        spawnedObjList.Clear();
        SetNextState(eSpawnerState.IDLE);
    }

    public Vector3 FindRandomSpawnPos()
    {
        MapManager mapMgr = FindObjectOfType<MapManager>();
        if (mapMgr == null)
            return transform.position;

        Vector3 mapSize = mapMgr.GetCurMapSize();
        float offsetPos = 2f;
        float convertedSizeX = mapSize.x - offsetPos;
        float convertedSizeZ = mapSize.z - offsetPos;
        float xPos = Random.Range(-convertedSizeX / 2, convertedSizeX / 2);
        float zPos = Random.Range(-convertedSizeZ / 2, convertedSizeZ / 2);

        return new Vector3(xPos, 0f, zPos);
    }

}
