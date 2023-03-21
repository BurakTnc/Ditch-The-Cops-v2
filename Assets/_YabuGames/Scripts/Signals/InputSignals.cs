using System;
using UnityEngine;
using UnityEngine.Events;

namespace _YabuGames.Scripts.Signals
{
    public class InputSignals: MonoBehaviour
    {
        public static InputSignals Instance;
        
        public UnityAction<bool> OnTouch = delegate { };
        public UnityAction<bool> CanMove = delegate { };

        private void Awake()
        {
            #region Singleton

            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }

            Instance = this;
            #endregion
        }

    }
}