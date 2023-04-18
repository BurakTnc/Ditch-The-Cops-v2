using System;
using UnityEngine;
using UnityEngine.Events;

namespace _YabuGames.Scripts.Signals
{
    public class AdSignals : MonoBehaviour
    {
        public static AdSignals Instance;

        public UnityAction<int> OnRewardedMapWatchComplete = delegate { };
        public UnityAction<int> OnRewardedCarWatchComplete = delegate { };
        private void Awake()
        {
            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }
    }
}