using System;
using UnityEngine;
using UnityEngine.Events;

namespace _YabuGames.Scripts.Signals
{
    public class SkillSignals : MonoBehaviour
    {
        public static SkillSignals Instance;
        
        public UnityAction OnGodMode = delegate { };
        public UnityAction OnHealing = delegate { };
        public UnityAction OnMaxHealth = delegate { };
        public UnityAction OnReduceDamage = delegate { };
        public UnityAction OnNitro = delegate { };
        public UnityAction OnBonusIncome = delegate { };

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