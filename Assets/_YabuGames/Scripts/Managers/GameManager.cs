using System;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [HideInInspector] public bool onSurvive;
        public int money;
        
        private int _playerLevel;
        private float _playerXp;
        private int _eliminatedCops;
        public float _survivedTime;
        

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
            GetValues();
        }

        #region Subscribtions
        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Start()
        {
            SetEliminatedCops();
            SetPlayerLevel();
            SetSurvivedTime();
        }

        private void Update()
        {
            if(!onSurvive)
                return;
            _survivedTime += Time.deltaTime;
        }

        private void Subscribe()
        {
            CoreGameSignals.Instance.OnSave += Save;
            CoreGameSignals.Instance.OnLevelFail += SetEliminatedCops;
            CoreGameSignals.Instance.OnLevelWin += SetEliminatedCops;
            CoreGameSignals.Instance.OnLevelFail += SetSurvivedTime;
            CoreGameSignals.Instance.OnLevelWin += SetSurvivedTime;
            CoreGameSignals.Instance.OnLevelFail += SetPlayerLevel;
            CoreGameSignals.Instance.OnLevelWin += SetPlayerLevel;
            LevelSignals.Instance.OnPoliceEliminated += IncreaseEliminatedCops;
        }

        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnSave -= Save;
            CoreGameSignals.Instance.OnLevelFail -= SetEliminatedCops;
            CoreGameSignals.Instance.OnLevelWin -= SetEliminatedCops;
            CoreGameSignals.Instance.OnLevelFail -= SetSurvivedTime;
            CoreGameSignals.Instance.OnLevelWin -= SetSurvivedTime;
            CoreGameSignals.Instance.OnLevelFail -= SetPlayerLevel;
            CoreGameSignals.Instance.OnLevelWin -= SetPlayerLevel;
            LevelSignals.Instance.OnPoliceEliminated -= IncreaseEliminatedCops;
        }

        #endregion

        private void GetValues()
        {
            money = PlayerPrefs.GetInt("money", 0);
            _eliminatedCops = PlayerPrefs.GetInt("eliminatedCops", 0);
            _survivedTime = PlayerPrefs.GetFloat("survivedTime", 0);
            _playerLevel = PlayerPrefs.GetInt("playerLevel",1);
            _playerXp = PlayerPrefs.GetFloat("playerXp", 0);
        }

        private void Save()
        {
            PlayerPrefs.SetInt("money",money);
            PlayerPrefs.SetInt("eliminatedCops",_eliminatedCops);
            PlayerPrefs.SetFloat("survivedTime",_survivedTime);
            PlayerPrefs.SetInt("playerLevel",_playerLevel);
            PlayerPrefs.SetFloat("playerXp", _playerXp);
        }

        public void ArrangeMoney(int value)
        {
            money += value;
        }

        public int GetMoney()
        {
            return money < 0 ? 0 : money;
        }

        public void IncreaseXp(int amount)
        {
            _playerXp += amount;
        }

        public int GetEliminatedCops() => _eliminatedCops;
        public float GetSurvivedTime() => _survivedTime;
        public int GetPlayerLevel() => _playerLevel;
        public float GetPlayerXp() => _playerXp;

        private void IncreaseEliminatedCops()
        {
            _eliminatedCops++;
        }
        private void SetEliminatedCops()
        {
            _eliminatedCops = Mathf.Clamp(_eliminatedCops, 0, 100);
            if(onSurvive)
                return;
            if (_eliminatedCops == 100)
            {
                UIManager.Instance.OpenClaimButton(0);
            }
            CoreGameSignals.Instance.OnSave?.Invoke();
        }

        private void SetSurvivedTime()
        {
            _survivedTime = Mathf.Clamp(_survivedTime, 0, 6000);
            if(onSurvive)
                return;
            if (_survivedTime >= 6000) 
            {
                UIManager.Instance.OpenClaimButton(1);
            }
            CoreGameSignals.Instance.OnSave?.Invoke();
        }

        private void SetPlayerLevel()
        {
            if(onSurvive)
                return;
            if (_playerLevel >= 100) 
            {
                UIManager.Instance.OpenClaimButton(2);
            }
            CoreGameSignals.Instance.OnSave?.Invoke();
        }
        

    }
}