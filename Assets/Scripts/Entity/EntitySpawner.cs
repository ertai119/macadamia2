using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    List<GameObject> curSpawnedEntities = new List<GameObject>();
    List<GameObject> curSpawnedBuff = new List<GameObject>();

    public ContentsLibrary contentsLibrary;
    public int obstacleMaxCount = 0;
    public float spawnDelay = 0f;
    public bool startSpawn = false;
    public float nextSpawnTime = 0f;
    public bool testMode = false;

    float curTime = 0f;
    bool pause = false;

    // Create map holder object
    string holderName = "Spawned Entities";
    Transform mapHolder;

    public void Init()
    {
        if (transform.Find (holderName))
        {
            DestroyImmediate (transform.Find (holderName).gameObject);
        }

        mapHolder = new GameObject (holderName).transform;
        mapHolder.parent = transform;

        contentsLibrary.InitLibrary();
    }

	void Start ()
    {
        Init();
	}

	void FixedUpdate ()
    {
        if (pause == true)
            return;

        curTime += Time.fixedDeltaTime;

        if (EnableSpawn() == false)
            return;
        
        nextSpawnTime += spawnDelay;
        SpawnEntity();
	}

    public void StartSpawn(int maxCount, float delay)
    {
        obstacleMaxCount = maxCount;
        spawnDelay = delay;
        nextSpawnTime = curTime + spawnDelay;

        pause = false;
        startSpawn = true;
    }

    bool EnableSpawn()
    {
        if (startSpawn == false)
            return false;

        if (curTime < nextSpawnTime)
            return false;

        if (curSpawnedEntities.Count >= obstacleMaxCount)
            return false;
        
        return true;
    }

    void SpawnEntity()
    {
        Vector3 spawnPos = FindRandomSpawnPos();

        string entityName = GameResourceName.contentBaseNpcName;

        bool isBuffEntity = false;
        float roullet = Random.Range(0f, 100f);
        if (roullet > 80)
        {
            entityName = GameResourceName.contentsBuffName;
            isBuffEntity = true;
        }
        else if (roullet > 20)
        {
            entityName = GameResourceName.contentsOptionalNpcName;
        }

        GameObject prefab = contentsLibrary.GetPrefabFromName(entityName);
        if (prefab == null)
            return;

        GameObject spawnedObj = Instantiate(prefab, spawnPos, transform.rotation);

        if (isBuffEntity)
        {
            curSpawnedBuff.Add(spawnedObj);
        }
        else
        {
            spawnedObj.tag = "Npc";
            curSpawnedEntities.Add(spawnedObj);
        }

        Entity entity = spawnedObj.GetComponent<Entity>();
        if (entity)
        {
            entity.SetPause(false);
        }

        spawnedObj.transform.parent = mapHolder;
    }

    public void SetPause(bool flag)
    {
        for (int i = 0; i < curSpawnedEntities.Count; i++)
        {
            GameObject obj = curSpawnedEntities[i];
            if (obj != null)
            {
                Entity entity = obj.GetComponent<Entity>();
                if (entity != null)
                {
                    entity.SetPause(flag);
                }
            }
        }

        for (int i = 0; i < curSpawnedBuff.Count; i++)
        {
            GameObject obj = curSpawnedBuff[i];
            if (obj != null)
            {
                Entity entity = obj.GetComponent<Entity>();
                if (entity != null)
                {
                    entity.SetPause(flag);
                }
            }
        }

        pause = flag;
    }

    public void ClearAllObstacle()
    {
        startSpawn = false;

        for(int i = 0 ; i < curSpawnedEntities.Count; i++)
        {
            Destroy(curSpawnedEntities[i], 0);
        }
        curSpawnedEntities.Clear();

        for (int i = 0; i < curSpawnedBuff.Count; i++)
        {
            Destroy(curSpawnedBuff[i], 0);
        }
        curSpawnedBuff.Clear();
    }

    Vector3 FindRandomSpawnPos()
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
