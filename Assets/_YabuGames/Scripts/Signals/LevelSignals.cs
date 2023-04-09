using System;
using UnityEngine;
using UnityEngine.Events;

namespace _YabuGames.Scripts.Signals
{
    public class LevelSignals : MonoBehaviour
    {
        public static LevelSignals Instance;

        public UnityAction OnPoliceEliminated = delegate { };
        public UnityAction<bool> OnSkillPanel = delegate { };
        public UnityAction<int> OnSkillActive = delegate { };
        public UnityAction OnPlayerDestroyed = delegate { };
        public UnityAction OnRevive = delegate { };

        private void Awake()
        {
            if (Instance!=this && Instance!=null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }
    }
}