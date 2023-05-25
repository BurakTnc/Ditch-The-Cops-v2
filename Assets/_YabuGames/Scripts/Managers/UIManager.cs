using System;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using TMPro;
using UnityEngine;  
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using GameAnalyticsSDK;

namespace _YabuGames.Scripts.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [SerializeField] private GameObject revivePanel, gamePanel, winPanel, skillPanel, losePanel, missionsPanel;
        [SerializeField] private TextMeshProUGUI[] moneyText;
        [SerializeField] private Image[] stars = new Image[5];
        [SerializeField] private Image healthBar;
        [SerializeField] private Sprite yellowStarSprite;
        [SerializeField] private TextMeshProUGUI[] scoreTexts;
        [SerializeField] private TextMeshProUGUI[] rewardMoneyTexts;
        [SerializeField] private TextMeshProUGUI surviveTimeText;

        [Header("Player Profile")] 
        [SerializeField] private TextMeshProUGUI playerXpText;
        [SerializeField] private TextMeshProUGUI playerLevel;
        [SerializeField] private Image playerProgressBar;
        [Header("Missions")] 
        [SerializeField] private TextMeshProUGUI eliminatedCopsText;
        [SerializeField] private TextMeshProUGUI survivedTimeText;
        [SerializeField] private TextMeshProUGUI reachedLevelText;
        [SerializeField] private Slider eliminatedCopsProgressBar;
        [SerializeField] private Slider survivedTimeProgressBar;
        [SerializeField] private Slider reachedLevelProgressBar;
        [SerializeField] private Button[] missionClaimButtons;

        private int _targetLevelXp;
        private int _targetEliminate;
        private int _targetSurviveTime;
        private int _targetReachedLevel;
        private int _playerLevel;
        private int _earnedMoney;
        private AudioSource _audioSource;
        private Sprite _defaultStarSprite;


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

            if (transform.TryGetComponent(out AudioSource source))
            {
                _audioSource = source;
            }
            
        }

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
            SetMoneyTexts();
            GetTargetValues();
            SetPlayerProgress();
            if (SceneManager.GetActiveScene().buildIndex != 0) 
            {
                _defaultStarSprite = stars[3].sprite;
                OpenMissionsPanel(false);
                return;
            }
            Time.timeScale = 1;
        }

        private void GetTargetValues()
        {
            _targetLevelXp = StatsManager.Instance.targetLevelXp;
            _playerLevel = StatsManager.Instance.playerLevel;
            _targetEliminate = StatsManager.Instance.targetEliminate;
            _targetSurviveTime = StatsManager.Instance.targetSurvivedTime;
            _targetReachedLevel = StatsManager.Instance.targetReachedLevel;
        }

        private void SetSurviveText()
        {
            if (!surviveTimeText)
                return;

            var total = GameManager.Instance.survivedTimePerAPlay;
            var minute = (int)(total / 60);
            var second = (int)(total % 60);

            // Format the time as a string
            var minuteString = minute.ToString("D2");
            var secondString = second.ToString("D2");

            surviveTimeText.text = $"{minuteString}:{secondString}";
   
        }
        public void SetPlayerProgress()
        {
            GetTargetValues();
            if (!playerXpText)
                return;
            var reachedXp = GameManager.Instance.GetPlayerXp();

            var reachedXpAmount = reachedXp / _targetLevelXp;
            playerProgressBar.DOFillAmount(reachedXpAmount, 2).SetEase(Ease.OutBack)
                .OnComplete(StatsManager.Instance.SetPlayerLevel).SetDelay(.3f);
            playerXpText.text = reachedXp + "/" + _targetLevelXp;
            playerLevel.text = _playerLevel.ToString();
        }

        private void SetProgressBars(bool onMainMenu=true)
        {
            if (!eliminatedCopsProgressBar)
                return;

            var delay = onMainMenu ? .3f : 1f;
            var eliminatedCops = GameManager.Instance.GetEliminatedCops();

            eliminatedCopsProgressBar.value = 0;
            eliminatedCopsProgressBar.maxValue = _targetEliminate;
            eliminatedCopsProgressBar.DOValue(eliminatedCops, 2).SetEase(Ease.OutBack).SetUpdate(UpdateType.Late, true)
                .SetDelay(delay);
            eliminatedCopsText.text = eliminatedCops + "/" + _targetEliminate;

            var survivedTime = GameManager.Instance.GetSurvivedTime();

            survivedTimeProgressBar.value = 0;
            survivedTimeProgressBar.maxValue = _targetSurviveTime;
            survivedTimeProgressBar.DOValue((int)(survivedTime / 60), 2).SetEase(Ease.OutBack)
                .SetUpdate(UpdateType.Late, true).SetDelay(delay);
            survivedTimeText.text = (int)(survivedTime / 60) + "/" + _targetSurviveTime;

            reachedLevelProgressBar.value = 0;
            reachedLevelProgressBar.maxValue = _targetReachedLevel;
            reachedLevelText.text = _playerLevel + "/" + _targetReachedLevel;
            reachedLevelProgressBar.DOValue(_playerLevel, 2).SetEase(Ease.OutBack).SetUpdate(UpdateType.Late, true)
                .SetDelay(delay);

        }

        private void Update()
        {
            SetMoneyTexts();
            SetSurviveText();
        }

        #region Subscribtions
        private void Subscribe()
        {
            CoreGameSignals.Instance.OnLevelWin += LevelWin;
            CoreGameSignals.Instance.OnLevelFail += LevelLose;
            LevelSignals.Instance.OnPlayerDestroyed += OpenRevivePanel;
            //CoreGameSignals.Instance.OnGameStart += OnGameStart;
        }

        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnLevelWin -= LevelWin;
            CoreGameSignals.Instance.OnLevelFail -= LevelLose;
            LevelSignals.Instance.OnPlayerDestroyed -= OpenRevivePanel;
            //CoreGameSignals.Instance.OnGameStart -= OnGameStart;
        }

        #endregion
        
        private void OnGameStart()
        {
            // mainPanel.SetActive(false);
            // gamePanel.SetActive(true);
        }
        private void SetMoneyTexts()
        {
            if (moneyText.Length <= 0) return;

            foreach (var t in moneyText)
            {
                if (t)
                {
                    t.text = GameManager.Instance.GetMoney().ToString();
                }
            }
        }

        private void OpenRevivePanel()
        {
            revivePanel.transform.localScale = Vector3.zero;
            revivePanel.SetActive(true);
            revivePanel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
        }
        private void LevelWin()
        {
            foreach (var t in scoreTexts)
            {
                t.text = GameManager.Instance.GetCurrentSurvivedTime().ToString();
            }

            foreach (var t in rewardMoneyTexts)
            {
                t.text = GameManager.Instance.GetEarnedMoney().ToString();
            }
            CoreGameSignals.Instance.OnSave?.Invoke();
            Time.timeScale = 0;
            gamePanel.SetActive(false);
            winPanel.transform.localScale = Vector3.zero;
            winPanel.SetActive(true);
            winPanel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
            HapticManager.Instance.PlaySuccessHaptic();
        }

        private void LevelLose()
        {
            CalculateEarnings(GameManager.Instance.GetEarnedMoney());
            CoreGameSignals.Instance.OnSave?.Invoke();
            Time.timeScale = 0;
            gamePanel.SetActive(false);
            losePanel.transform.localScale = Vector3.zero;
            losePanel.SetActive(true);
            losePanel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
            HapticManager.Instance.PlayFailureHaptic();
        }

        private void CalculateEarnings(int earnedValue)
        {
            foreach (var t in scoreTexts)
            {
                
                t.text = GameManager.Instance.GetCurrentSurvivedTime().ToString();
            }

            foreach (var t in rewardMoneyTexts)
            {
                t.text = _earnedMoney.ToString();
                DOTween.To(GetValue, SetValue, earnedValue, .8f).OnUpdate(UpdateValue)
                    .SetEase(Ease.OutSine).SetDelay(1);
                

                void UpdateValue()
                {
                    t.text = _earnedMoney.ToString();
                }
                
            }
            

            int GetValue()
            {
                return _earnedMoney;
            }

            void SetValue(int value)
            {
                _earnedMoney = value;
            }
        }

        public void DoubleTheIncome(GameObject button)
        {
            MaxManager._instance.button = button;
           MaxManager._instance.ShowRewarded("DoubleCoin");
        }

        public void CoinReward(GameObject button)
        {
            button.SetActive(false);
            CalculateEarnings(GameManager.Instance.GetEarnedMoney()*2);
            var r = Random.Range(10, 15);
            CoreGameSignals.Instance.OnSpawnCoins?.Invoke(r,0,0,true);
            HapticManager.Instance.PlaySelectionHaptic(); 
        }

        public void UpdateHealthBar(float amount)
        {
            healthBar.fillAmount = amount;
        }

        public void PlayMaxHpAnimation()
        {
            healthBar.transform.parent.DOScaleX(1.1f, .3f).SetLoops(6, LoopType.Yoyo);
        }
        public void SetStars(int wantedLevel)
        {
            foreach (var t in stars)
            {
                t.sprite = _defaultStarSprite;
            }

            for (var i = 0; i < wantedLevel; i++)
            {
                stars[i].sprite = yellowStarSprite;
                stars[i].transform.DOScale(Vector3.one * 1.5f, .3f).SetLoops(10, LoopType.Yoyo);
            }
        }
        public void OpenSkillPanel()
        {
            Time.timeScale = 0;
            skillPanel.transform.localScale=Vector3.zero;
            skillPanel.SetActive(true);
            skillPanel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutSine);
        }
        public void CloseSkillPanel()
        {
            skillPanel.transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InBack).OnComplete(Continue);

            void Continue()
            {
                Time.timeScale = 1;
                skillPanel.SetActive(false);
                LevelSignals.Instance.OnSkillPanel?.Invoke(false);
            }
        }

        public void OpenMissionsPanel(bool onMainMenu = true)
        {
            if(!missionsPanel)
                return;
            var duration = onMainMenu ? .5f : 1.2f;
            missionsPanel.SetActive(true);
            missionsPanel.transform.localScale = Vector3.zero;
            missionsPanel.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack).SetUpdate(UpdateType.Late, true);
            SetProgressBars(onMainMenu);
            HapticManager.Instance.PlaySelectionHaptic();
            if(onMainMenu)
                return;
            missionsPanel.transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack)
                .SetUpdate(UpdateType.Late, true).OnComplete(StartTheGame).SetDelay(2.5f);

            void StartTheGame()
            {
                Time.timeScale = 1;
                missionsPanel.SetActive(false);
            }
        }

        public void OpenPanel(GameObject panel)
        {
            if(!panel)
                return;
            panel.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);
            HapticManager.Instance.PlaySelectionHaptic();
            try
            {
                var iap = GameObject.Find("IAPMANAGER");
                if (iap.TryGetComponent(out IAPManager manager))
                {
                    manager.coinSetPriceText();
                }

            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void SearchForAdCloseButton()
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
                return;
            
            var closeButton = GameObject.Find("MaxInterstitialCloseButton").GetComponent<Button>();
            if (closeButton)
            {
                closeButton.onClick.AddListener(InitializeLevel);
            }
            else
            {
                InitializeLevel();
            }
        }

        private void InitializeLevel()
        {
            CoreGameSignals.Instance.OnSave?.Invoke();
            CoreGameSignals.Instance.OnGameStart?.Invoke();
            HapticManager.Instance.PlaySelectionHaptic();
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Player_Play");
        }
        public void PlayButton()
        {
            AdManager.Instance.ShowInter(true);
            SearchForAdCloseButton();
            //Invoke(nameof(SearchForAdCloseButton), 1);
            // CoreGameSignals.Instance.OnSave?.Invoke();
            // CoreGameSignals.Instance.OnGameStart?.Invoke();
            // HapticManager.Instance.PlaySelectionHaptic();
            // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Player_Play");

        }

        public void MenuButton()
        {
            CoreGameSignals.Instance.OnSave?.Invoke();
            CoreGameSignals.Instance.OnMainMenu?.Invoke();
            HapticManager.Instance.PlayLightHaptic();
        }

        public void NextButton()
        {
            CoreGameSignals.Instance.OnSave?.Invoke();
            CoreGameSignals.Instance.OnGameStart?.Invoke();
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void RetryButton()
        {
            CoreGameSignals.Instance.OnSave?.Invoke();
            CoreGameSignals.Instance.OnGameStart?.Invoke();
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void OpenClaimButton(int buttonID)
        {
            if(missionClaimButtons.Length<1 || !missionClaimButtons[buttonID])
                return;
            
            missionClaimButtons[buttonID].interactable = true;
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Mission_Completed_Prize");

        }

        public void ClaimButton(int buttonID)
        {
            if (_audioSource)
            {
                _audioSource.Stop();
                _audioSource.Play();
            }
            missionClaimButtons[buttonID].interactable = false;
            CoreGameSignals.Instance.OnSpawnCoins?.Invoke(20, 0, 50, true);
            GameManager.Instance.ResetMissionProgress(buttonID);
            GetTargetValues();
            SetProgressBars();
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void BuyMapButton(int mapID)
        {
            StoreManager.Instance.UnlockMap(mapID);
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void BuyCarButton(int carID)
        {
            StoreManager.Instance.UnlockCar(carID);
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void DeclineReviveButton()
        {
            revivePanel.SetActive(false);
            CoreGameSignals.Instance.OnLevelFail?.Invoke();
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void ReviveButton()
        {
            revivePanel.SetActive(false);
            AdManager.Instance.ShowRewardedRevive();
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void OpenPopUpIcon(GameObject icon)
        {
            icon.SetActive(true);
            icon.transform.DOShakeRotation(5f, Vector3.forward * 7, 7, 100, true).SetLoops(-1);
        }

        public void ClosePopUpButton(GameObject panel)
        {
            panel.transform.DOScale(Vector3.zero, .2f).SetEase(Ease.InBack).OnComplete(DisablePanel);
            Time.timeScale = 1;
            
            void DisablePanel()
            {
                panel.SetActive(false);
            }
        }

        public void CloseButton(GameObject panel)
        {
            panel.transform.DOScale(Vector3.zero, .2f).SetEase(Ease.InBack).OnComplete(Disable);

            void Disable()
            {
                panel.SetActive(false);
            }
        }
        public void PopUpRewardedButton(int rewardID)
        {
            LevelManager.Instance.ClaimPopUpReward(rewardID);
        }
    }
}
