using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void OnGameEvent(bool flag);
    public event OnGameEvent OnPauseEvent = delegate{};

    bool pause = false;
    int curStage = 0;
    int maxStage = 0;

    Camera mainCam;
    EntitySpawner entitySpawner;
    MapManager mapMgr;
    UIManager uiMgr;
    AdMobManager adMobMgr;

    void Awake()
    {
        PrepareManager();
        PrepareGameData();
        InitCam();
    }

    void InitCam()
    {
        if (mainCam == null)
            return;

        float aspect = (float)Screen.width / (float)Screen.height;

        mainCam.aspect = aspect;
    }

	void Start ()
    {
        GameStateManager.Instance.ChangeGameState(eGameState.INTRO);
        GameStateManager.Instance.ChangeGameState(eGameState.LOBBY);
    }

    void PrepareGameData()
    {
        maxStage = PlayerPrefs.GetInt("maxStage", 0);
    }

    void PrepareManager()
    {
        ContentsManager.Instance.Init();

        GameObject mainCamPrefab = Resources.Load("MainCamera") as GameObject;
        GameObject mainCamObj = Instantiate(mainCamPrefab, mainCamPrefab.transform.position, mainCamPrefab.transform.rotation);
        mainCamObj.name = mainCamPrefab.name;
        mainCam = mainCamObj.GetComponent<Camera>();

        GameObject mapMgrPrefab = Resources.Load("MapManager") as GameObject;
        GameObject mapMgrObj = Instantiate(mapMgrPrefab, Vector3.zero, Quaternion.identity);
        mapMgrObj.name = mapMgrPrefab.name;
        mapMgrObj.transform.parent = gameObject.transform;
        mapMgr = mapMgrObj.GetComponent<MapManager>();

        GameObject uiMgrPrefab = Resources.Load("UIManager") as GameObject;
        GameObject uiMgrObj = Instantiate(uiMgrPrefab, Vector3.zero, Quaternion.identity);
        uiMgrObj.name = uiMgrPrefab.name;
        uiMgrObj.transform.parent = gameObject.transform;
        uiMgr = uiMgrObj.GetComponent<UIManager>();

        GameObject npcSpawnerPrefab = Resources.Load("EntitySpawner") as GameObject;
        GameObject npcSpanwerObj = Instantiate(npcSpawnerPrefab, Vector3.zero, Quaternion.identity);
        npcSpanwerObj.name = npcSpawnerPrefab.name;
        npcSpanwerObj.transform.parent = gameObject.transform;
        entitySpawner = npcSpanwerObj.GetComponent<EntitySpawner>();

        GameObject admobPrefab = Resources.Load("AdManager") as GameObject;
        GameObject admobMgrObj = Instantiate(admobPrefab, Vector3.zero, Quaternion.identity);
        admobMgrObj.name = admobPrefab.name;
        admobMgrObj.transform.parent = gameObject.transform;
        adMobMgr = admobMgrObj.GetComponent<AdMobManager>();
        adMobMgr.Init();
    }

    public int GetCurStageIndex()
    {
        return curStage;
    }

    public int GetMaxStageIndex()
    {
        return maxStage;
    }

    public void UpdateStageInfo()
    {
        curStage++;
        if (curStage > maxStage)
        {
            maxStage = curStage;
            PlayerPrefs.SetInt("maxStage", maxStage);
        }
    }

    public Player SpawnPlayer()
    {
        Player spawnedPlayerObj = FindObjectOfType<Player>();
        if (spawnedPlayerObj)
        {
            
            DestroyImmediate(spawnedPlayerObj.gameObject);
        }

        GameObject playerPrefab = Resources.Load("Ufo") as GameObject;
        GameObject playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        playerObj.name = playerPrefab.name;
        playerObj.transform.parent = gameObject.transform;

        return playerObj.GetComponent<Player>();
    }

    public void ReleasePlayer()
    {
        Player spawnedPlayerObj = FindObjectOfType<Player>();
        if (spawnedPlayerObj)
        {
            DestroyImmediate(spawnedPlayerObj.gameObject);
        }
    }

    public bool IsPause()
    {
        return pause;
    }

    public void SetPause(bool flag)
    {
        pause = flag;

        UIView uiView = uiMgr.GetView(eUI_TYPE.HUD);
        if (uiView)
        {
            uiView.GetComponent<HudView>().SetPause(flag);
        }

        OnPauseEvent(flag);
    }

    public AdMobManager GetAdMobManager()
    {
        return adMobMgr;
    }

	// Update is called once per frame
	void Update ()
    {
	}
}
