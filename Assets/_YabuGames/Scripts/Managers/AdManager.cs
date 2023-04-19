using System;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance;

        private const int _showInterLimit = 120;

        private float _timer;
        private void Awake()
        {
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
            
        }

        public void ShowRewardedMap(int mapID)
        {
            AdSignals.Instance.OnRewardedMapWatchComplete?.Invoke(mapID);
        }

        public void ShowRewardedCar(int carID)
        {
            AdSignals.Instance.OnRewardedCarWatchComplete?.Invoke(carID);
        }

        public void ShowRewardedRevive()
        {
            LevelSignals.Instance.OnRevive?.Invoke();
        }

        public void ShowInter()
        {
            if (_timer<_showInterLimit)
                return;
            _timer = 0;

        }
    }
}