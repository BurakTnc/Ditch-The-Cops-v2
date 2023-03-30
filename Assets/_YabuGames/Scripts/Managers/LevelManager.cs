using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Objects;
using _YabuGames.Scripts.ScriptableObjects;
using _YabuGames.Scripts.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        [SerializeField] private float wantedLevelIncreaseValue;
        [SerializeField] private float skillPanelTime;
        [SerializeField] private List<SkillSpecs> skillSpecsList = new List<SkillSpecs>();
        [SerializeField] private SkillButton[] skillButtons;
        
        private float _wantedLevel;
        private int _passedLevels;
        private float _delayer;
        private readonly List<SkillSpecs> _chosenSkills = new List<SkillSpecs>(3);
        
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

        private void Start()
        {
            SetChaosValue();
            _delayer = skillPanelTime;

        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        #region Subscribtons

        private void Subscribe()
        {
            LevelSignals.Instance.OnPoliceEliminated += IncreaseWantedLevel;
            // CoreGameSignals.Instance.OnSave += Save;
            // CoreGameSignals.Instance.OnLevelWin += LevelWin;
            // CoreGameSignals.Instance.OnLevelLoad += LoadScene;
        }
        
        private void UnSubscribe()
        {
            LevelSignals.Instance.OnPoliceEliminated -= IncreaseWantedLevel;
            // CoreGameSignals.Instance.OnSave -= Save;
            // CoreGameSignals.Instance.OnLevelWin -= LevelWin;
            // CoreGameSignals.Instance.OnLevelLoad -= LoadScene;
        }

        #endregion

        private void Update()
        {
            OpenSkillPanel();
        }

        private void ChooseRandomSkill()
        {
            LevelSignals.Instance.OnSkillPanel?.Invoke(true);
            if (_chosenSkills.Count>0)
            {
                foreach (var skill in _chosenSkills)
                {
                    skillSpecsList.Add(skill);
                }
                _chosenSkills.Clear();
            }
            
            for (var i = 0; i < 3; i++)
            {
                var r = Random.Range(0, skillSpecsList.Count);
                var skill = skillSpecsList[r];
                skillSpecsList.Remove(skill);
                _chosenSkills.Add(skill);
            }

            for (var i = 0; i < 3; i++)
            {
                skillButtons[i].SetSkill(_chosenSkills[i]);
            }
        }
        private void OpenSkillPanel()
        {
            
            _delayer -= Time.deltaTime;
            _delayer = Mathf.Clamp(_delayer, 0, skillPanelTime);
            if (_delayer>0)
                return;
            ChooseRandomSkill();
            _delayer += skillPanelTime;
            UIManager.Instance.OpenSkillPanel();

        }

        private void IncreaseWantedLevel()
        {
            _wantedLevel += wantedLevelIncreaseValue;
            SetChaosValue();
        }

        private void SetChaosValue()
        {
            if (_wantedLevel>=5)
            {
                if(_passedLevels==5)
                    return;
                _passedLevels = 5;
                UIManager.Instance.SetStars(5);
                return;
            }
            if (_wantedLevel>=4)
            {
                if(_passedLevels==4)
                    return;
                _passedLevels = 4;
                UIManager.Instance.SetStars(4);
                return;
            }
            if (_wantedLevel>=3)
            {
                if(_passedLevels==3)
                    return;
                _passedLevels = 3;
                UIManager.Instance.SetStars(3);
                return;
            }
            if (_wantedLevel>=2)
            {
                if(_passedLevels==2)
                    return;
                _passedLevels = 2;
                UIManager.Instance.SetStars(2);
                return;
            }
            if (_wantedLevel<2)
            {
                if(_passedLevels==1)
                    return;
                _passedLevels = 1;
                UIManager.Instance.SetStars(1);
            }
        }
    }
}