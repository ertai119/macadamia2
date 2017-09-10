using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudView : UIView
{
    public Text stageInfoLabel;
    public Text playtimeLabel;
    public Text delayLabel;
    public Text helpLabel;

    private float playTime;
    private bool startPlayTimer = false;

    int delay;
    float delayTime;
    bool startDelayTimer = false;

    bool pause = false;

    void Awake()
    {
        ResetPlayTimer();

        delay = 0;
        delayTime = 0;
        startDelayTimer = false;
    }

    void Update()
    {
        if (pause == true)
            return;
        
        if (startPlayTimer == true)
        {
            UpdatePlayTimer();
        }

        if (startDelayTimer == true)
        {
            UpdateDelayTimer();
        }
    }

    public void SetPause(bool flag)
    {
        pause = flag;
    }

    public void OnClickMenu()
    {
        GameManager gameMgr = FindObjectOfType<GameManager>();
        if (gameMgr)
        {
            gameMgr.SetPause(true);
        }

        UIManager uiMgr = FindObjectOfType<UIManager>();
        if (uiMgr)
        {
            uiMgr.SetVisible(eUI_TYPE.MAIN_MENU, true);
        }
    }

    void UpdatePlayTimer()
    {
        playTime += Time.fixedDeltaTime;

        var minutes = playTime / 60; //Divide the guiTime by sixty to get the minutes.
        var seconds = playTime % 60;//Use the euclidean division for the seconds.
        var fraction = (playTime * 100) % 100;

        SetPlayTimerText(minutes, seconds, fraction);
    }

    void UpdateDelayTimer()
    {
        delayTime += Time.fixedDeltaTime;

        var seconds = delayTime % 60;//Use the euclidean division for the seconds.

        float remainDelay = delay - seconds;
        remainDelay = Mathf.Floor(remainDelay);
        remainDelay = Mathf.Max(remainDelay, 0);

        SetDelayTimerText(remainDelay);
    }

    void SetDelayTimerText(float seconds)
    {
        if (seconds <= 0)
        {
            delayLabel.text = GameUIString.start;
            helpLabel.text = "";
        }
        else
        {
            delayLabel.text = string.Format("{0:0}", seconds);
            helpLabel.text = GameUIString.heplMsg;
        }
    }

    void SetPlayTimerText(float minutes, float seconds, float fraction)
    {
        playtimeLabel.text = string.Format ("{0} - {1:00} : {2:00}", GameUIString.playTime, minutes, seconds);
    }

    public void SetDelayStart(int delay)
    {
        this.delay = delay;
        startDelayTimer = true;
    }

    public void ResetDelayTime()
    {
        startDelayTimer = false;
        delay = 0;
        delayTime = 0;
        delayLabel.text = "";
    }

    public void UpdateStageInfoLabel()
    {
        GameManager gameMgr = FindObjectOfType<GameManager>();
        if (gameMgr == null)
            return;
        
        stageInfoLabel.text = string.Format("{0} : {1} {2} : {3}"
            , GameUIString.stage
            , gameMgr.GetCurStageIndex() + 1
            , GameUIString.best
            , gameMgr.GetMaxStageIndex() + 1);
    }

    public void StopPlayTimer()
    {
        startPlayTimer = false;
    }

    public void StartPlayTimer()
    {
        playTime = 0f;
        startPlayTimer = true;
    }

    public void ResetPlayTimer()
    {
        playTime = 0f;
        startPlayTimer = false;
        SetPlayTimerText(0f, 0f, 0f);
    }
}
