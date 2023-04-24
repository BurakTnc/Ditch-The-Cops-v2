using System;
using DG.Tweening;
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


        private int _waitingRewards;

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

        private void Start()
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
            targetReachedLevel = PlayerPrefs.GetInt("targetReachedLevel", 2);
        }

        public void SetPlayerLevel()
        {
            var xp = GameManager.Instance.GetPlayerXp();
            
            if (!(xp >= targetLevelXp))
                return;
            GameManager.Instance.ResetPlayerXp(targetLevelXp);
            OnReachTargetLevel();
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
            targetSurvivedTime = (int)(value * 1.5f) / 60;
            Save();
        }

        public void OnReachTargetReachedLevel(int value)
        {
            targetReachedLevel=(int)(value * 1.5f);
            Save();
        }
    }
}
