using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : UIView
{
    public Button btnStart;
    public Toggle musicToggle;
    public Toggle soundToggle;
    public Image bgImg;
    public Text title;

    void Start()
    {
        if (musicToggle)
            musicToggle.isOn = AudioManager.Instance.musicVolumePercent > 0f ? true : false;

        if (soundToggle)
            soundToggle.isOn = AudioManager.Instance.sfxVolumePercent > 0f ? true : false;
    }

    public override void OnShow()
    {
        GameManager gameMgr = FindObjectOfType<GameManager>();
        if (gameMgr == null)
        {
            Debug.LogError("can not find main scene");
            return;
        }

        if (GameStateManager.Instance.gameState == eGameState.LOBBY)
        {
            SetTitleText(GameUIString.mainTitle);
            SetBgImg(true);
        }
        else if (GameStateManager.Instance.gameState == eGameState.GAME_END)
        {
            SetTitleText(GameUIString.gameEnd);
            SetBgImg(false);
        }
        else
        {
            SetTitleText(GameUIString.continueGame);
            SetBgImg(false);
        }

        AdMobManager adMgr = gameMgr.GetAdMobManager();
        if (adMgr)
        {
            adMgr.ShowBannerAd();
        }
    }

    public override void OnHide()
    {
        GameManager gameMgr = FindObjectOfType<GameManager>();
        if (gameMgr == null)
        {
            Debug.LogError("can not find main scene");
            return;
        }

        AdMobManager adMgr = gameMgr.GetAdMobManager();
        if (adMgr)
        {
            adMgr.HideBannerAd();
        }
    }

    public void SetBgImg(bool flag)
    {
        bgImg.enabled = flag;
    }

    public void SetTitleText(string text)
    {
        title.text = text;
    }

    public void OnActiveMusicToggle()
    {
        if (musicToggle == null)
            return;

        AudioManager.Instance.SetVolume(musicToggle.isOn ? 1f : 0f, AudioManager.AudioChannel.Music);
    }

    public void OnActiveSoundToggle()
    {
        if (soundToggle == null)
            return;

        AudioManager.Instance.SetVolume(soundToggle.isOn ? 1f : 0f, AudioManager.AudioChannel.Sfx);
    }

    public void OnClickStartGame()
    {
        MapManager mapMgr = FindObjectOfType<MapManager>();
        if (mapMgr == null)
            return;

        GameManager gameMgr = FindObjectOfType<GameManager>();
        if (gameMgr == null)
            return;

        UIManager uiMgr = FindObjectOfType<UIManager>();
        if (uiMgr == null)
            return;

        eGameState curState = GameStateManager.Instance.gameState;

        if (curState == eGameState.LOBBY)
        {
            mapMgr.SetCurMapIndex(1);
            GameStateManager.Instance.ChangeGameState(eGameState.INGAME);
        }
        else if (curState == eGameState.STAGE_COMPLETE)
        {
            GameStateManager.Instance.ChangeGameState(eGameState.INGAME);
        }
        else if (curState == eGameState.GAME_END)
        {
            GameStateManager.Instance.ChangeGameState(eGameState.INGAME);
        }

        gameMgr.SetPause(false);
        uiMgr.SetVisible(eUI_TYPE.MAIN_MENU, false);
    }
}
