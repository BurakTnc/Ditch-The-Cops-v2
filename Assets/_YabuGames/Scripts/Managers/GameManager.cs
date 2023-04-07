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
        
        private float _playerXp;
        private int _eliminatedCops;
        private float _survivedTime;
        private int _targetEliminate;
        private int _targetSurvive;
        private int _targetReachedLevel;
        private int _playerLevel;
        private int _eliminatedCopsPerAPlay;
        private float _survivedTimePerAPlay;
        private int _earnedMoney;
        
        

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
        private void Subscribe()
        {
            CoreGameSignals.Instance.OnSave += Save;
            CoreGameSignals.Instance.OnLevelFail += SetEliminatedCops;
            CoreGameSignals.Instance.OnLevelWin += SetEliminatedCops;
            CoreGameSignals.Instance.OnLevelFail += SetSurvivedTime;
            CoreGameSignals.Instance.OnLevelWin += SetSurvivedTime;
            CoreGameSignals.Instance.OnLevelFail += SetPlayerLevel;
            CoreGameSignals.Instance.OnLevelWin += SetPlayerLevel;
            CoreGameSignals.Instance.OnLevelFail += EarnMoney;
            CoreGameSignals.Instance.OnLevelWin += EarnMoney;
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
            CoreGameSignals.Instance.OnLevelFail -= EarnMoney;
            CoreGameSignals.Instance.OnLevelWin -= EarnMoney;
            LevelSignals.Instance.OnPoliceEliminated -= IncreaseEliminatedCops;
        }
        #endregion
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if(!onSurvive)
                return;
            _survivedTime += Time.deltaTime;
            _survivedTimePerAPlay += Time.deltaTime;

        }

        private void Initialize()
        {
            GetTargetValues();
            SetEliminatedCops();
            SetPlayerLevel();
            SetSurvivedTime();
            
        }

        private void GetTargetValues()
        {
            _playerLevel = StatsManager.Instance.playerLevel;
            _targetEliminate = StatsManager.Instance.targetEliminate;
            _targetSurvive = StatsManager.Instance.targetSurvivedTime;
            _targetReachedLevel = StatsManager.Instance.targetReachedLevel;
        }
        private void GetValues()
        {
            money = PlayerPrefs.GetInt("money", 0);
            _eliminatedCops = PlayerPrefs.GetInt("eliminatedCops", 0);
            _survivedTime = PlayerPrefs.GetFloat("survivedTime", 0);
            _playerXp = PlayerPrefs.GetFloat("playerXp", 0);
        }

        private void Save()
        {
            PlayerPrefs.SetInt("money",money);
            PlayerPrefs.SetInt("eliminatedCops",_eliminatedCops);
            PlayerPrefs.SetFloat("survivedTime",_survivedTime);
            PlayerPrefs.SetFloat("playerXp", _playerXp);
        }

        private void EarnMoney()
        {
            money += GetEarnedMoney();
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
        public float GetPlayerXp() => _playerXp;
        public int GetCurrentSurvivedTime() => (int)_survivedTimePerAPlay;
        public int GetEarnedMoney() => _eliminatedCopsPerAPlay + (int)_survivedTimePerAPlay / 100;

        private void IncreaseEliminatedCops()
        {
            _eliminatedCops++;
            _eliminatedCopsPerAPlay++;
        }
        private void SetEliminatedCops()
        {
            
            _eliminatedCops = Mathf.Clamp(_eliminatedCops, 0, 1000000);
            if(onSurvive)
                return;
            
            if (_eliminatedCops >= _targetEliminate)
            {
                _eliminatedCops = _targetEliminate;
                UIManager.Instance.OpenClaimButton(0);
            }
            CoreGameSignals.Instance.OnSave?.Invoke();
        }

        private void SetSurvivedTime()
        {
            _survivedTime = Mathf.Clamp(_survivedTime, 0, 600000);
            if(onSurvive)
                return;
            if (_survivedTime >= _targetSurvive*60)
            {
                _survivedTime = _targetSurvive*60;
                UIManager.Instance.OpenClaimButton(1);
            }
            CoreGameSignals.Instance.OnSave?.Invoke();
        }

        private void SetPlayerLevel()
        {
            if(onSurvive)
                return;
            if (_playerLevel >= _targetReachedLevel) 
            {
                UIManager.Instance.OpenClaimButton(2);
            }
            CoreGameSignals.Instance.OnSave?.Invoke();
        }


        public void ResetPlayerXp(int spentXp)
        {
            _playerXp -= spentXp;
            Save();
        }
    }
}