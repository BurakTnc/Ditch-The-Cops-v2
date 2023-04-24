using System;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [HideInInspector] public bool onSurvive;
        [HideInInspector] public float survivedTimePerAPlay;
        public int money;
        [SerializeField] private GameObject missionsIcon;
        
        private float _playerXp;
        private int _eliminatedCops;
        private float _survivedTime;
        private int _targetEliminate;
        private int _targetSurvive;
        private int _targetReachedLevel;
        private int _playerLevel;
        private int _eliminatedCopsPerAPlay;
        private int _waitingRewards;
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
            LevelSignals.Instance.OnPlayerDestroyed += Stop;
            LevelSignals.Instance.OnRevive += Revive;
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
            LevelSignals.Instance.OnPlayerDestroyed -= Stop;
            LevelSignals.Instance.OnRevive -= Revive;
            LevelSignals.Instance.OnPoliceEliminated -= IncreaseEliminatedCops;
        }
        #endregion
        private void Start()
        {
            Application.targetFrameRate = 60;
            Initialize();
        }

        private void Update()
        {
            if(!onSurvive)
                return;
            _survivedTime += Time.deltaTime;
            survivedTimePerAPlay += Time.deltaTime;

        }
        

        private void Stop()
        {
            onSurvive = false;
        }

        private void Revive()
        {
            onSurvive = true;
        }

        private void Initialize()
        {
            GetTargetValues();
            SetEliminatedCops();
            SetPlayerLevel();
            SetSurvivedTime();
            RewardStatus();
            
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
            RewardStatus();
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
        public int GetCurrentSurvivedTime() => (int)survivedTimePerAPlay;
        public int GetEarnedMoney() => _eliminatedCopsPerAPlay + (int)survivedTimePerAPlay ;

        private void IncreaseEliminatedCops()
        {
            _eliminatedCops++;
            _eliminatedCopsPerAPlay++;
        }

        private void RewardStatus()
        {
            if(!missionsIcon)
                return;
            if (_waitingRewards>0)
            {
                missionsIcon.transform.DOShakeRotation(2f, Vector3.forward * 30, 6, 100, true)
                    .SetLoops(-1, LoopType.Restart).SetDelay(1);
            }
            else
            {
                missionsIcon.transform.DOKill(true);
                missionsIcon.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        private void SetEliminatedCops()
        {
            
            _eliminatedCops = Mathf.Clamp(_eliminatedCops, 0, 1000000);
            if(onSurvive)
                return;
            if (_eliminatedCops >= _targetEliminate)
            {
                _waitingRewards++;
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
                _waitingRewards++;
                _survivedTime = _targetSurvive*60;
                UIManager.Instance.OpenClaimButton(1);
            }
            CoreGameSignals.Instance.OnSave?.Invoke();
        }

        public void SetPlayerLevel()
        {
            GetTargetValues();
            Debug.Log(_playerLevel + "/ " + _targetReachedLevel);
            if (_playerLevel >= _targetReachedLevel) 
            {
                _waitingRewards++;
                UIManager.Instance.OpenClaimButton(2);
            }
            CoreGameSignals.Instance.OnSave?.Invoke();
        }


        public void ResetPlayerXp(int spentXp)
        {
            _playerXp -= spentXp;
            _playerXp = Mathf.Clamp(_playerXp, 0, 10000000);
            //SetPlayerLevel();
            Save();
        }

        public void ResetMissionProgress(int id)
        {
            switch (id)
            {
                case 0:
                    StatsManager.Instance.OnReachTargetEliminate(_eliminatedCops);
                    _waitingRewards--;
                    _eliminatedCops = 0;
                    break;
                case 1:
                    StatsManager.Instance.OnReachTargetSurvive((int)_survivedTime);
                    _waitingRewards--;
                    _survivedTime = 0;
                    break;
                case 2:
                    StatsManager.Instance.OnReachTargetReachedLevel(_playerLevel);
                    _waitingRewards--;
                    break;
                default:
                    break;
                    
            }
            Save();
        }
    }
}