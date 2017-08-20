using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

// android
// banner_main_title
// ca-app-pub-8850312662591455/1965407077
// 광고단위id ca-app-pub-8850312662591455/1965407077
// 앱id ca-app-pub-8850312662591455~5764873679

// ios
// macadamia2_ios
// banner_main_title
// ca-app-pub-8850312662591455~9452837248 앱id
// ca-app-pub-8850312662591455/2316760861 광고단위id

// ios device id
// 56664112992c8d8d6d6c44e9a4509940

public class AdMobManager : MonoBehaviour
{
    public string android_banner_id;
    public string ios_banner_id;

    public string android_interstitial_id;
    public string ios_interstitial_id;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;

    public bool testDevice = false;

    public void Start()
    {
    }

    public void Init()
    {
        RequestBannerAd();
        RequestInterstitialAd();
    }

    public void RequestBannerAd()
    {
        string adUnitId = string.Empty;

#if UNITY_ANDROID
        adUnitId = android_banner_id;
#elif UNITY_IOS
        adUnitId = ios_banner_id;
#endif

        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        AdRequest request;
        if (testDevice == true)
        {
            request = new AdRequest.Builder().AddTestDevice(
                AdRequest.TestDeviceSimulator).AddTestDevice("56664112992c8d8d6d6c44e9a4509940").Build();
        }
        else
        {
            request = new AdRequest.Builder().Build();
        }

        bannerView.LoadAd(request);
    }

    private void RequestInterstitialAd()
    {
        string adUnitId = string.Empty;

#if UNITY_ANDROID
        adUnitId = android_interstitial_id;
#elif UNITY_IOS
        adUnitId = ios_interstitial_id;
#endif

        interstitialAd = new InterstitialAd(adUnitId);

        AdRequest request;
        if (testDevice == true)
        {
            request = new AdRequest.Builder().AddTestDevice(
                AdRequest.TestDeviceSimulator).AddTestDevice("56664112992c8d8d6d6c44e9a4509940").Build();
        }
        else
        {
            request = new AdRequest.Builder().Build();
        }

        interstitialAd.LoadAd(request);

        interstitialAd.OnAdClosed += HandleOnInterstitialAdClosed;
    }

    public void HandleOnInterstitialAdClosed(object sender, System.EventArgs args)
    {
        print("HandleOnInterstitialAdClosed event received.");

        interstitialAd.Destroy();

        RequestInterstitialAd();
    }

    public void ShowBannerAd()
    {
        if (bannerView != null)
            bannerView.Show();
    }

    public void HideBannerAd()
    {
        if (bannerView != null)
            bannerView.Hide();
    }

    public void ShowInterstitialAd()
    {
        if (!interstitialAd.IsLoaded())
        {
            RequestInterstitialAd();
            return;
        }

        interstitialAd.Show();
    }

    /*
    void OnGUI() // deprecated, use ordinary .UI now available in Unity
    {
        if(GUI.Button(new Rect(0, 0, 100, 100), "interstitial ad"))
        {
            ShowInterstitialAd();
            Debug.Log("show interstitialad");
        }

        if(GUI.Button(new Rect(0, 100, 100, 100), "hide banner"))
        {
            HideBannerAd();
            Debug.Log("hide banner");
        }

        if(GUI.Button(new Rect(0, 200, 100, 100), "show banner"))
        {
            ShowBannerAd();
            Debug.Log("show banner");
        }
    }*/

}