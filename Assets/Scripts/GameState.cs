using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game States
public enum eGameState
{
    INTRO,
    LOBBY,
    INGAME,
    MAIN_MENU,
    STAGE_COMPLETE,
    GAME_END
}

interface IGameState 
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
    string GetString();
    eGameState GetState();
}

public class IntroState : IGameState
{
    public eGameState GetState()
    {
        return eGameState.INTRO;
    }

    public string GetString()
    {
        return "eGameState.INTRO";
    }

    public void OnEnter()
    {
    }

    public void OnUpdate()
    {}

    public void OnExit()
    {
    }
}

public class LobbyState : IGameState
{
    MapManager mapMgr;
    UIManager uiMgr;
    GameManager gameMgr;

    public eGameState GetState()
    {
        return eGameState.LOBBY;
    }

    public string GetString()
    {
        return "eGameState.LOBBY";
    }

    public void OnEnter()
    {
        mapMgr = MonoBehaviour.FindObjectOfType<MapManager>();
        if (mapMgr)
        {
            mapMgr.LoadFromJson();
        }

        uiMgr = MonoBehaviour.FindObjectOfType<UIManager>();
        if (uiMgr)
        {
            uiMgr.SetVisible(eUI_TYPE.MAIN_MENU, true);
        }

        gameMgr = MonoBehaviour.FindObjectOfType<GameManager>();
        if (gameMgr)
        {
            gameMgr.ReleasePlayer();
        }

        AudioManager.Instance.PlayRandomMusic(1);
        AdMobManager adMgr = MonoBehaviour.FindObjectOfType<AdMobManager>();
        if (adMgr)
        {
            adMgr.ShowInterstitialAd();
        }
    }

    public void OnUpdate()
    {}

    public void OnExit()
    {
    }
}

public class GameEndState : IGameState
{
    
    public eGameState GetState()
    {
        return eGameState.GAME_END;
    }

    public string GetString()
    {
        return "eGameState.GAME_END";
    }

    public void OnEnter()
    {
        GameManager gameMgr = MonoBehaviour.FindObjectOfType<GameManager>();
        if (gameMgr)
        {
            gameMgr.SetPause(true);
        }

        UIManager uiMgr = MonoBehaviour.FindObjectOfType<UIManager>();
        if (uiMgr)
        {
            uiMgr.SetVisible(eUI_TYPE.MAIN_MENU, true);

            UIView uiView = uiMgr.GetView(eUI_TYPE.HUD);
            if (uiView)
            {
                HudView hudView = uiView.GetComponent<HudView>();
                if (hudView)
                {
                    hudView.StopPlayTimer();
                }
            }
        }
    }

    public void OnUpdate()
    {}

    public void OnExit()
    {
    }
}

public class StageCompleteState : IGameState
{
    public eGameState GetState()
    {
        return eGameState.STAGE_COMPLETE;
    }

    public string GetString()
    {
        return "eGameState.STAGE_COMPLETE";
    }

    public void OnEnter()
    {
        EntitySpawner entitySpawner = MonoBehaviour.FindObjectOfType<EntitySpawner>();
        if (entitySpawner == null)
            return;

        MapManager mapMgr = MonoBehaviour.FindObjectOfType<MapManager>();
        if (mapMgr == null)
            return;
            
        GameManager gameMgr = MonoBehaviour.FindObjectOfType<GameManager>();
        if (gameMgr == null)
            return;

        gameMgr.SetPause(true);

        int curIndex = mapMgr.GetCurMapIndex();
        if (curIndex >= mapMgr.GetTotalMapCount())
        {
            curIndex = 1;
        }
        else
        {
            curIndex++;
        }

        mapMgr.SetCurMapIndex(curIndex);

        gameMgr.UpdateStageInfo();

        UIManager uiMgr = MonoBehaviour.FindObjectOfType<UIManager>();
        if (uiMgr)
        {
            uiMgr.SetVisible(eUI_TYPE.MAIN_MENU, true);

            UIView uiView = uiMgr.GetView(eUI_TYPE.HUD);
            if (uiView)
            {
                HudView hudView = uiView.GetComponent<HudView>();
                if (hudView)
                {
                    hudView.UpdateStageInfoLabel();
                    hudView.StopPlayTimer();
                }
            }
        }
    }

    public void OnUpdate()
    {}

    public void OnExit()
    {
    }
}

public class InGameState : IGameState
{
    float delayDeltaTime = 0f;
    int delayStartSec = 3;
    bool triggerStart = false;

    UIManager uiMgr;
    MapManager mapMgr;
    EntitySpawner entitySpawner;
    GameManager gameMgr;

    public eGameState GetState()
    {
        return eGameState.INGAME;
    }

    public string GetString()
    {
        return "eGameState.InGame";
    }

    public void OnEnter()
    {
        mapMgr = MonoBehaviour.FindObjectOfType<MapManager>();
        if (mapMgr == null)
            return;

        entitySpawner = MonoBehaviour.FindObjectOfType<EntitySpawner>();
        if (entitySpawner == null)
            return;

        uiMgr = MonoBehaviour.FindObjectOfType<UIManager>();
        if (uiMgr == null)
            return;
        
        gameMgr = MonoBehaviour.FindObjectOfType<GameManager>();
        if (gameMgr == null)
            return;

        gameMgr.ReleasePlayer();

        mapMgr.LoadMap();
        uiMgr.HideAllViews();

        entitySpawner.ClearAllObstacle();

        uiMgr.SetVisible(eUI_TYPE.HUD, true);

        HudView hudView = uiMgr.GetView(eUI_TYPE.HUD) as HudView;
        if (hudView)
        {
            hudView.UpdateStageInfoLabel();
            hudView.ResetPlayTimer();
            hudView.SetDelayStart(delayStartSec);
        }

        delayDeltaTime = delayStartSec;
        triggerStart = true;
    }

    public void OnUpdate()
    {
        if (triggerStart == false)
            return;
        
        delayDeltaTime -= Time.deltaTime;
        if (delayDeltaTime > 0)
            return;

        Player player = gameMgr.SpawnPlayer();
        if (player == null)
            return;

        player.StartGame(mapMgr.GetMotionPath());

        entitySpawner.StartSpawn(mapMgr.GetObstacleMaxCount(), 1.5f, true);

        gameMgr.SetPause(false);

        HudView hudView = uiMgr.GetView(eUI_TYPE.HUD) as HudView;
        if (hudView)
        {
            hudView.ResetDelayTime();
            hudView.StartPlayTimer();
        }

        triggerStart = false;
    }

    public void OnExit()
    {
    }
}
