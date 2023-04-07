using System;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class StatsManager : MonoBehaviour
    {

        public static StatsManager Instance;

        [HideInInspector] public int targetLevelXp;
        [HideInInspector] public int targetEliminate;
        [HideInInspector] public int targetSurvivedTime;
        [HideInInspector] public int targetReachedLevel;
        [HideInInspector] public int playerLevel;

        private void Awake()
        {
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            GetValues();
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Subscribe()
        {
            
        }

        private void UnSubscribe()
        {
            
        }

        private void Save()
        {
            PlayerPrefs.SetInt("playerLevel",playerLevel);
            PlayerPrefs.SetInt("targetLevelXp", targetLevelXp);
            PlayerPrefs.SetInt("targetEliminate", targetEliminate);
            PlayerPrefs.SetInt("targetSurvivedTime", targetSurvivedTime);
            PlayerPrefs.SetInt("targetReachedLevel", targetReachedLevel);

        }

        private void GetValues()
        {
            playerLevel = PlayerPrefs.GetInt("playerLevel", 1);
            targetLevelXp = PlayerPrefs.GetInt("targetLevelXp", 1000);
            targetEliminate = PlayerPrefs.GetInt("targetEliminate", 50);
            targetSurvivedTime = PlayerPrefs.GetInt("targetSurvivedTime", 5); 
            targetReachedLevel = PlayerPrefs.GetInt("targetReachedLevel", 5);
        }

        public void SetPlayerLevel()
        {
            var xp = GameManager.Instance.GetPlayerXp();
            
            if (!(xp >= targetLevelXp))
                return;
            OnReachTargetLevel();
            GameManager.Instance.ResetPlayerXp(targetLevelXp);
            UIManager.Instance.SetPlayerProgress();
        }

        private void OnReachTargetLevel()
        {
            playerLevel++;
            targetLevelXp = (int)(targetLevelXp * 1.5f);
            Save();
        }

        public void OnReachTargetEliminate(int value)
        {
            targetEliminate = (int)(value * 1.5f);
            Save();
        }

        public void OnReachTargetSurvive(int value)
        {
            targetSurvivedTime=(int)(value * 1.5f);
            Save();
        }

        public void OnReachTargetReachedLevel(int value)
        {
            targetReachedLevel=(int)(value * 1.5f);
            Save();
        }
    }
}
