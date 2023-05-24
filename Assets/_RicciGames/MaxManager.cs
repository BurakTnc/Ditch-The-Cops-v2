using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using UnityEngine;

public class MaxManager : MonoBehaviour
{
    public static MaxManager _instance;

    public int currentCar;
    public int currentMap;
    public GameObject button;
        
    void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // AppLovin SDK is initialized, start loading ads
        };
        MaxSdk.SetSdkKey("_kziwD3Fce76fVb2eZQT0v4pGloAtkTsSQZMecbGGkNwDzytXNKXHPLT97i1sBuIcReF3EWJyuE9rA1NTLvqmF");
        MaxSdk.InitializeSdk();
        InitializeInterstitialAds();
        InitializeRewardedAds();

   
    }
    public static string interstitialAdUnitId = "6032d3962789e27f";
    public static string rewardedAdUnitId = "3704ade8023b3cd3";

    int retryAttempt;
    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        

        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        //Time.timeScale = 1;

        if (currentInter == "INTER")
        {

            StopMusic("Play");

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "INTER");
            PlayerPrefs.SetInt("INTER", PlayerPrefs.GetInt("INTER", 0) + 1);
            //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "INTER" + PlayerPrefs.GetInt("INTER", 0));

        }

        LoadInterstitial();
    }

 


    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first RewardedAd
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardedAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        

        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
       

        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward
        // 
        GiveReward();

        LoadRewardedAd();
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId) { }

    private void OnRewardedAdClickedEvent(string adUnitId) { }

    private void OnRewardedAdDismissedEvent(string adUnitId)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

 
    public Animator flyAnim, TntAnim, BoostAnim;
    private void GiveReward()
    {
        Time.timeScale = 1;
   
        if(currentreward == "CarRewarded")
        {
            AdSignals.Instance.OnRewardedCarWatchComplete.Invoke(currentCar);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "CarRewarded" + DateTime.Now.Year+"_"+DateTime.Now.Month+"_"+DateTime.Now.Day);
            PlayerPrefs.SetInt("CarRewarded", PlayerPrefs.GetInt("CarRewarded", 0) + 1);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "CarRewarded" + PlayerPrefs.GetInt("CarRewarded", 0));
        }
        else if (currentreward == "MapRewarded")
        {
            AdSignals.Instance.OnRewardedMapWatchComplete?.Invoke(currentMap);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "MapRewarded" + DateTime.Now.Year+"_"+DateTime.Now.Month+"_"+DateTime.Now.Day);
            PlayerPrefs.SetInt("MapRewarded", PlayerPrefs.GetInt("MapRewarded", 0) + 1);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "MapRewarded" + PlayerPrefs.GetInt("MapRewarded", 0));
        }
        else if (currentreward == "DoubleCoin")
        {
            UIManager.Instance.CoinReward(button);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "DoubleCoin" + DateTime.Now.Year+"_"+DateTime.Now.Month+"_"+DateTime.Now.Day);
            PlayerPrefs.SetInt("DoubleCoin", PlayerPrefs.GetInt("DoubleCoin", 0) + 1);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "DoubleCoin" + PlayerPrefs.GetInt("DoubleCoin", 0)); 
        }
        else if (currentreward == "OnRevive")
        {
            LevelSignals.Instance.OnRevive.Invoke();
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "OnRevive" + DateTime.Now.Year+"_"+DateTime.Now.Month+"_"+DateTime.Now.Day);
            PlayerPrefs.SetInt("OnRevive", PlayerPrefs.GetInt("OnRevive", 0) + 1);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "OnRevive" + PlayerPrefs.GetInt("OnRevive", 0)); 
        }

        StopMusic("Play");

        LoadRewardedAd();
            //Debug.LogError("Rewarded Bitti " + currentreward);
    }
    public string currentreward, currentInter;

    public void ShowRewarded(string value)
    {

        if (MaxSdk.IsRewardedAdReady(MaxManager.rewardedAdUnitId))
        {
            Time.timeScale = 0;
           
            StopMusic("Stop");
          
            currentreward = value;
            MaxSdk.ShowRewardedAd(MaxManager.rewardedAdUnitId);

        }
    }
    public void ShowInter(string value)
    {
        if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
        {
          
            StopMusic("Stop");
            Time.timeScale = 0;

        
            currentInter = value;
            MaxSdk.ShowInterstitial(interstitialAdUnitId);
        }

    }

    AudioSource[] allAudioSources;
    List<AudioSource> allAudioSourcesStopped = new List<AudioSource>();
    void StopMusic(string CurrentValue)
    {
        allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var item in allAudioSources)
        {
            if (item.isPlaying)
            {
                allAudioSourcesStopped.Add(item);
            }
        }
        if (CurrentValue == "Play")
        {
            foreach (var audioS in allAudioSourcesStopped)
            {
                if (audioS != null)
                {
                    audioS.Play();

                }
            }
            allAudioSourcesStopped.Clear();
        }
        else
        {
            foreach (var audioS in allAudioSourcesStopped)
            {
                if (audioS != null)
                {
                    audioS.Stop();

                }
            }
        }

    }
}
