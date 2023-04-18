using System;
using _YabuGames.Scripts.Signals;
using _YabuGames.Scripts.Spawners;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class IAPManager : MonoBehaviour
    {
        public static IAPManager Instance;

        private void Awake()
        {
            if (Instance != this && Instance != null) 
            {
              Destroy(this);
              return;
            }

            Instance = this;
        }

        public void PurchaseCoin(int earnValue)
        {
            var coinCount = earnValue / 100;
            CoreGameSignals.Instance.OnSpawnCoins?.Invoke(coinCount, 0, 0, true);
            GameManager.Instance.money += earnValue;
            CoreGameSignals.Instance.OnSave?.Invoke();
        }
    }
}