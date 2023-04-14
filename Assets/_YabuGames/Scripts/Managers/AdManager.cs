using System;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance;

        private void Awake()
        {
            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        public void ShowRewardedMap(int mapID)
        {
            
        }
    }
}