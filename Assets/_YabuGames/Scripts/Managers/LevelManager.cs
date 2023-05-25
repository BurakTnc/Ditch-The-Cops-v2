using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Objects;
using _YabuGames.Scripts.ScriptableObjects;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using GameAnalyticsSDK;
using TMPro;
using UnityEngine.Serialization;

namespace _YabuGames.Scripts.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        public List<SkillSpecs> skillSpecsList = new List<SkillSpecs>();
        [HideInInspector] public List<SkillSpecs> chosenSkills = new List<SkillSpecs>(3);
        [HideInInspector] public bool onLose;

        [SerializeField] private TextMeshProUGUI skillBoostText;
        
        [SerializeField] private GameObject criticalHealthPopUp, wantedLevelPopUp;
        [SerializeField] private GameObject reduceSkillChoosePopUp;
        [SerializeField] private GameObject extraSkillPopUp;

        [SerializeField] private float wantedLevelIncreaseValue;
        [SerializeField] private float skillPanelTime;
        [SerializeField] private SkillButton[] skillButtons;
        [SerializeField] private AudioClip[] wantedLevelIncreaseSounds;

        private AudioSource _source;
        private float _wantedLevel;
        private int _passedLevels;
        private float _delayer;
        private float _boostTimer;
        private int _skillCount;
        private float _originalSkillPanelTime;
        private bool _hasBonusHealOffer, _hasReduceCooldownOffer, _hasReduceWantedLevelOffer;
        private bool _onSkillBoost;
        
        
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

            _source = GetComponent<AudioSource>();

        }

        private void Start()
        {
            _originalSkillPanelTime = skillPanelTime;
            GameManager.Instance.onSurvive = true;
            SetChaosValue();
            _delayer = skillPanelTime;
            var r = Random.Range(0, 101);
            if(r<61)
                return;
            Invoke(nameof(ShowExtraSkillOffer), 1);
        }

        private void ShowExtraSkillOffer()
        {
            Time.timeScale = 0;
            var panel = extraSkillPopUp;
            panel.transform.localScale = Vector3.zero;
            panel.SetActive(true);
            panel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);

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
            LevelSignals.Instance.OnRevive += Revive;
            LevelSignals.Instance.OnPlayerDestroyed += Lose;
            CoreGameSignals.Instance.OnLevelWin += Lose;
            LevelSignals.Instance.OnBonusHealing += CloseHealOffer;
            // CoreGameSignals.Instance.OnSave += Save;
            // CoreGameSignals.Instance.OnLevelWin += LevelWin;
            // CoreGameSignals.Instance.OnLevelLoad += LoadScene;
        }
        
        private void UnSubscribe()
        {
            LevelSignals.Instance.OnPoliceEliminated -= IncreaseWantedLevel;
            LevelSignals.Instance.OnRevive -= Revive;
            CoreGameSignals.Instance.OnLevelWin -= Lose;
            LevelSignals.Instance.OnBonusHealing -= CloseHealOffer;
            // CoreGameSignals.Instance.OnSave -= Save;
            // CoreGameSignals.Instance.OnLevelWin -= LevelWin;
            // CoreGameSignals.Instance.OnLevelLoad -= LoadScene;
        }

        #endregion

        private void Update()
        {
            OpenSkillPanel();
            ApplySkillTimeBoost();
        }

        private void CloseHealOffer()
        {
            criticalHealthPopUp.SetActive(false);
        }
        private void ApplySkillTimeBoost()
        {
            if(!_onSkillBoost)
                return;
            var minutes = (int)_boostTimer / 60;
            var seconds = (int)_boostTimer % 60;

            if (skillBoostText)
            {
                skillBoostText.text = minutes.ToString("00") + " : " + seconds.ToString("00");
            }
            
            if (_boostTimer > 0)
            {
                skillPanelTime = _originalSkillPanelTime / 2;
                
            }
            
            else
            {
                skillPanelTime = _originalSkillPanelTime;
                _onSkillBoost = false;
                skillBoostText.transform.parent.gameObject.SetActive(false);
            }

            _boostTimer -= Time.deltaTime;
            _boostTimer = Mathf.Clamp(_boostTimer, 0, 60);
        }

        private void Revive()
        {
            if (skillSpecsList.Count > 0 || chosenSkills.Count > 0)
            {
                onLose = false;
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Player_Revive");

            }
        }

        private void Lose()
        {
            GameManager.Instance.onSurvive = false;
            CoreGameSignals.Instance.OnSave?.Invoke();
            onLose = true;
            var r = Random.Range(0, wantedLevelIncreaseSounds.Length);
            _source.PlayOneShot(wantedLevelIncreaseSounds[r],.5f);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Player_Lose");
            
            
        }
        private void ChooseRandomSkill()
        {

            
            if (chosenSkills.Count>0)
            {
                foreach (var skill in chosenSkills)
                {
                    skillSpecsList.Add(skill);
                }
                chosenSkills.Clear();
            }
            if(skillSpecsList.Count<1)
                return;
            LevelSignals.Instance.OnSkillPanel?.Invoke(true);
            
            var chooseCount = 3;
            if (skillSpecsList.Count<3)
            {
                chooseCount = skillSpecsList.Count;
            }
            for (var i = 0; i < chooseCount; i++)
            {
                var r = Random.Range(0, skillSpecsList.Count);
                var skill = skillSpecsList[r];
                skillSpecsList.Remove(skill);
                chosenSkills.Add(skill);
            }

            for (var i = 0; i < chooseCount; i++)
            {
                skillButtons[i].SetSkill(chosenSkills[i]);
                if (chooseCount<3)
                {
                    skillButtons[chooseCount].gameObject.SetActive(false);
                }
            }
        }
        public void OpenSkillPanel()
        {
            if(onLose)
                return;
            
            _delayer -= Time.deltaTime;
            _delayer = Mathf.Clamp(_delayer, 0, skillPanelTime);
            if (_delayer>0)
                return;
            SkillSignals.Instance.OnSkillPanelOpened?.Invoke(true);
            ChooseRandomSkill();
            _delayer += skillPanelTime;
            UIManager.Instance.OpenSkillPanel();
            AdManager.Instance.ShowInter();
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
                var r = Random.Range(0, wantedLevelIncreaseSounds.Length);
                _source.PlayOneShot(wantedLevelIncreaseSounds[r],.5f);
                UIManager.Instance.SetStars(5);
                if(_hasReduceWantedLevelOffer)
                    return;
                Invoke(nameof(ShowWantedLevelOffer),10);
                return;
            }
            if (_wantedLevel>=4)
            {
                if(_passedLevels==4)
                    return;
                _passedLevels = 4;
                var r = Random.Range(0, wantedLevelIncreaseSounds.Length);
                _source.PlayOneShot(wantedLevelIncreaseSounds[r],.5f);
                UIManager.Instance.SetStars(4);
                return;
            }
            if (_wantedLevel>=3)
            {
                if(_passedLevels==3)
                    return;
                _passedLevels = 3;
                var r = Random.Range(0, wantedLevelIncreaseSounds.Length);
                _source.PlayOneShot(wantedLevelIncreaseSounds[r],.5f);
                UIManager.Instance.SetStars(3);
                return;
            }
            if (_wantedLevel>=2)
            {
                if(_passedLevels==2)
                    return;
                _passedLevels = 2;
                var r = Random.Range(0, wantedLevelIncreaseSounds.Length);
                _source.PlayOneShot(wantedLevelIncreaseSounds[r],.5f);
                UIManager.Instance.SetStars(2);
                return;
            }
            if (_wantedLevel<2)
            {
                if(_passedLevels==1)
                    return;
                var r = Random.Range(0, wantedLevelIncreaseSounds.Length);
                _source.PlayOneShot(wantedLevelIncreaseSounds[r],.5f);
                _passedLevels = 1;
                UIManager.Instance.SetStars(1);
            }
        }

        public int GetWantedLevel() => _passedLevels;

        public void SetSkillCount()
        {
            _skillCount++;
            if (_skillCount < 6 ) 
                return;
            Invoke(nameof(ShowReduceSkillOffer),.2f);

        }
        
        private void ShowReduceSkillOffer()
        {
            if(_hasReduceCooldownOffer)
                return;
            _hasReduceCooldownOffer = true;
            Time.timeScale = 0;
            var panel = reduceSkillChoosePopUp;
            panel.transform.localScale = Vector3.zero;
            panel.SetActive(true);
            panel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
        }
        private void ShowWantedLevelOffer()
        {
            if(_hasReduceWantedLevelOffer)
                return;
            _hasReduceWantedLevelOffer = true;
            Time.timeScale = 0;
            var panel = wantedLevelPopUp;
            panel.transform.localScale = Vector3.zero;
            panel.SetActive(true);
            panel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
        }
        
        public void ShowHealOffer()
        {
            if(_hasBonusHealOffer)
                return;
            _hasBonusHealOffer = true;
            Time.timeScale = 0;
            var panel = criticalHealthPopUp;
            panel.transform.localScale = Vector3.zero;
            panel.SetActive(true);
            panel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
        }

        public void ReduceWantedLevel()
        {
            wantedLevelPopUp.SetActive(false);
            _wantedLevel = 4;
            _passedLevels = 3;
        }

        public void ReduceSkillChooseTime()
        {
            reduceSkillChoosePopUp.SetActive(false);
            _boostTimer = 60;
            _onSkillBoost = true;
            if(!skillBoostText)
                return;
            var parent = skillBoostText.transform.parent;
            parent.gameObject.SetActive(true);
            parent.DOScale(Vector3.one * 1.1f, .4f).SetLoops(7, LoopType.Yoyo);
        }

        public void OpenBonusSkill()
        {
            _delayer = 0;
            extraSkillPopUp.SetActive(false);
        }

        public void ClaimPopUpReward(int rewardID)
        {
            // 0-Extra Skill 1-Critical Health 2- Wanted Level Decrease 3- Reduce Skill Choose Time
            switch (rewardID)
            {
                case 0:
                    AdManager.Instance.showReward("ExtraSkillOffer");
                    break;
                case 1:
                    AdManager.Instance.showReward("BonusHealOffer");
                    break;
                case 2:
                    AdManager.Instance.showReward("ReduceWantedLevelOffer");
                    break;
                case 3:
                    AdManager.Instance.showReward("ReduceSkillChooseTimeOffer");
                    break;
            }
        }
    }
}