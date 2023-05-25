using System;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;
using GameAnalyticsSDK;
namespace _YabuGames.Scripts.Managers
{
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance;
        public bool onMenu;

        [SerializeField] private MaxManager MAX;
        [SerializeField] private GameObject adLoadingPanel;
        private const int _showInterLimit = 70;

       [HideInInspector] public float _timer;
        private void Awake()
        {
        
            GameAnalytics.Initialize();
            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }
            
            Instance = this;
            MAX = GameObject.Find("MAXMANAGER").GetComponent<MaxManager>();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if(!onMenu)
                return;
            ShowInter();
        }

        public void showReward(string pass)
        {
            MAX.ShowRewarded(pass);

        }

        public void ShowRewardedMap(int mapID)
        {
            MAX.currentMap = mapID;
            MAX.ShowRewarded("MapRewarded");
        }

        public void ShowRewardedCar(int carID)
        {


            MAX.currentCar = carID;
            MAX.ShowRewarded("CarRewarded");
        }

        public void ShowRewardedRevive()
        {
            MAX.ShowRewarded("OnRevive");
        }

        public void ShowInter(bool isForced=false)
        {
            if (isForced)
            {
                MAX.ShowInter("INTER");
                return;
            }
            if (PlayerPrefs.GetInt("IsNoAds") == 1)
            {
                _timer = 0;
                return;
            }
            if (_timer<_showInterLimit)
                return;
            _timer = 0;
            if (adLoadingPanel)
            {
                adLoadingPanel.SetActive(true);
                adLoadingPanel.transform.DOScale(Vector3.one, .9f).OnComplete(Show);
            }
            else
            {
                MAX.ShowInter("INTER");
            }

            void Show()
            {
                adLoadingPanel.SetActive(false);
                MAX.ShowInter("INTER");
            }

        }
        
    }
}