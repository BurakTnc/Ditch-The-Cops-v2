using System;
using _YabuGames.Scripts.Signals;
using UnityEngine;
using GameAnalyticsSDK;
namespace _YabuGames.Scripts.Managers
{
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance;
        public bool onMenu;

        public MaxManager MAX;
        private const int _showInterLimit = 120;

        public float _timer;
        private void Awake()
        {
        
            GameAnalytics.Initialize();
            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }
            
            Instance = this;
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

        public void ShowInter()
        {
            if (_timer<_showInterLimit)
                return;
            _timer = 0;
            MAX.ShowInter("INTER");

        }
    }
}