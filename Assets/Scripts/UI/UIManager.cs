using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eUI_TYPE
{
    MAIN_MENU,
    HUD,
}

public class UIView : MonoBehaviour
{
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    public virtual void OnShow()
    {}

    public virtual void OnHide()
    {}
}

public class UIManager : MonoBehaviourSingleton<UIManager>
{
    Dictionary<eUI_TYPE, UIView> uiViews = new Dictionary<eUI_TYPE, UIView>();

	// Use this for initialization
    void MakeUIGameObject(eUI_TYPE uiType, string resName)
    {
        GameObject uiPrefab = Resources.Load("UI/" + resName) as GameObject;

        GameObject uiGameObject = Instantiate(uiPrefab, Vector3.zero, Quaternion.identity);
        uiGameObject.name = uiPrefab.name;

        uiGameObject.transform.SetParent(transform);

        UIView uiView = uiGameObject.GetComponent<UIView>();
        uiViews.Add(uiType, uiView);
    }

    void Awake()
    {
        MakeUIGameObject(eUI_TYPE.MAIN_MENU, "MainMenuView");
        MakeUIGameObject(eUI_TYPE.HUD, "HudView");

        HideAllViews();
    }

	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void HideAllViews()
    {
        foreach (var pair in uiViews)
        {
            SetVisible(pair.Key, false);
        }
    }

    public void SetVisible(eUI_TYPE uiType, bool visible)
    {
        if (uiViews.ContainsKey(uiType) == false)
        {
            Debug.LogError("invalid ui type : " + uiType);
            return;
        }

        uiViews[uiType].SetVisible(visible);

        if (visible)
        {
            uiViews[uiType].OnShow();
        }
        else
        {
            uiViews[uiType].OnHide();
        }
    }

    public UIView GetView(eUI_TYPE uiType)
    {
        if (uiViews.ContainsKey(uiType) == false)
            return null;

        return uiViews[uiType];
    }
}
