using System;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class StatsManager : MonoBehaviour
    {

        public static StatsManager Instance;

        public int[] targetLevelXp;
        public int[] targetEliminate;
        public int[] targetSurvivedTime;
        public int[] targetReachedLevel;

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
